using System;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Copy : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string objid)
        {
            var url = "~/Apps/MyDb/Copy.aspx";

            if (objid != null)
            {
                url += String.Format("?objid={0}", objid);
            }

            return url;
        }

        protected DatabaseObject obj;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {       
            }
        }    

        protected void Ok_Click(object sender, EventArgs e)
        {
            Response.Redirect(Tables.GetUrl(), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }
    }
}