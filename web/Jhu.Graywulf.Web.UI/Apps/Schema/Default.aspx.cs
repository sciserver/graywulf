using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Default : FederationPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Schema/Default.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("{0}?objid={1}", GetUrl(), objid);
        }

        #region Properties

        protected string CurrentView
        {
            get { return (string)(ViewState["CurrentView"] ?? "summary"); }
            set { ViewState["CurrentView"] = value; }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DatabaseObject dbobj = null;

                var dbobjid = (string)Request.QueryString["objid"] ?? SelectedSchemaObject;
                if (dbobjid != null)
                {

                    try
                    {
                        dbobj = FederationContext.SchemaManager.GetDatabaseObjectByKey(dbobjid);
                    }
                    catch
                    {

                    }
                }

                if (dbobj != null)
                {
                    RefreshDatasetList();
                    DatasetList.SelectedValue = dbobj.DatasetName;

                    RefreshObjectTypeList();
                    ObjectTypeList.SelectedValue = Jhu.Graywulf.Schema.Constants.SimpleDatabaseObjectTypes[dbobj.ObjectType].ToString();

                    RefreshObjectList();
                    ObjectList.SelectedValue = dbobj.UniqueKey;
                }
                else
                {
                    RefreshDatasetList();
                    RefreshObjectTypeList();
                    RefreshObjectList();
                }
            }

            UpdateForm();
        }

        protected void DatasetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshObjectTypeList();
            RefreshObjectList();
        }

        protected void ObjectTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshObjectList();
        }

        protected void ObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        protected void ToolbarButton_Command(object sender, CommandEventArgs e)
        {
            CurrentView = e.CommandName;
            UpdateForm();
        }

        #endregion

        private void RefreshDatasetList()
        {
            DatasetList.Items.Clear();

            // Add MyDB etc. to the beginning of the list
            if (FederationContext.RegistryUser != null)
            {
                var uf = UserDatabaseFactory.Create(RegistryContext.Federation);
                var mydbds = uf.GetUserDatabases(RegistryUser);

                foreach (var key in mydbds.Keys)
                {
                    var mydbli = CreateDatasetListItem(mydbds[key]);
                    mydbli.Attributes.Add("class", "ToolbarControlHighlight");
                    DatasetList.Items.Add(mydbli);
                }
            }

            // Code is the next
            var codedbli = new ListItem(Registry.Constants.CodeDbName, Registry.Constants.CodeDbName);
            codedbli.Attributes.Add("class", "ToolbarControlHighlight");
            DatasetList.Items.Add(codedbli);

            // Add other registered catalogs     
            FederationContext.SchemaManager.Datasets.LoadAll(false);

            // TODO: this needs to be modified here, use flags instead of filtering on name!
            foreach (var dsd in FederationContext.SchemaManager.Datasets.Values.Where(k =>
                k.Name != Graywulf.Registry.Constants.UserDbName &&
                k.Name != Graywulf.Registry.Constants.CodeDbName &&
                k.Name != Graywulf.Registry.Constants.TempDbName).OrderBy(k => k.Name))
            {
                var li = CreateDatasetListItem(dsd);
                DatasetList.Items.Add(li);
            }
        }

        private ListItem CreateDatasetListItem(DatasetBase ds)
        {
            var li = new ListItem(ds.Name, ds.Name);

            if (ds.IsInError)
            {
                li.Text += " (not available)";
            }

            return li;
        }

        private void RefreshObjectTypeList()
        {
            ObjectTypeList.Items.Clear();

            ObjectTypeList.Items.Add(new ListItem("Tables", "Table"));
            ObjectTypeList.Items.Add(new ListItem("Views", "View"));

            if (SchemaManager.Comparer.Compare(DatasetList.SelectedValue, Registry.Constants.CodeDbName) == 0)
            {
                ObjectTypeList.Items.Add(new ListItem("User-defined Types", "DataType"));
                ObjectTypeList.Items.Add(new ListItem("Stored Procedures", "StoredProcedure"));
                ObjectTypeList.Items.Add(new ListItem("Scalar Functions", "ScalarFunction"));
                ObjectTypeList.Items.Add(new ListItem("Table-valued Functions", "TableValuedFunction"));
            }
        }

        private void RefreshObjectList()
        {
                ObjectList.Items.Clear();

            try
            {
                var dataset = FederationContext.SchemaManager.Datasets[DatasetList.SelectedValue];

                DatabaseObjectType type;
                if (Enum.TryParse<DatabaseObjectType>(ObjectTypeList.SelectedValue, out type))
                {
                    var li = new ListItem("(select item)", "");
                    ObjectList.Items.Add(li);

                    switch (type)
                    {
                        case DatabaseObjectType.DataType:
                            LoadDataTypes(dataset);
                            break;
                        case DatabaseObjectType.Table:
                            LoadTables(dataset);
                            break;
                        case DatabaseObjectType.View:
                            LoadViews(dataset);
                            break;
                        case DatabaseObjectType.TableValuedFunction:
                            LoadTableValuedFunctions(dataset);
                            break;
                        case DatabaseObjectType.ScalarFunction:
                            LoadScalarFunctions(dataset);
                            break;
                        case DatabaseObjectType.StoredProcedure:
                            LoadStoredProcedures(dataset);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    var li = new ListItem("(no items)", "");
                    ObjectList.Items.Add(li);
                }
            }
            catch (Exception ex)
            {
                ObjectList.Items.Clear();
                var li = new ListItem("(not available)", "");
                ObjectList.Items.Add(li);
            }
        }

        protected void LoadDataTypes(DatasetBase dataset)
        {
            dataset.UserDefinedTypes.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.UserDefinedTypes.Values);
        }

        protected void LoadTables(DatasetBase dataset)
        {
            dataset.Tables.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.Tables.Values);
        }

        protected void LoadViews(DatasetBase dataset)
        {
            dataset.Views.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.Views.Values);
        }

        protected void LoadTableValuedFunctions(DatasetBase dataset)
        {
            dataset.TableValuedFunctions.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.TableValuedFunctions.Values);
        }

        protected void LoadScalarFunctions(DatasetBase dataset)
        {
            dataset.ScalarFunctions.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.ScalarFunctions.Values);
        }

        protected void LoadStoredProcedures(DatasetBase dataset)
        {
            dataset.StoredProcedures.LoadAll(dataset.IsMutable);
            LoadDatabaseObjects(dataset.StoredProcedures.Values);
        }

        protected void LoadDatabaseObjects(IEnumerable<DatabaseObject> objects)
        {
            foreach (var d in objects.OrderBy(f => f.DisplayName))
            {
                var li = new ListItem(d.DisplayName, d.UniqueKey);
                ObjectList.Items.Add(li);
            }

            if (ObjectList.Items.Count == 1)
            {
                ObjectList.Items.Clear();
                var li = new ListItem("(no items)", "");
                ObjectList.Items.Add(li);
            }
        }

        private DatabaseObject GetSelectedObject()
        {
            var dbobjid = ObjectList.SelectedValue;
            DatabaseObject dbobj = null;

            try
            {
                dbobj = FederationContext.SchemaManager.GetDatabaseObjectByKey(dbobjid);
            }
            catch
            {
            }

            if (dbobj != null)
            {
                // Display details
                SelectedSchemaObject = dbobjid;
            }

            return dbobj;
        }

        private void UpdateForm()
        {
            var dbobj = GetSelectedObject();

            HideAllViews();

            if (dbobj == null)
            {
                summary.Visible = false;
                columns.Visible = false;
                indexes.Visible = false;
                parameters.Visible = false;
                peek.Visible = false;
            }
            else
            {
                summary.Visible = true;
                columns.Visible = (dbobj is IColumns);
                indexes.Visible = (dbobj is IIndexes);
                parameters.Visible = (dbobj is IParameters);

                peek.Visible = (dbobj is TableOrView);
                peek.NavigateUrl = Peek.GetUrl(dbobj.UniqueKey);

                switch (CurrentView)
                {
                    case "columns":
                        if (dbobj is IColumns)
                        {
                            ShowColumns();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case "indexes":
                        if (dbobj is IIndexes)
                        {
                            ShowIndexes();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case "parameters":
                        if (dbobj is IParameters)
                        {
                            ShowParameters();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case "summary":
                    default:
                        ShowSummary();
                        break;
                }
            }
        }

        private void HideAllViews()
        {
            introForm.Visible = false;
            summaryForm.Visible = false;
            columnList.Visible = false;
            indexList.Visible = false;
            parameterList.Visible = false;

            summary.CssClass = "";
            columns.CssClass = "";
            indexes.CssClass = "";
            parameters.CssClass = "";
        }

        private void ShowSummary()
        {
            summaryForm.DatabaseObject = GetSelectedObject();
            summaryForm.Visible = true;
            summary.CssClass = "selected";
        }

        private void ShowColumns()
        {
            columnList.DatabaseObject = (IColumns)GetSelectedObject();
            columnList.Visible = true;
            columns.CssClass = "selected";
        }

        private void ShowIndexes()
        {
            indexList.DatabaseObject = (IIndexes)GetSelectedObject();
            indexList.Visible = true;
            indexes.CssClass = "selected";
        }

        private void ShowParameters()
        {
            parameterList.DatabaseObject = (IParameters)GetSelectedObject();
            parameterList.Visible = true;
            parameters.CssClass = "selected";
        }
    }
}