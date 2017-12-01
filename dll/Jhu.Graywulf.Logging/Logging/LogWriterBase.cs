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
        private ManualResetEvent workerWaitHandle;
        private Thread worker;

        private bool isAsync;
        private int asyncQueueSize;
        private int asyncTimeout;
        private bool failOnError;
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

        public int AsyncQueueSize
        {
            get { return asyncQueueSize; }
            set { asyncQueueSize = value; }
        }

        public int AsyncTimeout
        {
            get { return asyncTimeout; }
            set { asyncTimeout = value; }
        }

        public bool FailOnError
        {
            get { return failOnError; }
            set { failOnError = value; }
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
            this.asyncQueueSize = Constants.DefaultLogWriterAsyncQueueSize;
            this.asyncTimeout = Constants.DefaultLogWriterAsyncTimeout;
            this.failOnError = true;
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
                queue = new BlockingCollection<Event>(asyncQueueSize);
                stopRequestSource = new CancellationTokenSource();
                stopRequestToken = stopRequestSource.Token;

                // Create new background thread
                // Do not use thead pool here because we want more control
                workerWaitHandle = new ManualResetEvent(false);
                var start = new ThreadStart(Worker);
                worker = new Thread(start);
                worker.Start();
            }

            isExecuting = true;
        }

        public void Stop()
        {
            if (isAsync)
            {
                stopRequestSource.Cancel();
                workerWaitHandle.WaitOne();
            }

            OnStop();

            isExecuting = false;
        }

        private void Worker()
        {
            Thread.CurrentThread.Name = String.Format("Log writer thread for {0}", this.GetType().FullName);

            Event e;

            while (true)
            {
                try
                {
                    e = queue.Take(stopRequestToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful way of stopping
                    break;
                }

                try
                {
                    // TODO: when converting it to async/await, make sure to add some sleep
                    // time to avoid running away when an exception happens and
                    // the event is requeued

                    OnWriteEvent(e);
                }
                catch (Exception ex)
                {
                    // If the exception happens in the log writer, just put it back into the queue.
                    // If the error is permanent, it will fill the queue anyway and the error
                    // will eventually be reported.
                    queue.Add(e);
                    LoggingContext.Current.LogError(EventSource.Logger, ex);
                }
            }

            workerWaitHandle.Set();
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        public void WriteEvent(Event e)
        {
            if (isAsync && !isExecuting)
            {
                throw new InvalidOperationException();
            }

            try
            {
                // Enforce mask
                if ((e.Source & this.SourceMask) != 0 &&
                    (e.Severity & this.SeverityMask) != 0 &&
                    (e.ExecutionStatus & this.StatusMask) != 0)
                {
                    if (isAsync)
                    {
                        if (!queue.TryAdd(e, asyncTimeout))
                        {
                            throw Error.AsyncTimeout(this);
                        }
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
            catch (Exception)
            {
                if (failOnError)
                {
                    throw;
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
