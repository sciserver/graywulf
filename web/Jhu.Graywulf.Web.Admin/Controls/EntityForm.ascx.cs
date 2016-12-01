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
    public partial class EntityForm : UserControl
    {
        public TextBox Name
        {
            get { return name; }
        }

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

            Icon.ImageUrl = String.Format("~/Icons/Large/{0}.gif", entity.EntityType);

            string op = entity.IsExisting ? Resources.Labels.ModifyEntity : Resources.Labels.CreateEntity;
            OperationLabel.Text = string.Format(op, Registry.Constants.EntityNames_Singular[entity.EntityType]);

            name.Text = entity.Name;
            displayName.Text = entity.DisplayName;
            version.Text = entity.Version;
            comments.Text = entity.Comments;
        }

        public void SaveForm()
        {
            Entity entity = ((IEntityForm)Page).Item;

            entity.Name = name.Text;
            entity.DisplayName = displayName.Text;
            entity.Version = version.Text;
            entity.Comments = comments.Text;
        }
    }
}