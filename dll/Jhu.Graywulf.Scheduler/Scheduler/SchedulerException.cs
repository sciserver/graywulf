using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Scheduler
{
    [Serializable]
    public class SchedulerException : Exception
    {
        public SchedulerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SchedulerException()
            : base()
        {
        }

        public SchedulerException(string message)
            : base(message)
        {
        }

        public SchedulerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
