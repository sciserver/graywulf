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
    public partial class EntityLink : UserControlBase
    {
        private EntityProperty<Entity> entityReference;
        private string expression;

        public EntityProperty<Entity> EntityReference
        {
            get { return entityReference; }
        }

        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        public EntityLink()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.entityReference = new EntityProperty<Entity>();
            this.expression = null;
        }

        private void RefreshForm()
        {
            if (!entityReference.IsEmpty)
            {
                entityReference.Context = RegistryContext;
                Entity entity = entityReference.Value;

                Image.ImageUrl = String.Format("~/Icons/Small/{0}.gif", entity.EntityType);
                Image.Visible = true;

                if (expression == null)
                {
                    HyperLink.Text = entity.Name;
                }
                else
                {
                    HyperLink.Text = Registry.Util.ResolveExpression(EntityReference.Value, expression);
                }

                HyperLink.NavigateUrl = entity.GetDetailsUrl();
            }
            else
            {
                Image.Visible = false;
                HyperLink.Text = Resources.Labels.NotApplicable;
                HyperLink.NavigateUrl = string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

    }
}