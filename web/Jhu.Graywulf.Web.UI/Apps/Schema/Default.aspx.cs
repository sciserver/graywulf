using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Default : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Schema/Default.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/Schema/Default.aspx?objid={0}", objid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load all datasets and refresh cache by getting all items in list
                FederationContext.SchemaManager.Datasets.LoadAll();

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

                    ShowDetails(dbobjid);
                }
                else
                {
                    RefreshDatasetList();
                    RefreshObjectTypeList();
                    RefreshObjectList();
                }
            }
        }

        private void RefreshDatasetList()
        {
            DatasetList.Items.Clear();

            // Add MyDB as the first item
            // TODO: extend this in case of multiple user databases
            if (FederationContext.RegistryUser != null)
            {
                var mydbli = new ListItem(FederationContext.MyDBDataset.Name, FederationContext.MyDBDataset.Name);
                mydbli.Attributes.Add("class", "ToolbarControlHighlight");
                DatasetList.Items.Add(mydbli);
            }

            // Code is the second

            var codedbli = new ListItem(Registry.Constants.CodeDbName, Registry.Constants.CodeDbName);
            codedbli.Attributes.Add("class", "ToolbarControlHighlight");
            DatasetList.Items.Add(codedbli);

            // Add other registered catalogs            
            // TODO: this needs to be modified here, use flags instead of filtering on name!
            FederationContext.SchemaManager.Datasets.LoadAll();
            foreach (var dsd in FederationContext.SchemaManager.Datasets.Values.Where(k =>
                k.Name != Graywulf.Registry.Constants.UserDbName &&
                k.Name != Graywulf.Registry.Constants.CodeDbName &&
                k.Name != Graywulf.Registry.Constants.TempDbName).OrderBy(k => k.Name))
            {
                DatasetList.Items.Add(dsd.Name);
            }
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
            try
            {
                ObjectList.Items.Clear();

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
            dataset.UserDefinedTypes.LoadAll();
            LoadDatabaseObjects(dataset.UserDefinedTypes.Values);
        }

        protected void LoadTables(DatasetBase dataset)
        {
            dataset.Tables.LoadAll();
            LoadDatabaseObjects(dataset.Tables.Values);
        }

        protected void LoadViews(DatasetBase dataset)
        {
            dataset.Views.LoadAll();
            LoadDatabaseObjects(dataset.Views.Values);
        }

        protected void LoadTableValuedFunctions(DatasetBase dataset)
        {

            dataset.TableValuedFunctions.LoadAll();
            LoadDatabaseObjects(dataset.TableValuedFunctions.Values);
        }

        protected void LoadScalarFunctions(DatasetBase dataset)
        {
            dataset.ScalarFunctions.LoadAll();
            LoadDatabaseObjects(dataset.ScalarFunctions.Values);
        }

        protected void LoadStoredProcedures(DatasetBase dataset)
        {
            dataset.StoredProcedures.LoadAll();
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

        private void HideAllPanels()
        {
            IntroForm.Visible = false;
            DetailsPanel.Visible = false;
        }

        private void ShowDetails(string dbobjid)
        {
            HideAllPanels();

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
                DetailsPanel.DatabaseObjectID = dbobjid;
                DetailsPanel.Visible = true;

                SelectedSchemaObject = dbobjid;
            }
        }

        //--

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
            ShowDetails(ObjectList.SelectedValue);
        }
    }
}