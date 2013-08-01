using System;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Home.NavigateUrl = Jhu.Graywulf.Web.Admin.Default.GetUrl();

            if (Request.IsAuthenticated)
            {
                Cluster.NavigateUrl = Jhu.Graywulf.Web.Admin.Cluster.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Federation.NavigateUrl = Jhu.Graywulf.Web.Admin.Federation.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Layout.NavigateUrl = Jhu.Graywulf.Web.Admin.Layout.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Jobs.NavigateUrl = Jhu.Graywulf.Web.Admin.Jobs.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Security.NavigateUrl = Jhu.Graywulf.Web.Admin.Security.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Monitor.NavigateUrl = Jhu.Graywulf.Web.Admin.Monitor.ClusterDetails.GetUrl((Guid)Session[Constants.SessionClusterGuid]);
                Log.NavigateUrl = Jhu.Graywulf.Web.Admin.Log.Default.GetUrl();
                Docs.NavigateUrl = Jhu.Graywulf.Web.Admin.Docs.Default.GetUrl();
            }
            else
            {
                Cluster.Visible = false;
                Federation.Visible = false;
                Layout.Visible = false;
                Jobs.Visible = false;
                Security.Visible = false;
                Monitor.Visible = false;
                Log.Visible = false;
            }
        }
    }
}