using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    class JobWorkflowEvent
    {
        private Job job;
        private WorkflowEventType workflowEvent;
        private Exception workflowException;

        public Job Job
        {
            get { return job; }
            set { job = value; }
        }

        public WorkflowEventType WorkflowEvent
        {
            get { return workflowEvent; }
            set { workflowEvent = value; }
        }

        public Exception WorkflowException
        {
            get { return workflowException; }
            set { workflowException = value; }
        }

        private void InitializeMembers()
        {
            this.job = null;
            this.workflowEvent = WorkflowEventType.Unknown;
            this.workflowException = null;
        }
    }
}
