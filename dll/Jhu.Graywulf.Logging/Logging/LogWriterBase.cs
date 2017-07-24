using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Logging
{
    public abstract class LogWriterBase : IDisposable
    {
        #region Private member variables

        private bool isExecuting;
        private BlockingCollection<Event> queue;
        private CancellationTokenSource stopRequestSource;
        private CancellationToken stopRequestToken;
        private Task worker;

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
            this.stopRequestSource = null;
            this.worker = null;

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

        public void Start()
        {
            OnStart();

            if (isAsync)
            {
                queue = new BlockingCollection<Event>(Constants.LogWriterAsyncCapacity);
                stopRequestSource = new CancellationTokenSource();
                stopRequestToken = stopRequestSource.Token;
                worker = new Task(Worker);
                worker.Start();
            }

            isExecuting = true;
        }

        public void Stop()
        {
            if (isAsync)
            {
                stopRequestSource.Cancel();
                worker.Wait();
            }

            OnStop();

            isExecuting = false;
        }

        private void Worker()
        {
            Event e;

            while (true)
            {
                try
                {
                    e = queue.Take(stopRequestToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                OnWriteEvent(e);
            }
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        public void WriteEvent(Event e)
        {
            if (isAsync && !isExecuting)
            {
                throw new InvalidOperationException();
            }

            // Enforce mask
            if ((e.Source & this.SourceMask) != 0 &&
                (e.Severity & this.SeverityMask) != 0 &&
                (e.ExecutionStatus & this.StatusMask) != 0)
            {
                if (isAsync)
                {
                    queue.Add(e);
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

        public virtual IEnumerable<Check.CheckRoutineBase> GetCheckRoutines()
        {
            yield break;
        }
    }
}
