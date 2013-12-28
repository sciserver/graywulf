using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICancelableTask
    {

        bool IsCanceled
        {
            [OperationContract]
            get;
        }

        [OperationContract]
        void Cancel();
    }

    /// <summary>
    /// Implements basic functions to cancel long-running operations.
    /// The class also supports task delegation to remote servers.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class CancelableTask : ICancelableTask
    {
        #region Private members

        /// <summary>
        /// Holds a reference to the async task.
        /// </summary>
        [NonSerialized]
        private Task task;

        /// <summary>
        /// Flags is the operation is cancelled
        /// </summary>
        [NonSerialized]
        private bool isCanceled;
        
        /// <summary>
        /// Holds a list of cancellable operations
        /// </summary>
        [NonSerialized]
        private Dictionary<string, ICancelableTask> cancelableTasks;

        #endregion

        /// <summary>
        /// Gets whether the task is cancelled.
        /// </summary>
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
            if (isCanceled)
            {
                throw new InvalidOperationException(ExceptionMessages.TaskAlreadyCanceled);
            }

            OnExecute();
        }

        /// <summary>
        /// When overriden in derived classes, executes the task logic.
        /// </summary>
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

            if (isCanceled)
            {
                throw new InvalidOperationException(ExceptionMessages.TaskAlreadyCanceled);
            }

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

        /// <summary>
        /// Cancels the task by cancelling all asynchronously running subtasks.
        /// </summary>
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

        /// <summary>
        /// Registers a cancellable subtask.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="task"></param>
        protected void RegisterCancelable(Guid key, ICancelableTask task)
        {
            RegisterCancelable(key.ToString(), task);
        }

        /// <summary>
        /// Registers a cancellable subtask.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="task"></param>
        protected void RegisterCancelable(string key, ICancelableTask task)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Add(key, task);
            }
        }

        /// <summary>
        /// Unregisters a cancellable subtask.
        /// </summary>
        /// <param name="key"></param>
        protected void UnregisterCancelable(Guid key)
        {
            UnregisterCancelable(key.ToString());
        }

        /// <summary>
        /// Unregisters a cancellable subtask.
        /// </summary>
        /// <param name="key"></param>
        protected void UnregisterCancelable(string key)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Remove(key);
            }
        }
    }
}
