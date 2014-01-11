using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Registry.Sql
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct ScheduleType : INullable
    {
        public static readonly int Unknown = (int)Jhu.Graywulf.Registry.ScheduleType.Unknown;
        public static readonly int Queued = (int)Jhu.Graywulf.Registry.ScheduleType.Queued;
        public static readonly int Timed = (int)Jhu.Graywulf.Registry.ScheduleType.Timed;
        public static readonly int Recurring = (int)Jhu.Graywulf.Registry.ScheduleType.Recurring;

        #region Dummy members to support SQL CLR UDTs

#pragma warning disable 169
        private bool dummy;
#pragma warning restore 169

        public bool IsNull
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public static ScheduleType Null
        {
            get { throw new NotImplementedException(); }
        }

        public static ScheduleType Parse(SqlString s)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}