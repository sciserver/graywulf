using System;
using System.Collections.Generic;
using System.Activities.Tracking;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    public class JobTrackingParticipant : TrackingParticipant
    {
        TrackingProfile trackingProfile;

        public override TrackingProfile TrackingProfile
        {
            get { return this.trackingProfile; }
            set { this.trackingProfile = value; }
        }

        public JobTrackingParticipant()
        {
            trackingProfile = new TrackingProfile();
            trackingProfile.ActivityDefinitionId = "*";
            trackingProfile.ImplementationVisibility = ImplementationVisibility.All;

            var cq = new CustomTrackingQuery();
            cq.Name = "*";
            cq.ActivityName = "*";
            trackingProfile.Queries.Add(cq);

            var aq = new ActivityStateQuery();
            aq.ActivityName = "*";
            aq.States.Add("*");
            aq.Arguments.Add(Activities.Constants.ActivityParameterJobInfo);
            aq.Arguments.Add(Activities.Constants.ActivityParameterEntityGuid);
            aq.Arguments.Add(Activities.Constants.ActivityParameterEntityGuidFrom);
            aq.Arguments.Add(Activities.Constants.ActivityParameterEntityGuidTo);
            trackingProfile.Queries.Add(aq);

            var fq = new FaultPropagationQuery();
            fq.FaultHandlerActivityName = "*";
            fq.FaultSourceActivityName = "*";
            trackingProfile.Queries.Add(fq);
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            Event e = null;

            if (record is WorkflowInstanceRecord)
            {
            }
            else if (record is ActivityStateRecord)
            {
                e = ProcessTrackingRecord((ActivityStateRecord)record);
            }
            else if (record is CustomTrackingRecord)
            {
                e = ProcessTrackingRecord((CustomTrackingRecord)record);
            }
            else if (record is FaultPropagationRecord)
            {
                e = ProcessTrackingRecord((FaultPropagationRecord)record);
            }
            else
            {
                // Other records are not tracked
            }

            if (e != null)
            {
                LoggingContext.Current.UpdateEvent(e);

                e.Source |= EventSource.Workflow;
                e.Severity = MapEventSeverity(record.Level);
                e.DateTime = record.EventTime;
                e.Order = record.RecordNumber;
                
                LoggingContext.Current.WriteEvent(e);
            }
        }

        private Event ProcessTrackingRecord(CustomTrackingRecord record)
        {
            if (record.Data != null && record.Data.ContainsKey(Activities.Constants.ActivityRecordDataItemEvent))
            {
                return (Event)record.Data[Activities.Constants.ActivityRecordDataItemEvent];
            }
            else
            {
                return null;
            }
        }

        private Event ProcessTrackingRecord(ActivityStateRecord record)
        {
            // Only record events of IGraywulfActivity activities
            if (record.Arguments.ContainsKey(Activities.Constants.ActivityParameterJobInfo))
            {
                var e = Logging.LoggingContext.Current.CreateEvent(
                    EventSeverity.Status,
                    EventSource.Workflow,
                    record.Activity.Name + " is " + record.State.ToLowerInvariant() + ".",
                    record.Activity.TypeName,
                    null,
                    null);

                e.ExecutionStatus = MapExecutionStatus(record.State);

                SetEventDetails(e, record.Arguments);

                return e;
            }
            else
            {
                return null;
            }
        }

        private Event ProcessTrackingRecord(FaultPropagationRecord record)
        {
            if (record.IsFaultSource)
            {
                var e = Logging.LoggingContext.Current.CreateEvent(
                    EventSeverity.Error,
                    EventSource.Job,
                    null,
                    record.FaultSource.TypeName,
                    record.Fault,
                    null);

                e.ExecutionStatus = ExecutionStatus.Faulted;

                // TODO: where to get context from?

                return e;
            }
            else
            {
                return null;
            }
        }

        private Logging.EventSeverity MapEventSeverity(System.Diagnostics.TraceLevel level)
        {
            switch (level)
            {
                case System.Diagnostics.TraceLevel.Error:
                    return Logging.EventSeverity.Error;
                case System.Diagnostics.TraceLevel.Verbose:
                    return Logging.EventSeverity.Debug;
                case System.Diagnostics.TraceLevel.Info:
                    return Logging.EventSeverity.Operation;
                case System.Diagnostics.TraceLevel.Off:
                    return Logging.EventSeverity.None;
                case System.Diagnostics.TraceLevel.Warning:
                    return Logging.EventSeverity.Warning;
                default:
                    throw new NotImplementedException();
            }
        }

        private Logging.ExecutionStatus MapExecutionStatus(string state)
        {
            ExecutionStatus exst;
            if (Enum.TryParse<ExecutionStatus>(state, true, out exst))
            {
                return exst;
            }
            else
            {
                return ExecutionStatus.Unknown;    //
            }
        }

        private void SetEventDetails(Event e, IDictionary<string,object> data)
        {
            if (data.ContainsKey(Activities.Constants.ActivityParameterJobInfo))
            {
                var jobinfo = (JobInfo)data[Activities.Constants.ActivityParameterJobInfo];
                e.UserGuid = jobinfo.UserGuid;
                e.JobGuid = jobinfo.JobGuid;
            }

            // TODO Why don't we just copy everything?

            if (data.ContainsKey(Activities.Constants.ActivityParameterEntityGuid))
            {
                e.UserData[Activities.Constants.ActivityParameterEntityGuid] = (Guid)data[Activities.Constants.ActivityParameterEntityGuid];
            }

            if (data.ContainsKey(Activities.Constants.ActivityParameterEntityGuidFrom))
            {
                e.UserData[Activities.Constants.ActivityParameterEntityGuidFrom] = (Guid)data[Activities.Constants.ActivityParameterEntityGuidFrom];
            }

            if (data.ContainsKey(Activities.Constants.ActivityParameterEntityGuidTo))
            {
                e.UserData[Activities.Constants.ActivityParameterEntityGuidTo] = (Guid)data[Activities.Constants.ActivityParameterEntityGuidTo];
            }
        }
    }
}
