using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Controls;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    [ParseChildren(true)]
    [PersistChildren(true)]
    public partial class EntityList : Jhu.Graywulf.Web.UI.UserControlBase
    {
        private Entity parentEntity;
        private EntityType childrenType;
        private EntityGroup entityGroup;
        private string text;
        private DataControlFieldCollection columns;

        public Entity ParentEntity
        {
            get { return parentEntity; }
            set { parentEntity = value; }
        }

        public EntityType ChildrenType
        {
            get { return childrenType; }
            set { childrenType = value; }
        }

        public EntityGroup EntityGroup
        {
            get { return entityGroup; }
            set { entityGroup = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public DataControlFieldCollection Columns
        {
            get { return this.columns; }
        }

        public string DataObjectTypeName
        {
            get { return InternalDataSource.DataObjectTypeName; }
            set { InternalDataSource.DataObjectTypeName = value; }
        }

        public HashSet<string> SelectedDataKeys
        {
            get { return InternalGridView.SelectedDataKeys; }
        }

        public string SelectedDataKey
        {
            get { return InternalGridView.SelectedDataKeys.FirstOrDefault(); }
        }

        public EntityList()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.parentEntity = null;
            this.childrenType = EntityType.Unknown;
            this.entityGroup = Registry.EntityGroup.Unknown;
            this.text = null;
            this.columns = new DataControlFieldCollection();
        }

        protected void InternalDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            if (parentEntity != null)
            {
                WebEntityFactory ef = new WebEntityFactory(((PageBase)Page).RegistryContext);
                ef.ParentEntity = parentEntity;
                ef.ChildrenType = childrenType;
                e.ObjectInstance = ef;
            }
            else
            {
                e.ObjectInstance = null;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var sf = new SelectionField();
            sf.ItemStyle.CssClass = "GridViewIcon";
            InternalGridView.Columns.Add(sf);

            var nf = new System.Web.UI.WebControls.BoundField();
            nf.DataField = "Number";
            nf.ItemStyle.CssClass = "GridViewIcon";
            InternalGridView.Columns.Add(nf);

            var imgf = new ImageField();
            imgf.DataImageUrlField = "EntityType";
            imgf.DataImageUrlFormatString = "~/Icons/Small/{0}.gif";
            imgf.ItemStyle.CssClass = "GridViewIcon";
            InternalGridView.Columns.Add(imgf);

            var namefield = new HyperLinkField();
            namefield.HeaderText = "Name";
            namefield.DataTextField = "Name";
            namefield.ItemStyle.CssClass = "GridViewSpan";
            namefield.DataNavigateUrlFields = new[] { "EntityType", "Guid" };
            namefield.DataNavigateUrlFormatString = "~/" + entityGroup.ToString() + "/{0}Details.aspx?guid={1}";
            InternalGridView.Columns.Add(namefield);

            foreach (DataControlField col in columns)
            {
                InternalGridView.Columns.Add(col);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Create.OnClientClick = Util.UrlFormatter.GetClientRedirect(parentEntity.GetNewChildFormUrl(childrenType));
            }
        }

        private void CopyItem(Guid guid)
        {
            var ef = new EntityFactory(RegistryContext);
            var item = ef.LoadEntity(guid);

            var copy = item.Copy(this.parentEntity, true);

            if (copy != null)
            {
                Response.Redirect(copy.GetFormUrl(), false);
            }
        }

        private void EditItem(Guid guid)
        {
            var ef = new EntityFactory(RegistryContext);
            var item = ef.LoadEntity(guid);

            Response.Redirect(item.GetFormUrl(), false);
        }

        private void DeleteItem(Guid[] guids)
        {
            Response.Redirect(Jhu.Graywulf.Web.Admin.Common.Delete.GetUrl(guids), false);
        }

        private void MoveItem(Guid guid, EntityMoveDirection dir)
        {
            var ef = new EntityFactory(RegistryContext);
            var item = ef.LoadEntity(guid);

            item.Move(dir);
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            Guid guid = Guid.Empty;
            Guid[] guids = null;

            if (InternalGridView.SelectedDataKeys.Count > 0)
            {
                guid = new Guid(InternalGridView.SelectedDataKeys.FirstOrDefault());
                guids = InternalGridView.SelectedDataKeys.Select(g => new Guid(g)).ToArray();
            }

            Page.Validate("EntityList");

            if (Page.IsValid)
            {
                switch (e.CommandName)
                {
                    case "Copy":
                        CopyItem(guid);
                        break;
                    case "Edit":
                        EditItem(new Guid(InternalGridView.SelectedDataKeys.First()));
                        break;
                    case "Delete":
                        DeleteItem(guids);
                        break;
                    case "MoveDown":
                        MoveItem(guid, EntityMoveDirection.Down);
                        break;
                    case "MoveUp":
                        MoveItem(guid, EntityMoveDirection.Up);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected void ItemSelectedValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = InternalGridView.SelectedDataKeys.Count > 0;
        }


    }
}