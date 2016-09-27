using System;
using System.Text.RegularExpressions;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DbObjectDetails : FederationUserControlBase
    {
        private string databaseObjectID;

        public string DatabaseObjectID
        {
            get { return databaseObjectID; }
            set
            {
                databaseObjectID = value;
                RefreshForm();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }


        protected override object SaveControlState()
        {
            object[] state = new object[] { base.SaveControlState(), databaseObjectID };

            return state;
        }

        protected override void LoadControlState(object savedState)
        {
            object[] state = (object[])savedState;

            databaseObjectID = (string)state[1];

            base.LoadControlState(state[0]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        protected void RefreshForm()
        {
            if (databaseObjectID != null)
            {
                DatabaseObject dbobj = Page.FederationContext.SchemaManager.GetDatabaseObjectByKey(databaseObjectID);

                FullyQualifiedNameLabel.Text = Jhu.Graywulf.Schema.Constants.DatabaseObjectsName_Singular[dbobj.ObjectType];

                DatasetNameLabel.Text = dbobj.DatasetName;
                SchemaNameLabel.Text = dbobj.SchemaName;
                DbObjectNameLabel.Text = dbobj.ObjectName;

                SummaryLabel.Text = dbobj.Metadata.Summary;
                RemarksLabel.Text = dbobj.Metadata.Remarks;
                RemarksPanel.Visible = !String.IsNullOrEmpty(dbobj.Metadata.Remarks);
                ExampleLabel.Text = Util.DocumentationFormatter.FormatExample(dbobj.Metadata.Example);
                ExamplePanel.Visible = !String.IsNullOrEmpty(dbobj.Metadata.Example);

                if (dbobj is IColumns)
                {
                    columnsList.DatabaseObject = (IColumns)dbobj;
                    columnsTab.Hidden = false;
                }
                else
                {
                    columnsTab.Hidden = true;
                }

                if (dbobj is IIndexes)
                {
                    indexesList.DatabaseObject = (IIndexes)dbobj;
                    indexesTab.Hidden = false;
                }
                else
                {
                    indexesTab.Hidden = true;
                }

                if (dbobj is IParameters)
                {
                    parametersList.DatabaseObject = (IParameters)dbobj;
                    parametersTab.Hidden = false;
                }
                else
                {
                    parametersTab.Hidden = true;
                }

                if (((TabView)multiView.Views[multiView.ActiveViewIndex]).Hidden)
                {
                    multiView.ActiveViewIndex = 0;  // switch back to details
                }

                // Show or hide buttons that are applicable to mydb objects only

                var mydbbuttonsenabled = GraywulfSchemaManager.Comparer.Compare(dbobj.DatasetName, Page.RegistryContext.Federation.UserDatabaseVersion.Name) == 0;
                Export.Enabled = Rename.Enabled = Drop.Enabled = mydbbuttonsenabled;
                // TODO: funtions etc cannot be exported or peeked into!

                if (dbobj.ObjectType == DatabaseObjectType.Table || dbobj.ObjectType == DatabaseObjectType.View)
                {
                    Peek.OnClientClick = Util.UrlFormatter.GetClientPopUp(Jhu.Graywulf.Web.UI.Apps.Schema.Peek.GetUrl(dbobj.UniqueKey));
                    Peek.Enabled = true;
                }
                else
                {
                    Peek.Enabled = false;
                }
            }
        }

        protected void Export_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("~/mydb/ExportTable.aspx?objid={0}", databaseObjectID), false);
        }

        protected void Rename_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("~/mydb/RenameObject.aspx?objid={0}", databaseObjectID), false);
        }

        protected void Drop_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("~/mydb/DropObject.aspx?objid={0}", databaseObjectID), false);
        }
    }
}