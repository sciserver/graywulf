using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Tasks
{
    [Serializable]
    public abstract class CancelableTask : ICancelableTask
    {
        [NonSerialized]
        private Task task;

        [NonSerialized]
        private bool isCanceled;
        
        [NonSerialized]
        private Dictionary<string, ICancelableTask> cancelableTasks;

        public virtual bool IsCanceled
        {
            get { return isCanceled; }
        }

        public CancelableTask()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.task = null;
            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();
        }

        public virtual void Execute()
        {
            OnExecute();
        }

        protected abstract void OnExecute();

        /// <summary>
        /// When overriden in derived classes, executes the task asynchronously
        /// </summary>
        public virtual void BeginExecute()
        {
            if (task != null)
            {
                throw new InvalidOperationException(ExceptionMessages.TaskAlreadyRunning);
            }

            isCanceled = false;

            task = Task.Factory.StartNew(OnExecute);
        }

        /// <summary>
        /// Waits for the asynchronous task to complete
        /// </summary>
        public virtual void EndExecute()
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions != null && ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerExceptions[0];
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                task = null;
            }
        }

        public virtual void Cancel()
        {
            if (isCanceled)
            {
                throw new InvalidOperationException(ExceptionMessages.TaskAlreadyCanceled);
            }

            lock (cancelableTasks)
            {
                foreach (var t in cancelableTasks.Values)
                {
                    t.Cancel();
                }
            }

            isCanceled = true;
        }

        protected void RegisterCancelable(Guid key, ICancelableTask task)
        {
            RegisterCancelable(key.ToString(), task);
        }

        protected void RegisterCancelable(string key, ICancelableTask task)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Add(key, task);
            }
        }

        protected void UnregisterCancelable(Guid key)
        {
            UnregisterCancelable(key.ToString());
        }

        protected void UnregisterCancelable(string key)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Remove(key);
            }
        }
    }
}
