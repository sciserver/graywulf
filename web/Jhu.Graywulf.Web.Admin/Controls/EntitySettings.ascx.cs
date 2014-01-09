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
    public partial class EntitySettings : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UpdateForm();
            }
            else
            {
                SaveForm();
            }
        }

        public void UpdateForm()
        {
            Entity entity = ((IEntityForm)Page).Item;

            Settings.Text = entity.Settings.XmlText;
        }

        public void SaveForm()
        {
            Entity entity = ((IEntityForm)Page).Item;

            entity.Settings.XmlText = Settings.Text;
        }
    }
}