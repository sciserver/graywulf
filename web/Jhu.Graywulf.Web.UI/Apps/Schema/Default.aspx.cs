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
            Objects,
            Summary,
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

        private SchemaView selectedView;

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
                DatasetBase dataset = null;

                if (!String.IsNullOrWhiteSpace(datasetList.SelectedValue))
                {
                    try
                    {
                        dataset = FederationContext.SchemaManager.Datasets[datasetList.SelectedValue];
                    }
                    catch
                    {
                    }
                }

                if (dataset == null && SelectedDatabaseObject != null)
                {
                    dataset = SelectedDatabaseObject.Dataset;
                }

                return dataset;
            }
        }

        protected DatabaseObject SelectedDatabaseObject
        {
            get
            {
                DatabaseObject dbobj = null;

                if (!String.IsNullOrWhiteSpace(databaseObjectList.SelectedValue))
                {
                    try
                    {
                        dbobj = FederationContext.SchemaManager.GetDatabaseObjectByKey(databaseObjectList.SelectedValue);
                    }
                    catch
                    {

                    }
                }

                return dbobj;
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
        }

        protected void ViewButton_Command(object sender, CommandEventArgs e)
        {
            SchemaView view;

            if (Enum.TryParse<SchemaView>(e.CommandName, out view))
            {
                SelectedView = view;
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
            var dbobj = SelectedDatabaseObject;

            HideAllViews();

            if (dbobj == null)
            {
                summaryButton.Visible = false;
                columnsButton.Visible = false;
                indexesButton.Visible = false;
                parametersButton.Visible = false;
                peekButton.Visible = false;
            }
            else
            {
                summaryButton.Visible = true;
                columnsButton.Visible = (dbobj is IColumns);
                indexesButton.Visible = (dbobj is IIndexes);
                parametersButton.Visible = (dbobj is IParameters);

                peekButton.Visible = (dbobj is TableOrView);
                peekButton.NavigateUrl = Peek.GetUrl(dbobj.UniqueKey);

                switch (SelectedView)
                {
                    case SchemaView.Columns:
                        if (dbobj is IColumns)
                        {
                            ShowColumns();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case SchemaView.Indexes:
                        if (dbobj is IIndexes)
                        {
                            ShowIndexes();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case SchemaView.Parameters:
                        if (dbobj is IParameters)
                        {
                            ShowParameters();
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    case SchemaView.Summary:
                    default:
                        ShowSummary();
                        break;
                }
            }

            SessionView = SelectedView;
            SessionDataset = SelectedDataset?.Name;
            SessionDatabaseObject = SelectedDatabaseObject?.UniqueKey;
            OverrideUrl(Page.ResolveUrl(GetUrl(SelectedView, SelectedDataset, SelectedDatabaseObject)));
        }

        private void HideAllViews()
        {
            introForm.Visible = false;
            summaryForm.Visible = false;
            columnList.Visible = false;
            indexList.Visible = false;
            parameterList.Visible = false;

            summaryButton.CssClass = "";
            columnsButton.CssClass = "";
            indexesButton.CssClass = "";
            parametersButton.CssClass = "";
        }

        private void ShowSummary()
        {
            summaryForm.DatabaseObject = SelectedDatabaseObject;
            summaryForm.Visible = true;
            summaryButton.CssClass = "selected";
        }

        private void ShowColumns()
        {
            columnList.DatabaseObject = (IColumns)SelectedDatabaseObject;
            columnList.Visible = true;
            columnsButton.CssClass = "selected";
        }

        private void ShowIndexes()
        {
            indexList.DatabaseObject = (IIndexes)SelectedDatabaseObject;
            indexList.Visible = true;
            indexesButton.CssClass = "selected";
        }

        private void ShowParameters()
        {
            parameterList.DatabaseObject = (IParameters)SelectedDatabaseObject;
            parameterList.Visible = true;
            parametersButton.CssClass = "selected";
        }
    }
}