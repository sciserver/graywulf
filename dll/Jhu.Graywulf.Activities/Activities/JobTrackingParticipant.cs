using System;
using System.Collections.Generic;
using System.Activities.Tracking;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
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

            new WorkflowInstanceQuery

            CustomTrackingQuery cq = new CustomTrackingQuery();
            cq.Name = "*";
            cq.ActivityName = "*";
            trackingProfile.Queries.Add(cq);

            ActivityStateQuery aq = new ActivityStateQuery();
            aq.ActivityName = "*";
            aq.States.Add("*");
            aq.Arguments.Add(Constants.ActivityParameterJobInfo);
            aq.Arguments.Add(Constants.ActivityParameterEntityGuid);
            aq.Arguments.Add(Constants.ActivityParameterEntityGuidFrom);
            aq.Arguments.Add(Constants.ActivityParameterEntityGuidTo);
            trackingProfile.Queries.Add(aq);

            FaultPropagationQuery fq = new FaultPropagationQuery();
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
                
                Logger.Instance.WriteEvent(e);
            }
        }

        private Event ProcessTrackingRecord(CustomTrackingRecord record)
        {
            if (record.Data != null && record.Data.ContainsKey(Constants.ActivityRecordDataItemEvent))
            {
                return (Event)record.Data[Constants.ActivityRecordDataItemEvent];
            }
            else
            {
                return null;
            }
        }

        private Event ProcessTrackingRecord(ActivityStateRecord record)
        {
            // Only record events of IGraywulfActivity activities
            if (record.Arguments.ContainsKey(Constants.ActivityParameterJobInfo))
            {
                var e = Logger.Instance.CreateEvent(
                    EventSeverity.Status,
                    EventSource.Workflow,
                    record.Activity.Name + " " + record.State,
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
                var e = Logger.Instance.CreateEvent(
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
            if (data.ContainsKey(Constants.ActivityParameterJobInfo))
            {
                var jobinfo = (JobInfo)data[Constants.ActivityParameterJobInfo];
                e.UserGuid = jobinfo.UserGuid;
                e.JobGuid = jobinfo.JobGuid;
            }

            // TODO Why don't we just copy everything?

            if (data.ContainsKey(Constants.ActivityParameterEntityGuid))
            {
                e.UserData[Constants.ActivityParameterEntityGuid] = (Guid)data[Constants.ActivityParameterEntityGuid];
            }

            if (data.ContainsKey(Constants.ActivityParameterEntityGuidFrom))
            {
                e.UserData[Constants.ActivityParameterEntityGuidFrom] = (Guid)data[Constants.ActivityParameterEntityGuidFrom];
            }

            if (data.ContainsKey(Constants.ActivityParameterEntityGuidTo))
            {
                e.UserData[Constants.ActivityParameterEntityGuidTo] = (Guid)data[Constants.ActivityParameterEntityGuidTo];
            }
        }
    }
}
