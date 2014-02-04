using System;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Home.NavigateUrl = Jhu.Graywulf.Web.UI.Default.GetUrl();
            Schema.NavigateUrl = Jhu.Graywulf.Web.UI.Schema.Default.GetUrl();
            Query.NavigateUrl = Jhu.Graywulf.Web.UI.Query.Default.GetUrl();
            Jobs.NavigateUrl = Jhu.Graywulf.Web.UI.Jobs.Default.GetUrl();
            MyDB.NavigateUrl = Jhu.Graywulf.Web.UI.MyDB.Default.GetUrl();
            Api.NavigateUrl = Jhu.Graywulf.Web.UI.Api.Default.GetUrl();
            Docs.NavigateUrl = Jhu.Graywulf.Web.UI.Docs.Default.GetUrl();
        }
    }
}