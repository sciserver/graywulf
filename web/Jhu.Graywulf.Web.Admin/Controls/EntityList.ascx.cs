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
        private EntitySearch search;
        private Entity parentEntity;
        private EntityType childrenType;
        private EntityGroup entityGroup;
        private string text;
        private DataControlFieldCollection columns;

        public bool CreateVisible
        {
            get { return create.Visible; }
            set { create.Visible = value; }
        }

        public bool CopyVisible
        {
            get { return copy.Visible; }
            set { copy.Visible = value; }
        }
        
        public bool EditVisible
        {
            get { return edit.Visible; }
            set { edit.Visible = value; }
        }

        public bool DeleteVisible
        {
            get { return delete.Visible; }
            set { delete.Visible = value; }
        }

        public bool ExportVisible
        {
            get { return export.Visible; }
            set { export.Visible = value; }
        }

        public bool MoveVisible
        {
            get { return moveGroup.Visible; }
            set { moveGroup.Visible = value; }
        }

        public EntitySearch Search
        {
            get { return search; }
            set { search = value; }
        }

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

        public bool ButtonsVisible
        {
            get { return buttonsDiv.Visible; }
            set { buttonsDiv.Visible = value; }
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
            this.search = null;
            this.parentEntity = null;
            this.childrenType = EntityType.Unknown;
            this.entityGroup = Registry.EntityGroup.Unknown;
            this.text = null;
            this.columns = new DataControlFieldCollection();
        }

        protected void InternalDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            if (search != null)
            {
                e.ObjectInstance = search;
            }
            else if (parentEntity != null)
            {
                var s = new EntitySearch(((PageBase)Page).RegistryContext);
                s.Parent = parentEntity;
                s.EntityType = childrenType;
                e.ObjectInstance = s;
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
                if (create.Visible && parentEntity != null)
                {
                    create.OnClientClick = Util.UrlFormatter.GetClientRedirect(parentEntity.GetNewChildFormUrl(childrenType));
                }
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

        private void DeleteItems(Guid[] guids)
        {
            Response.Redirect(Jhu.Graywulf.Web.Admin.Common.Delete.GetUrl(guids), false);
        }

        private void MoveItem(Guid guid, EntityMoveDirection dir)
        {
            var ef = new EntityFactory(RegistryContext);
            var item = ef.LoadEntity(guid);

            item.Move(dir);
        }

        private void ExportItems(Guid[] guids)
        {
            var key = ((PageBase)Page).FormCacheSave(guids);
            Response.Redirect(Web.Admin.Common.Export.GetUrl(key, EntityGroup));
        }

        private void DiscoverItems(Guid[] guids)
        {
            var key = ((PageBase)Page).FormCacheSave(guids);
            Response.Redirect(Web.Admin.Common.Discover.GetUrl(key));
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
                        DeleteItems(guids);
                        break;
                    case "MoveDown":
                        MoveItem(guid, EntityMoveDirection.Down);
                        break;
                    case "MoveUp":
                        MoveItem(guid, EntityMoveDirection.Up);
                        break;
                    case "Export":
                        ExportItems(guids);
                        break;
                    case "Deploy":
                    case "Undeploy":
                    case "Allocate":
                    case "Detach":
                    case "Attach":
                    case "Drop":
                        break;
                    case "Discover":
                        DiscoverItems(guids);
                        break;
                    case "Start":
                    case "Stop":
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