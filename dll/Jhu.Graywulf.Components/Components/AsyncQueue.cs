using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public delegate void AsyncQueueCallback<T>(T item);

    public class AsyncQueue<T> : IDisposable
    {
        #region Private member variables

        private bool executing;
        private ConcurrentQueue<T> queue;
        private EventWaitHandle waitHandle;
        private bool stopRequested;
        private bool abortRequested;
        private Task worker;

        #endregion
        #region Properties

        public int Count
        {
            get { return queue.Count; }
        }

        public bool IsEmpty
        {
            get { return queue.IsEmpty; }
        }

        #endregion
        #region events

        public event EventHandler OnBatchStart;
        public event EventHandler<AsyncQueueItemProcessingEventArgs<T>> OnItemProcessing;
        public event EventHandler OnBatchEnd;
        public event EventHandler<AsyncQueueUnhandledExceptionEventArgs> OnUnhandledException;

        #endregion
        #region Constructors and initializers

        public AsyncQueue()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.executing = false;
            this.queue = null;
            this.waitHandle = null;
            this.stopRequested = false;
            this.abortRequested = false;
            this.worker = null;
        }

        public void Dispose()
        {
            if (executing)
            {
                Stop();
            }
        }

        #endregion

        public void Enqueue(T item)
        {
            if (!executing || stopRequested)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            queue.Enqueue(item);
            waitHandle.Set();
        }

        public void Start()
        {
            if (executing)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            executing = true;
            queue = new ConcurrentQueue<T>();
            waitHandle = new AutoResetEvent(false);
            stopRequested = false;
            abortRequested = false;
            worker = new Task(Worker);

            worker.Start();
        }

        public void Stop()
        {
            if (!executing)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            stopRequested = true;
            waitHandle.Set();
            worker.Wait();

            queue = null;
            waitHandle.Dispose();
            waitHandle = null;
            stopRequested = false;
            abortRequested = false;
            worker.Dispose();
            worker = null;
            executing = false;
        }

        private void Worker()
        {
            while (!stopRequested || queue.Count > 0)
            {
                T item;
                bool batch = false;

                while (queue.TryDequeue(out item))
                {
                    if (!batch)
                    {
                        try
                        {
                            OnBatchStart?.Invoke(this, new EventArgs());
                        }
                        catch (Exception ex)
                        {
                            if (ProcessUnhandledException(ex))
                            {
                                abortRequested = true;
                                break;
                            }
                        }

                        batch = true;
                    }

                    try
                    {
                        var e = new AsyncQueueItemProcessingEventArgs<T>(item);
                        OnItemProcessing(this, e);
                    }
                    catch (Exception ex)
                    {
                        if (ProcessUnhandledException(ex))
                        {
                            abortRequested = true;
                            break;
                        }
                    }
                }

                if (abortRequested)
                {
                    break;
                }

                if (batch)
                {
                    try
                    {
                        OnBatchEnd?.Invoke(this, new EventArgs());
                    }
                    catch (Exception ex)
                    {
                        if (ProcessUnhandledException(ex))
                        {
                            abortRequested = true;
                            break;
                        }
                    }
                    batch = false;
                }

                // block until woken up by an Enqueue operation
                waitHandle.WaitOne();
            }
        }

        private bool ProcessUnhandledException(Exception ex)
        {
            if (OnUnhandledException != null)
            {
                var e = new AsyncQueueUnhandledExceptionEventArgs(ex);
                OnUnhandledException(this, e);

                return e.Abort;
            }
            else
            {
                throw ex;
            }
        }
    }
}
