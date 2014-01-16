using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class EntitySelect : System.Web.UI.UserControl
    {
        private EntityProperty<Entity> parentEntityProperty;
        private EntityProperty<Entity> selectedEntityProperty;
        private EntityType[] childEntityTypes;

        public Entity ParentEntity
        {
            get { return parentEntityProperty.Value; }
            set
            {
                parentEntityProperty.Value = value;
                RefreshForm();
            }
        }

        public EntityType[] ChildEntityTypes
        {
            get { return childEntityTypes; }
            set
            {
                childEntityTypes = value;
                RefreshForm();
            }
        }

        public Entity SelectedValue
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(entityList.SelectedValue))
                {
                    selectedEntityProperty.Guid = Guid.Parse(entityList.SelectedValue);
                }
                else
                {
                    selectedEntityProperty.Value = null;
                }

                return selectedEntityProperty.Value;
            }
            set
            {
                selectedEntityProperty.Value = value;
                entityList.SelectedValue = selectedEntityProperty.Guid.ToString();
            }
        }

        public EntitySelect()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.parentEntityProperty = new EntityProperty<Entity>(((PageBase)Page).RegistryContext);
            this.selectedEntityProperty = new EntityProperty<Entity>(((PageBase)Page).RegistryContext);
            this.childEntityTypes = null;
        }

        private void RefreshForm()
        {
            entityList.Items.Clear();
            entityList.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            if (!parentEntityProperty.IsEmpty && childEntityTypes != null)
            {
                RefreshListRecursively(ParentEntity, ParentEntity.Name, 0);
            }
        }

        private void RefreshListRecursively(Entity entity, string text, int depth)
        {
            if (depth < childEntityTypes.Length)
            {
                entity.LoadChildren(childEntityTypes[depth], false);
                foreach (var e in entity.EnumerateChildren(childEntityTypes[depth]))
                {
                    RefreshListRecursively(e, String.Format("{0}.{1}", text, e.Name), depth + 1);
                }
            }
            else
            {
                var li = new ListItem(text, entity.Guid.ToString());
                entityList.Items.Add(li);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}