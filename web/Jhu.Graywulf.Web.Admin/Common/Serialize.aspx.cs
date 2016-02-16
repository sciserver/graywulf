using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Common
{
    public partial class Serialize : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Common/Serialize.aspx?guid={0}", guid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Ok_Click(object sender, EventArgs e)
        {
            var f = new EntityFactory(RegistryContext);
            var entity = f.LoadEntity(new Guid(Request.QueryString["guid"]));

            Response.ContentType = "text/xml";
            f.Serialize(entity, Response.Output, EntityGroup.All, true, false);
            Response.End();
        }
    }
}