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
    public partial class Export : CommonPage
    {
        public static string GetUrl(Guid key, EntityGroup mask)
        {
            return String.Format("~/Common/Export.aspx?key={0}&mask={1}", key, mask);
        }

        protected EntityGroup Mask
        {
            get { return (EntityGroup)Enum.Parse(typeof(EntityGroup), Request.QueryString["mask"]); }
        }
        
        protected override void UpdateForm()
        {
        }

        protected override void ProcessForm()
        {
            LoadEntities();

            var s = new RegistrySerializer(Entities)
            {
                Recursive = recursive.Checked,
                ExcludeUserCreated = excludeUserCreated.Checked,
                EntityGroupMask = Mask,
            };

            var filename = String.Format("Registry_{0}.xml", Mask);
            var content = String.Format("attachment;{0}", filename);

            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Disposition", content);
            s.Serialize(Response.Output);
            Response.End();
        }
    }
}