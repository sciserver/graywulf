using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        private Registry.Cluster cluster;

        public Registry.Cluster Cluster
        {
            get
            {
                if (cluster == null && Session[Constants.SessionClusterGuid] != null)
                {
                    cluster = new Registry.Cluster(RegistryContext);
                    cluster.Guid = (Guid)Session[Constants.SessionClusterGuid];
                    cluster.Load();
                }

                return cluster;
            }
        }

        protected override void OnUserSignedIn()
        {
            base.OnUserSignedIn();

            if (Cluster == null)
            {
                cluster = RegistryUser.Cluster;

                Session[Constants.SessionClusterGuid] = cluster.Guid;
                Session[Constants.SessionClusterName] = cluster.Name;
            }
        }
    }
}