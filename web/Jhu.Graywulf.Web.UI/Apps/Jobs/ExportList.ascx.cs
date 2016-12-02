using System;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ExportList : System.Web.UI.UserControl, IJobList
    {
        public MultiSelectGridView List
        {
            get { return list; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}