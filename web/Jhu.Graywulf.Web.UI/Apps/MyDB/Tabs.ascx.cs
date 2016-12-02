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
            summary.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Default.GetUrl();
            tables.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Tables.GetUrl();
            copy.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Copy.GetUrl();
            import.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Import.GetUrl();
            export.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.MyDB.Export.GetUrl();
        }
    }
}