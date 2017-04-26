using System;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class Menu : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Home.NavigateUrl = Jhu.Graywulf.Web.Admin.Default.GetUrl();

            if (Request.IsAuthenticated)
            {
                var clusterGuid = ((PageBase)Page).RegistryContext.ClusterReference.Guid;

                Search.NavigateUrl = Jhu.Graywulf.Web.Admin.Common.Search.GetUrl();
                Cluster.NavigateUrl = Jhu.Graywulf.Web.Admin.Cluster.ClusterDetails.GetUrl(clusterGuid);
                Domain.NavigateUrl = Jhu.Graywulf.Web.Admin.Domain.ClusterDetails.GetUrl(clusterGuid);
                Federation.NavigateUrl = Jhu.Graywulf.Web.Admin.Federation.ClusterDetails.GetUrl(clusterGuid);
                Layout.NavigateUrl = Jhu.Graywulf.Web.Admin.Layout.ClusterDetails.GetUrl(clusterGuid);
                Jobs.NavigateUrl = Jhu.Graywulf.Web.Admin.Jobs.ClusterDetails.GetUrl(clusterGuid);
                Monitor.NavigateUrl = Jhu.Graywulf.Web.Admin.Monitor.ClusterDetails.GetUrl(clusterGuid);
                Log.NavigateUrl = Jhu.Graywulf.Web.Admin.Log.Default.GetUrl();
                Docs.NavigateUrl = Jhu.Graywulf.Web.Admin.Docs.Default.GetUrl();
            }
            else
            {
                Search.Visible = false;
                Cluster.Visible = false;
                Domain.Visible = false;
                Federation.Visible = false;
                Layout.Visible = false;
                Jobs.Visible = false;
                Monitor.Visible = false;
                Log.Visible = false;
            }
        }
    }
}