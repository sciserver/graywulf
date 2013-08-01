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

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class EntityTitle : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UpdateForm();
            }
        }

        protected void UpdateForm()
        {
            Entity entity = ((IEntityForm)Page).Item;

            Icon.ImageUrl = String.Format("~/Icons/Large/{0}.gif", entity.EntityType);

            NameLabel.Text = Registry.Constants.EntityNames_Singular[entity.EntityType] + ":"; // TODO
            Name.Text = entity.Name;
        }
    }
}