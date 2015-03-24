using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class is used by command-line executor
    /// </remarks>
    public class WorkflowApplicationHost
    {
        class WorkflowDetails
        {
            public WorkflowApplication WorkflowApplication;
            public Exception LastException;
        }

        private object syncRoot;
        private bool stopRequested;

        private Dictionary<Guid, WorkflowDetails> workflows;

        private GraywulfTrackingParticipant graywulfLogger;

        public event EventHandler<HostEventArgs> WorkflowEvent;

        public WorkflowApplicationHost()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.syncRoot = new object();
            this.stopRequested = false;
            this.workflows = new Dictionary<Guid, WorkflowDetails>();
            this.graywulfLogger = null;
        }

        private void EnsureNotStopping()
        {
            lock (syncRoot)
            {
                if (stopRequested)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Start()
        {
            EnsureNotStopping();

            Jhu.Graywulf.Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.StreamLogWriter(Console.Out));
            graywulfLogger = new Jhu.Graywulf.Activities.GraywulfTrackingParticipant();
        }

        public void Stop(TimeSpan timeout)
        {
            lock (syncRoot)
            {
                if (stopRequested)
                {
                    throw new InvalidOperationException();
                }

                stopRequested = true;
            }

            // Wait until all workflows complete

            while (true)
            {
                lock (syncRoot)
                {
                    if (workflows.Count == 0)
                    {
                        break;
                    }
                }

                Thread.Sleep(100);  // TODO: use constant
            }

            graywulfLogger = null;
            stopRequested = false;
        }

        private WorkflowApplication CreateWorkflowApplication(Activity wf, Dictionary<string, object> par)
        {
            var wfapp = par == null ? new WorkflowApplication(wf) : new WorkflowApplication(wf, par);

            // Add necessary participants
            wfapp.Extensions.Add(graywulfLogger);

            // Wire-up workflow runtime events
            wfapp.OnUnhandledException = wfapp_OnUnhandledException;
            wfapp.Completed = wfapp_WorkflowCompleted;
            wfapp.Unloaded = wfapp_WorkflowUnloaded;
            wfapp.Aborted = wfapp_WorkflowAborted;

            return wfapp;
        }

        private void RegisterWorkflow(WorkflowApplication wfapp)
        {
            lock (syncRoot)
            {
                workflows.Add(wfapp.Id, new WorkflowDetails()
                {
                    WorkflowApplication = wfapp,
                });
            }
        }

        public Guid PrepareStartWorkflow(Activity wf, Dictionary<string, object> par)
        {
            var wfapp = CreateWorkflowApplication(wf, par);

            RegisterWorkflow(wfapp);

            return wfapp.Id;
        }

        public void RunWorkflow(Guid instanceId)
        {
            EnsureNotStopping();

            lock (syncRoot)
            {
                workflows[instanceId].WorkflowApplication.Run();
            }
        }

        private Guid CancelWorkflow(Guid instanceId)
        {
            workflows[instanceId].WorkflowApplication.Cancel(TimeSpan.FromMinutes(5));  // *** TODO: from settings
            return instanceId;
        }

        private void FinishWorkflow(Guid instanceId)
        {
            lock (syncRoot)
            {
                workflows.Remove(instanceId);
            }
        }

        #region Workflow host events

        private UnhandledExceptionAction wfapp_OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {

#if BREAKDEBUG
            System.Diagnostics.Debugger.Break();
#endif

            lock (syncRoot)
            {
                var workflow = workflows[e.InstanceId];
                workflow.LastException = e.UnhandledException;
            }

            // Force the workflow to execute the cancel logic
            return UnhandledExceptionAction.Cancel;
        }

        /// <summary>
        /// Executes when a workflow completed either successfully, either failing.
        /// </summary>
        /// <param name="e"></param>
        private void wfapp_WorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            WorkflowDetails workflow;
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            if (WorkflowEvent != null)
            {

                switch (e.CompletionState)
                {
                    case ActivityInstanceState.Closed:
                        // Completed successfully
                        WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Completed, e.InstanceId));
                        break;
                    case ActivityInstanceState.Canceled:
                        if (workflow.LastException == null)
                        {
                            WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Cancelled, e.InstanceId));
                        }
                        else
                        {
                            WorkflowEvent(this, new HostEventArgs(WorkflowEventType.Failed, e.InstanceId, workflow.LastException.Message));
                        }
                        break;
                    case ActivityInstanceState.Faulted:
                    // This should not happen, workflows are forced to cancel
                    case ActivityInstanceState.Executing:
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void wfapp_WorkflowUnloaded(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            FinishWorkflow(e.InstanceId);
        }

        private void wfapp_WorkflowAborted(WorkflowApplicationEventArgs e)
        {
            WorkflowDetails workflow;
            lock (syncRoot)
            {
                workflow = workflows[e.InstanceId];
            }

            FinishWorkflow(e.InstanceId);
        }

        #endregion
    }
}
