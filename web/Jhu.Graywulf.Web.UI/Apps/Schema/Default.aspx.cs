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
        public enum SchemaView
        {
            Default,
            Datasets,
            Dataset,
            DatabaseObject,
            Columns,
            Parameters,
            Indexes
        }

        public static string GetUrl()
        {
            return "~/Apps/Schema/Default.aspx";
        }

        public static string GetUrl(string objid)
        {
            return GetUrl(SchemaView.Default, null, objid);
        }

        public static string GetUrl(SchemaView view, DatasetBase dataset, DatabaseObject dbobj)
        {
            return GetUrl(view,
                dataset == null ? null : dataset.Name,
                dbobj == null ? null : dbobj.UniqueKey);
        }

        public static string GetUrl(SchemaView view, string dataset, string objid)
        {
            var pars = "";

            if (view != SchemaView.Default)
            {
                pars += String.Format("&view={0}", view);
            }

            if (!String.IsNullOrWhiteSpace(dataset))
            {
                pars += String.Format("&ds={0}", dataset);
            }

            if (!String.IsNullOrWhiteSpace(objid))
            {
                pars += String.Format("&objid={0}", objid);
            }

            if (String.IsNullOrEmpty(pars))
            {
                return GetUrl();
            }
            else
            {
                return GetUrl() + "?" + pars.Substring(1);
            }
        }

        public string GetUrl(DatasetBase dataset)
        {
            return GetUrl(SchemaView.Dataset, dataset.Name, null);
        }

        #region Private member variables

        private SchemaView selectedView;
        private DatasetBase selectedDataset;
        private DatabaseObject selectedDatabaseObject;

        #endregion
        #region Properties

        public SchemaView SessionView
        {
            get { return (SchemaView)(Session["Jhu.Graywulf.Web.UI.App.Schema.View"] ?? SchemaView.Default); }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.View"] = value; }
        }

        public string SessionDataset
        {
            get { return (string)Session["Jhu.Graywulf.Web.UI.App.Schema.Dataset"]; }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.Dataset"] = value; }
        }

        public string SessionDatabaseObject
        {
            get { return (string)Session["Jhu.Graywulf.Web.UI.App.Schema.DatabaseObject"]; }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.DatabaseObject"] = value; }
        }

        public SchemaView RequestView
        {
            get
            {
                SchemaView view = SchemaView.Default;
                Enum.TryParse<SchemaView>(Request.QueryString["view"], out view);
                return view;
            }
        }

        public string RequestDataset
        {
            get { return Request.QueryString["ds"]; }
        }

        public string RequestDatabaseObject
        {
            get { return Request.QueryString["objid"]; }
        }

        protected SchemaView SelectedView
        {
            get { return selectedView; }
            set { selectedView = value; }
        }

        protected DatasetBase SelectedDataset
        {
            get
            {
                if (selectedDataset != null)
                {
                    return selectedDataset;
                }
                else if (!String.IsNullOrWhiteSpace(datasetList.SelectedValue))
                {
                    try
                    {
                        selectedDataset = FederationContext.SchemaManager.Datasets[datasetList.SelectedValue];
                    }
                    catch { }
                }

                if (selectedDataset == null && SelectedDatabaseObject != null)
                {
                    selectedDataset = SelectedDatabaseObject.Dataset;
                }

                return selectedDataset;
            }
            set
            {
                selectedDataset = value;
            }
        }

        protected DatabaseObject SelectedDatabaseObject
        {
            get
            {
                if (selectedDatabaseObject != null)
                {
                    return selectedDatabaseObject;
                }
                else if (!String.IsNullOrWhiteSpace(databaseObjectList.SelectedValue))
                {
                    try
                    {
                        selectedDatabaseObject = FederationContext.SchemaManager.GetDatabaseObjectByKey(databaseObjectList.SelectedValue);
                    }
                    catch { }
                }

                return selectedDatabaseObject;
            }
            set
            {
                selectedDatabaseObject = value;
            }
        }

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                selectedView =
                    RequestView != SchemaView.Default ? RequestView :
                    SessionView != SchemaView.Default ? SessionView :
                    SchemaView.Default;

                RefreshDatasetList();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
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

        protected void DatabaseObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing here, pre_render event will show requested view
        }

        protected void ViewButton_Command(object sender, CommandEventArgs e)
        {
            SchemaView view;

            if (Enum.TryParse<SchemaView>(e.CommandName, out view))
            {
                SelectedView = view;
            }
        }

        protected void SchemaView_Command(object sender, CommandEventArgs e)
        {
            SchemaView view;

            if (Enum.TryParse<SchemaView>(e.CommandName, true, out view))
            {
                switch (view)
                {
                    case SchemaView.Dataset:
                        ShowDataset(FederationContext.SchemaManager.Datasets[(string)e.CommandArgument]);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        private void RefreshDatasetList()
        {
            datasetList.Items.Clear();
            datasetList.Items.Add(new ListItem("(select data set)", ""));

            var dss = FederationContext.SchemaManager.EnumerateDatasets(true, true);

            foreach (var ds in dss)
            {
                var li = new ListItem(ds.Name, ds.Name);

                if (ds.IsInError)
                {
                    li.Text += " (not available)";
                }

                if (ds.IsMutable)
                {
                    li.Attributes.Add("class", "ToolbarControlHighlight");
                }

                datasetList.Items.Add(li);
            }

            if (!IsPostBack)
            {
                datasetList.SelectedValue = RequestDataset ?? SessionDataset ?? "";
                RefreshObjectTypeList();
            }
        }

        private void RefreshObjectTypeList()
        {
            objectTypeList.Items.Clear();

            if (SelectedDataset != null)
            {
                objectTypeList.Items.Add(new ListItem("Tables", DatabaseObjectType.Table.ToString()));
                objectTypeList.Items.Add(new ListItem("Views", DatabaseObjectType.View.ToString()));

                if (SchemaManager.Comparer.Compare(datasetList.SelectedValue, Registry.Constants.CodeDbName) == 0)
                {
                    objectTypeList.Items.Add(new ListItem("User-defined Types", DatabaseObjectType.DataType.ToString()));
                    objectTypeList.Items.Add(new ListItem("Stored Procedures", DatabaseObjectType.StoredProcedure.ToString()));
                    objectTypeList.Items.Add(new ListItem("Scalar Functions", DatabaseObjectType.ScalarFunction.ToString()));
                    objectTypeList.Items.Add(new ListItem("Table-valued Functions", DatabaseObjectType.TableValuedFunction.ToString()));
                }
            }

            if (!IsPostBack)
            {
                var dbobjid = RequestDatabaseObject ?? SessionDatabaseObject;

                if (!String.IsNullOrWhiteSpace(dbobjid))
                {
                    var dbobjtype = SchemaManager.GetDatabaseObjectTypeFromKey(dbobjid);
                    dbobjtype = Jhu.Graywulf.Schema.Constants.SimpleDatabaseObjectTypes[dbobjtype];
                    objectTypeList.SelectedValue = dbobjtype.ToString();
                    RefreshObjectList();
                }
            }
        }

        private void RefreshObjectList()
        {
            databaseObjectList.Items.Clear();

            try
            {
                var dataset = FederationContext.SchemaManager.Datasets[datasetList.SelectedValue];

                DatabaseObjectType type;
                if (Enum.TryParse<DatabaseObjectType>(objectTypeList.SelectedValue, out type))
                {
                    var li = new ListItem("(select item)", "");
                    databaseObjectList.Items.Add(li);

                    switch (type)
                    {
                        case DatabaseObjectType.DataType:
                            dataset.UserDefinedTypes.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.UserDefinedTypes.Values);
                            break;
                        case DatabaseObjectType.Table:
                            dataset.Tables.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.Tables.Values);
                            break;
                        case DatabaseObjectType.View:
                            dataset.Views.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.Views.Values);
                            break;
                        case DatabaseObjectType.TableValuedFunction:
                            dataset.TableValuedFunctions.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.TableValuedFunctions.Values);
                            break;
                        case DatabaseObjectType.ScalarFunction:
                            dataset.ScalarFunctions.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.ScalarFunctions.Values);
                            break;
                        case DatabaseObjectType.StoredProcedure:
                            dataset.StoredProcedures.LoadAll(dataset.IsMutable);
                            LoadDatabaseObjects(dataset.StoredProcedures.Values);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    var li = new ListItem("(no items)", "");
                    databaseObjectList.Items.Add(li);
                }
            }
            catch (Exception ex)
            {
                databaseObjectList.Items.Clear();
                var li = new ListItem("(not available)", "");
                databaseObjectList.Items.Add(li);
            }

            if (!IsPostBack)
            {
                databaseObjectList.SelectedValue = RequestDatabaseObject ?? SessionDatabaseObject;
            }
        }

        protected void LoadDatabaseObjects(IEnumerable<DatabaseObject> objects)
        {
            foreach (var d in objects.OrderBy(f => f.DisplayName))
            {
                var li = new ListItem(d.DisplayName, d.UniqueKey);
                databaseObjectList.Items.Add(li);
            }

            if (databaseObjectList.Items.Count == 1)
            {
                databaseObjectList.Items.Clear();
                var li = new ListItem("(no items)", "");
                databaseObjectList.Items.Add(li);
            }
        }

        private void UpdateForm()
        {
            var ds = SelectedDataset;
            var dbobj = SelectedDatabaseObject;

            HideAllViews();

            if (ds == null)
            {

            }
            else if (dbobj == null)
            {
            }
            else
            {
                summaryButton.Visible = true;
                columnsButton.Visible = (dbobj is IColumns);
                indexesButton.Visible = (dbobj is IIndexes);
                parametersButton.Visible = (dbobj is IParameters);

                peekButton.Visible = (dbobj is TableOrView);
                peekButton.NavigateUrl = Peek.GetUrl(dbobj.UniqueKey);
            }

            switch (SelectedView)
            {
                case SchemaView.Default:
                case SchemaView.Datasets:
                    datasetsView.Datasets = FederationContext.SchemaManager.EnumerateDatasets(true, true);
                    datasetsView.Visible = true;
                    break;
                case SchemaView.Dataset:
                    datasetView.Item = SelectedDataset;
                    datasetView.Visible = true;
                    break;
                case SchemaView.Columns:
                    if (dbobj is IColumns)
                    {
                        columnsView.Item = (IColumns)SelectedDatabaseObject;
                        columnsView.Visible = true;
                        columnsButton.CssClass = "selected";
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case SchemaView.Indexes:
                    if (dbobj is IIndexes)
                    {
                        indexesView.Item = (IIndexes)SelectedDatabaseObject;
                        indexesView.Visible = true;
                        indexesButton.CssClass = "selected";
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case SchemaView.Parameters:
                    if (dbobj is IParameters)
                    {
                        parametersView.Item = (IParameters)SelectedDatabaseObject;
                        parametersView.Visible = true;
                        parametersButton.CssClass = "selected";
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case SchemaView.DatabaseObject:
                default:
                    databaseObjectView.Item = SelectedDatabaseObject;
                    databaseObjectView.Visible = true;
                    //columnsButton.CssClass = "selected";
                    break;
            }

            SessionView = SelectedView;
            SessionDataset = SelectedDataset?.Name;
            SessionDatabaseObject = SelectedDatabaseObject?.UniqueKey;
            OverrideUrl(Page.ResolveUrl(GetUrl(SelectedView, SelectedDataset, SelectedDatabaseObject)));
        }

        private void HideAllViews()
        {
            introForm.Visible = false;
            datasetsView.Visible = false;
            datasetView.Visible = false;
            databaseObjectView.Visible = false;
            columnsView.Visible = false;
            parametersView.Visible = false;
            indexesView.Visible = false;

            summaryButton.Visible = false;
            columnsButton.Visible = false;
            indexesButton.Visible = false;
            parametersButton.Visible = false;
            peekButton.Visible = false;

            summaryButton.CssClass = "";
            columnsButton.CssClass = "";
            indexesButton.CssClass = "";
            parametersButton.CssClass = "";
        }

        private void ShowDatasets()
        {
        }

        private void ShowDataset(DatasetBase ds)
        {
            SelectedDataset = ds;
            SelectedView = SchemaView.Dataset;
        }
    }
}