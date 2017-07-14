using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    [Serializable]
    public class JobContext : JobInfo
    {
        #region Singleton

        [ThreadStatic]
        private static JobContext context;

        public static JobContext Current
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
            }
        }

        #endregion
        #region Private member variables

        #endregion
        #region Properties

        #endregion

        internal JobContext()
        {
            InitializeMembers(new StreamingContext());
        }

        internal JobContext(IJobActivity activity, CodeActivityContext activityContext)
            :base(activityContext.GetValue(activity.JobInfo))
        {
            InitializeMembers(new StreamingContext());
        }
        
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }
        
        private void CopyMembers(JobContext old)
        {
        }

        public void Push()
        {
            context = this;
        }

        public void Pop()
        {
            context = null;
        }
    }
}
