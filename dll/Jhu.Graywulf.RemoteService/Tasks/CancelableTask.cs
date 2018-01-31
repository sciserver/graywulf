using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;
using System.ServiceModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICancelableTask
    {
        CancellationContext CancellationContext
        {
            get; set;
        }

        CancelableTaskProgress Progress
        {
            [OperationContract]
            get;
        }

        bool IsCancellationRequested
        {
            [OperationContract]
            get;
        }

        bool IsCancelled
        {
            [OperationContract]
            get;
        }

        [OperationContract]
        void Cancel();
    }

    /// <summary>
    /// Implements basic functions to cancel long-running operations via WCF.
    /// The class also supports task delegation to remote servers.
    /// </summary>
    /// <remarks>
    /// WCF doesn't support cancellation of async tasks with cancellation tokens
    /// but if objects are instantiated per session, calling a Cancel method can
    /// server the same purpose.
    /// </remarks>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class CancelableTask : ICancelableTask, IDisposable
    {
        #region Private member varuables

        [NonSerialized]
        private CancellationContext cancellationContext;

        [NonSerialized]
        private bool ownsCancellationContext;

        private bool isCancellationRequested;
        private bool isCancelled;

        /// <summary>
        /// Keeps track of the progress of the long running task.
        /// </summary>
        private CancelableTaskProgress progress;

        #endregion
        #region Properties

        protected CancellationContext CancellationContext
        {
            get { return cancellationContext; }
        }

        CancellationContext ICancelableTask.CancellationContext
        {
            get { return cancellationContext; }
            set
            {
                cancellationContext = value;
                ownsCancellationContext = false;
            }
        }

        /// <summary>
        /// Gets whether the task is cancelled, either by calling the Cancel method
        /// directly, or requesting a cancellation from the outside via a cancellation token.
        /// </summary>
        public bool IsCancellationRequested
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return isCancellationRequested; }
        }

        public bool IsCancelled
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return isCancelled; }
        }

        /// <summary>
        /// Gets the status of the long running task.
        /// </summary>
        public CancelableTaskProgress Progress
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return progress; }
        }

        #endregion
        #region Constructors and initializers

        public CancelableTask()
            :this(null)
        {
            // Overload
        }

        public CancelableTask(CancellationContext cancellationContext)
        {
            InitializeMembers();

            if (cancellationContext == null)
            {
                this.cancellationContext = new Tasks.CancellationContext();
                this.ownsCancellationContext = true;
            }
            else
            {
                this.cancellationContext = cancellationContext;
                this.ownsCancellationContext = false;
                this.cancellationContext.Register(this);
            }
        }

        private void InitializeMembers()
        {
            this.cancellationContext = null;
            this.isCancellationRequested = false;
            this.isCancelled = false;
            this.progress = new CancelableTaskProgress();
        }

        public virtual void Dispose()
        {
            if (ownsCancellationContext && cancellationContext != null)
            {
                cancellationContext.Dispose();
            }
        }

        #endregion
        #region Task logic implementation

        /// <summary>
        /// When overriden in derived classes, executes the task logic.
        /// </summary>
        protected abstract Task OnExecuteAsync();

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation(Constants.DefaultRole)]
        public async Task ExecuteAsync()
        {
            try
            {
                await OnExecuteAsync();
            }
            catch (Exception ex)
            {
                var helper = new Util.CancellationHelper(ex);

                if (isCancellationRequested && helper.IsCancelled)
                {
                    isCancelled = true;
                }
                else
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(helper.DispatchException()).Throw();
                }
            }
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation(Constants.DefaultRole)]
        public void Cancel()
        {
            this.isCancellationRequested = true;

            OnCancel();

            CancellationContext.Cancel();
        }

        protected virtual void OnCancel()
        {
        }

        #endregion
    }
}
