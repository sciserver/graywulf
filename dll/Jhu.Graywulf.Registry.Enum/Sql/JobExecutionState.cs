using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Registry.Sql
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct JobExecutionState : INullable
    {
        public static readonly int Unknown = (int)Jhu.Graywulf.Registry.JobExecutionState.Unknown;
        public static readonly int Scheduled = (int)Jhu.Graywulf.Registry.JobExecutionState.Scheduled;
        public static readonly int Starting = (int)Jhu.Graywulf.Registry.JobExecutionState.Starting;
        public static readonly int Executing = (int)Jhu.Graywulf.Registry.JobExecutionState.Executing;
        public static readonly int Completed = (int)Jhu.Graywulf.Registry.JobExecutionState.Completed;
        public static readonly int Persisting = (int)Jhu.Graywulf.Registry.JobExecutionState.Persisting;
        public static readonly int Persisted = (int)Jhu.Graywulf.Registry.JobExecutionState.Persisted;
        public static readonly int Failed = (int)Jhu.Graywulf.Registry.JobExecutionState.Failed;
        public static readonly int CancelReqested = (int)Jhu.Graywulf.Registry.JobExecutionState.CancelRequested;
        public static readonly int Cancelling = (int)Jhu.Graywulf.Registry.JobExecutionState.Cancelling;
        public static readonly int Cancelled = (int)Jhu.Graywulf.Registry.JobExecutionState.Cancelled;
        public static readonly int TimedOut = (int)Jhu.Graywulf.Registry.JobExecutionState.TimedOut;
        public static readonly int Suspended = (int)Jhu.Graywulf.Registry.JobExecutionState.Suspended;
        public static readonly int Resumed = (int)Jhu.Graywulf.Registry.JobExecutionState.Resumed;
        public static readonly int All = (int)Jhu.Graywulf.Registry.JobExecutionState.All;

#if false
        public static readonly int Suspended = (int)Jhu.Graywulf.Registry.JobExecutionState.Suspended;
        public static readonly int Resumed = (int)Jhu.Graywulf.Registry.JobExecutionState.Resumed;
#endif
        
        
        

        #region Dummy members to support SQL CLR UDTs

        private bool dummy;

        public bool IsNull
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public static JobExecutionState Null
        {
            get { throw new NotImplementedException(); }
        }

        public static JobExecutionState Parse(SqlString s)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}