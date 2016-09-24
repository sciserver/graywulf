using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Tabs : System.Web.UI.UserControl
    {
        public string SelectedTab { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Summary.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Default.GetUrl();
            Tables.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Tables.GetUrl();
            Import.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Import.GetUrl();
            Export.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Export.GetUrl();

            TabHeader.SelectedTab = (Tab)TabHeader.FindControl(SelectedTab);
        }
    }
}