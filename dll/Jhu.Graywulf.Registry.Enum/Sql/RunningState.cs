using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Registry.Sql
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct RunningState : INullable
    {

        public static readonly int Unknown = (int)Jhu.Graywulf.Registry.RunningState.Unknown;
        public static readonly int Running = (int)Jhu.Graywulf.Registry.RunningState.Running;
        public static readonly int Stopped = (int)Jhu.Graywulf.Registry.RunningState.Stopped;
        public static readonly int Paused = (int)Jhu.Graywulf.Registry.RunningState.Paused;
        public static readonly int Attached = (int)Jhu.Graywulf.Registry.RunningState.Attached;
        public static readonly int Detached = (int)Jhu.Graywulf.Registry.RunningState.Detached;

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

        public static RunningState Null
        {
            get { throw new NotImplementedException(); }
        }

        public static RunningState Parse(SqlString s)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}