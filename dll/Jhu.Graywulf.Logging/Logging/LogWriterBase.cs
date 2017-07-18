using System;
using System.Collections.Generic;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Logging
{
    public abstract class LogWriterBase : IDisposable
    {
        #region Private member variables

        private bool isExecuting;
        private AsyncQueue<Event> queue;

        private bool isAsync;
        private object syncRoot;
        private EventSource sourceMask;
        private EventSeverity severityMask;
        private ExecutionStatus statusMask;

        #endregion
        #region Properties

        public bool IsAsync
        {
            get { return isAsync; }
            set { isAsync = value; }
        }

        public EventSource SourceMask
        {
            get { return sourceMask; }
            set { sourceMask = value; }
        }

        public EventSeverity SeverityMask
        {
            get { return severityMask; }
            set { severityMask = value; }
        }

        public ExecutionStatus StatusMask
        {
            get { return statusMask; }
            set { statusMask = value; }
        }

        #endregion
        #region Constructors and initializers

        public LogWriterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.isExecuting = false;
            this.queue = null;

            this.isAsync = true;
            this.syncRoot = new object();
            this.sourceMask = EventSource.All;
            this.severityMask = EventSeverity.All;
            this.statusMask = ExecutionStatus.All;
        }

        public virtual void Dispose()
        {
            if (isExecuting)
            {
                Stop();
            }
        }

        #endregion
        
        private void Queue_OnBatchStart(object sender, EventArgs e)
        {
            OnBatchStart();
        }

        private void Queue_OnBatchEnd(object sender, EventArgs e)
        {
            OnBatchEnd();
        }

        private void Queue_OnItemProcessing(object sender, AsyncQueueItemProcessingEventArgs<Event> e)
        {
            OnWriteEvent(e.Item);
        }

        private void Queue_OnUnhandledException(object sender, AsyncQueueUnhandledExceptionEventArgs<Event> e)
        {
            OnUnhandledExpcetion(e.Exception);
        }

        public void Start()
        {
            isExecuting = true;

            if (isAsync)
            {
                queue = new AsyncQueue<Event>();
                queue.OnBatchStart += Queue_OnBatchStart;
                queue.OnBatchEnd += Queue_OnBatchEnd;
                queue.OnItemProcessing += Queue_OnItemProcessing;
                queue.OnUnhandledException += Queue_OnUnhandledException;

                queue.Start();
            }

            OnStart();

            if (!IsAsync)
            {
                OnBatchStart();
            }
        }

        public void Stop()
        {
            if (isAsync)
            {
                queue.Dispose();
                queue = null;
            }
            else
            {
                OnBatchEnd();
            }

            OnStop();

            isExecuting = false;
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        protected abstract void OnBatchStart();

        protected abstract void OnBatchEnd();

        public void WriteEvent(Event e)
        {
            // Enforce mask

            if ((e.Source & this.SourceMask) != 0 &&
                (e.Severity & this.SeverityMask) != 0 &&
                (e.ExecutionStatus & this.StatusMask) != 0)
            {
                if (isAsync)
                {
                    queue.Enqueue(e);
                }
                else
                {
                    lock (syncRoot)
                    {
                        OnWriteEvent(e);
                    }
                }
            }
        }

        protected abstract void OnWriteEvent(Event e);

        protected abstract void OnUnhandledExpcetion(Exception ex);

        public virtual IEnumerable<Check.CheckRoutineBase> GetCheckRoutines()
        {
            yield break;
        }
    }
}
