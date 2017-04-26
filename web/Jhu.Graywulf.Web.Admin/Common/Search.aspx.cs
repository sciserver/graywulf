using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Common
{
    public partial class Search : PageBase
    {

        public static string GetUrl()
        {
            return "~/Common/Search.aspx";
        }

        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            var s = new EntitySearch(RegistryContext);
            s.Name = name.Text;
            SearchResultList.Search = s;

            SearchResultList.Visible = true;
        }
    }
}