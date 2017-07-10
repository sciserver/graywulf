using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// Used to pass information about the workflow status to
    /// the event handlers.
    /// </summary>
    [Serializable]
    public class WorkflowApplicationHostEventArgs : EventArgs
    {
        /// <summary>
        /// Type of event happened.
        /// </summary>
        public WorkflowEventType EventType;

        /// <summary>
        /// Unique identifier of the workflow.
        /// </summary>
        public Guid InstanceId;

        /// <summary>
        /// Exception, if there is any.
        /// </summary>
        public string ExceptionMessage;

        public WorkflowApplicationHostEventArgs(WorkflowEventType eventType, Guid instanceId)
        {
            this.EventType = eventType;
            this.InstanceId = instanceId;
        }

        public WorkflowApplicationHostEventArgs(WorkflowEventType eventType, Guid instanceId, string exceptionMessage)
        {
            this.EventType = eventType;
            this.InstanceId = instanceId;
            this.ExceptionMessage = exceptionMessage;
        }
    }
}
