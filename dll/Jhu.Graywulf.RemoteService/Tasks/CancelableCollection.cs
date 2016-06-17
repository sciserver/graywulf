using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Tasks
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class CancelableCollection
    {
        /// <summary>
        /// Holds a list of ICancelableTask instances that are all to be canceled
        /// if the query workflow is canceled.
        /// </summary>
        [NonSerialized]
        private Dictionary<string, ICancelableTask> cancelableTasks;

        /// <summary>
        /// Flag to know if query was already cancelled. Used in ICancelableTask
        /// implementation
        /// </summary>
        [NonSerialized]
        private bool isCanceled;

        /// <summary>
        /// Gets if the query has been canceled.
        /// </summary>
        [IgnoreDataMember]
        public bool IsCanceled
        {
            get { return isCanceled; }
        }

        protected CancelableCollection()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();
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
    }
}
