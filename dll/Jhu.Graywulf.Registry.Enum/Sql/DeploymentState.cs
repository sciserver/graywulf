using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Registry.Sql
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct DeploymentState : INullable
    {

        /*
        public static readonly int Unknown = (int)Jhu.Graywulf.Registry.DeploymentState.Unknown;
        public static readonly int PendingNew = (int)Jhu.Graywulf.Registry.DeploymentState.PendingNew;
        public static readonly int PendingModify = (int)Jhu.Graywulf.Registry.DeploymentState.PendingModify;
        public static readonly int Deploying = (int)Jhu.Graywulf.Registry.DeploymentState.Deploying;
        public static readonly int Deployed = (int)Jhu.Graywulf.Registry.DeploymentState.Deployed;
        public static readonly int Detached = (int)Jhu.Graywulf.Registry.DeploymentState.Detached;
        public static readonly int PendingUndeploy = (int)Jhu.Graywulf.Registry.DeploymentState.PendingUndeploy;
        public static readonly int Undeploying = (int)Jhu.Graywulf.Registry.DeploymentState.Undeploying;
        public static readonly int Undeployed = (int)Jhu.Graywulf.Registry.DeploymentState.Undeployed;*/

        public static readonly int Unknown = (int)Jhu.Graywulf.Registry.DeploymentState.Unknown;
        public static readonly int New = (int)Jhu.Graywulf.Registry.DeploymentState.New;
        public static readonly int Deploying = (int)Jhu.Graywulf.Registry.DeploymentState.Deploying;
        public static readonly int Deployed = (int)Jhu.Graywulf.Registry.DeploymentState.Deployed;
        public static readonly int Undeploying = (int)Jhu.Graywulf.Registry.DeploymentState.Undeploying;
        public static readonly int Undeployed = (int)Jhu.Graywulf.Registry.DeploymentState.Undeployed;

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

        public static DeploymentState Null
        {
            get { throw new NotImplementedException(); }
        }

        public static DeploymentState Parse(SqlString s)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}