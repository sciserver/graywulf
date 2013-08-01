using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Logging
{
    public class EventFilter
    {
        private DateTime eventDateTimeFrom;
        private DateTime eventDateTimeTo;
        private EventSource eventSource;
        private EventSeverity eventSeverity;
        private List<ExecutionStatus> executionStatus;
        private Guid userGuid;
        private Guid jobGuid;
        private Guid contextGuid;
        private string operation;
        private Guid entityGuid;
        private string exceptionType;
        private string message;

        public DateTime EventDateTimeFrom
        {
            get { return eventDateTimeFrom; }
            set { eventDateTimeFrom = value; }
        }

        public DateTime EventDateTimeTo
        {
            get { return eventDateTimeTo; }
            set { eventDateTimeTo = value; }
        }

        public EventSource EventSource
        {
            get { return eventSource; }
            set { eventSource = value; }
        }

        public EventSeverity EventSeverity
        {
            get { return eventSeverity; }
            set { eventSeverity = value; }
        }

        public List<ExecutionStatus> ExecutionStatus
        {
            get { return executionStatus; }
        }

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        public string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        public Guid EntityGuid
        {
            get { return entityGuid; }
            set { entityGuid = value; }
        }

        public string ExceptionType
        {
            get { return exceptionType; }
            set { exceptionType = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public EventFilter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.eventDateTimeFrom = DateTime.MinValue;
            this.eventDateTimeTo = DateTime.MaxValue;
            this.eventSource = EventSource.All;
            this.eventSeverity = EventSeverity.All;
            this.executionStatus = new List<ExecutionStatus>();
            this.userGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.contextGuid = Guid.Empty;
            this.operation = string.Empty;
            this.entityGuid = Guid.Empty;
            this.exceptionType = string.Empty;
            this.message = string.Empty;
        }

        public string GenerateWhereCondition(SqlCommand cmd)
        {
            StringBuilder where = new StringBuilder();

            if (eventDateTimeFrom != DateTime.MinValue)
            {
                where.AppendLine("AND EventDateTime >= @EventDateTimeFrom");
                cmd.Parameters.Add("@EventDateTimeFrom", SqlDbType.DateTime).Value = eventDateTimeFrom;
            }
            if (eventDateTimeTo != DateTime.MaxValue)
            {
                where.AppendLine("AND EventDateTime <= @EventDateTimeTo");
                cmd.Parameters.Add("@EventDateTimeTo", SqlDbType.DateTime).Value = eventDateTimeTo;
            }
            if (eventSource != EventSource.None)
            {
                where.AppendLine("AND (EventSource & @EventSource) <> 0");
                cmd.Parameters.Add("@EventSource", SqlDbType.Int).Value = (int)eventSource;
            }
            if (eventSeverity != EventSeverity.None)
            {
                where.AppendLine("AND (EventSeverity & @EventSeverity) <> 0");
                cmd.Parameters.Add("@EventSeverity", SqlDbType.Int).Value = (int)eventSeverity;
            }
            if (executionStatus != null && executionStatus.Count > 0)
            {
                string l = string.Empty;
                for (int i = 0; i < executionStatus.Count; i++)
                {
                    if (i > 0) l += ",";
                    l += ((int)executionStatus[i]).ToString();
                }
                where.AppendLine("AND (ExecutionStatus IN (" + l + "))");
            }
            if (userGuid != Guid.Empty)
            {
                where.AppendLine("AND (UserGuid = @userGuid)");
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
            }
            if (jobGuid != Guid.Empty)
            {
                where.AppendLine("AND (JobGuid = @jobGuid)");
                cmd.Parameters.Add("@JobGuid", SqlDbType.UniqueIdentifier).Value = jobGuid;
            }
            if (contextGuid != Guid.Empty)
            {
                where.AppendLine("AND (ContextGuid = @contextGuid)");
                cmd.Parameters.Add("@ContextGuid", SqlDbType.UniqueIdentifier).Value = contextGuid;
            }
            if (entityGuid != Guid.Empty)
            {
                where.AppendLine("AND (EntityGuid = @entityGuid OR EntityGuidFrom = @entityGuid OR EntityGuidTo = @entityGuid)");
                cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier).Value = entityGuid;
            }

            if (where.Length > 0)
                return where.ToString(4, where.Length - 4); // strip the first AND
            else
                return null;
        }
    }
}
