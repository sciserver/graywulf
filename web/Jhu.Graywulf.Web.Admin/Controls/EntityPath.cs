using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    /// <summary>
    /// Summary description for EntityPathControlBase
    /// </summary>
    public class EntityPath : UserControlBase
    {
        protected EntityGroup entityGroupMask;

        protected override void OnLoad(EventArgs e)
        {
            entityGroupMask = EntityGroup.None;
            string dir = Request.AppRelativeCurrentExecutionFilePath;
            if (dir.Length > 2)
            {
                int idx = dir.IndexOf('/', 2);
                if (idx > 2)
                {
                    dir = dir.Substring(2, idx - 2);
                    Enum.TryParse<EntityGroup>(dir, true, out entityGroupMask);
                }
            }

            Entity entity = ((IEntityForm)Page).Item;

            this.AppRelativeTemplateSourceDirectory = Page.AppRelativeTemplateSourceDirectory;

            Entity ep = entity.Parent;

            Panel p = new Panel();
            p.CssClass = "Path";

            while (ep != null)
            {
                HyperLink link = new HyperLink();
                link.Text = ep.Name;
                link.NavigateUrl = ep.GetDetailsUrl(entityGroupMask); 

                Image img = new Image();
                img.ImageAlign = ImageAlign.Middle;
                img.ImageUrl = String.Format("~/Icons/Small/{0}.gif", ep.EntityType);

                p.Controls.AddAt(0, new LiteralControl("&nbsp;&nbsp;&nbsp;►&nbsp;&nbsp;&nbsp;"));
                p.Controls.AddAt(0, link);
                p.Controls.AddAt(0, new LiteralControl("&nbsp;&nbsp;"));
                p.Controls.AddAt(0, img);

                ep = ep.Parent;
            }

            p.Controls.AddAt(0, new LiteralControl("►&nbsp;&nbsp;&nbsp;"));
            this.Controls.Add(p);

            base.OnLoad(e);
        }
    }
}