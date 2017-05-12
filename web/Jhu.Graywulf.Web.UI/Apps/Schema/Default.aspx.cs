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
            DatabaseObjects,
            Columns,
            Parameters,
            Indexes
        }

        public static string GetUrl()
        {
            return "~/Apps/Schema/Default.aspx";
        }

        public string GetUrl(DatasetBase dataset)
        {
            return GetUrl(SchemaView.Dataset, dataset.Name, null, null);
        }

        public static string GetUrl(string objid)
        {
            return GetUrl(SchemaView.Default, null, null, objid);
        }

        public static string GetUrl(SchemaView view, DatasetBase dataset, DatabaseObjectType objtype, DatabaseObject dbobj)
        {
            return GetUrl(view,
                dataset == null ? null : dataset.Name,
                objtype == DatabaseObjectType.Unknown ? null : objtype.ToString(),
                dbobj == null ? null : dbobj.UniqueKey);
        }

        private static string GetUrl(SchemaView view, string dataset, string objtype, string objid)
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

            if (!String.IsNullOrWhiteSpace(objtype))
            {
                pars += String.Format("&ot={0}", objtype);
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

        #region Private member variables

        private SchemaView selectedView;
        private DatasetBase selectedDataset;
        private DatabaseObjectType selectedObjectType;
        private DatabaseObject selectedDatabaseObject;

        #endregion
        #region Properties

        public SchemaView SessionView
        {
            get { return (SchemaView)(Session["Jhu.Graywulf.Web.UI.App.Schema.View"] ?? SchemaView.Default); }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.View"] = value.ToString(); }
        }

        public DatasetBase SessionDataset
        {
            get { return ParseDataset((string)Session["Jhu.Graywulf.Web.UI.App.Schema.Dataset"]); }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.Dataset"] = value?.Name; }
        }

        public DatabaseObjectType SessionObjectType
        {
            get { return ParseObjectType((string)Session["Jhu.Graywulf.Web.UI.App.Schema.ObjectType"]); }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.ObjectType"] = value.ToString(); }
        }

        public DatabaseObject SessionDatabaseObject
        {
            get { return ParseDatabaseObject((string)Session["Jhu.Graywulf.Web.UI.App.Schema.DatabaseObject"]); }
            set { Session["Jhu.Graywulf.Web.UI.App.Schema.DatabaseObject"] = value?.UniqueKey; }
        }

        public SchemaView RequestView
        {
            get { return ParseSchemaView(Request["view"]); }
        }

        public DatasetBase RequestDataset
        {
            get { return ParseDataset(Request.QueryString["ds"]); }
        }

        public DatabaseObjectType RequestObjectType
        {
            get { return ParseObjectType(Request.QueryString["ot"]); }
        }

        public DatabaseObject RequestDatabaseObject
        {
            get { return ParseDatabaseObject(Request.QueryString["objid"]); }
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
                else
                {
                    selectedDataset = ParseDataset(datasetList.SelectedValue);
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

        protected DatabaseObjectType SelectedObjectType
        {
            get
            {
                if (selectedObjectType != DatabaseObjectType.Unknown)
                {
                    return selectedObjectType;
                }
                else
                {
                    selectedObjectType = ParseObjectType(objectTypeList.SelectedValue);
                }

                if (selectedObjectType == DatabaseObjectType.Unknown && SelectedDatabaseObject != null)
                {
                    selectedObjectType = Jhu.Graywulf.Schema.Constants.SimpleDatabaseObjectTypes[SelectedDatabaseObject.ObjectType];
                }

                return selectedObjectType;
            }
            set
            {
                selectedObjectType = value;
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
                else
                {
                    selectedDatabaseObject = ParseDatabaseObject(databaseObjectList.SelectedValue);
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

                selectedDataset = RequestDataset ?? SessionDataset;

                selectedObjectType =
                    RequestObjectType != DatabaseObjectType.Unknown ? RequestObjectType :
                    SessionObjectType != DatabaseObjectType.Unknown ? SessionObjectType :
                    DatabaseObjectType.Unknown;

                selectedDatabaseObject = RequestDatabaseObject ?? SessionDatabaseObject;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RefreshDatasetList();
            RefreshObjectTypeList();
            RefreshDatabaseObjectList();

            UpdateForm();
        }

        protected void DatasetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dataset = datasetList.SelectedValue;

            if (!String.IsNullOrWhiteSpace(dataset))
            {
                ShowDatasetView(ParseDataset(dataset));
            }
            else
            {
                ShowDatasetsView();
            }
        }

        protected void ObjectTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objectType = objectTypeList.SelectedValue;

            if (!String.IsNullOrWhiteSpace(objectType))
            {
                ShowDatabaseObjectsView(SelectedDataset, ParseObjectType(objectType));
            }
            else if (SelectedDataset != null)
            {
                ShowDatasetView(SelectedDataset);
            }
            else
            {
                ShowDatasetsView();
            }
        }

        protected void DatabaseObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objid = databaseObjectList.SelectedValue;

            if (!String.IsNullOrWhiteSpace(objid))
            {
                ShowDatabaseObjectView(ParseDatabaseObject(objid));
            }
            else if (SelectedDataset != null && SelectedObjectType != DatabaseObjectType.Unknown)
            {
                ShowDatabaseObjectsView(SelectedDataset, SelectedObjectType);
            }
            else if (SelectedDataset != null)
            {
                ShowDatasetView(SelectedDataset);
            }
            else
            {
                ShowDatasetsView();
            }
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
                        ShowDatasetView(ParseDataset((string)e.CommandArgument));
                        break;
                    case SchemaView.DatabaseObject:
                        ShowDatabaseObjectView(ParseDatabaseObject((string)e.CommandArgument));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        private SchemaView ParseSchemaView(string view)
        {
            SchemaView v = SchemaView.Default;
            Enum.TryParse<SchemaView>(view, out v);
            return v;
        }

        private DatasetBase ParseDataset(string dataset)
        {
            if (!String.IsNullOrWhiteSpace(dataset))
            {
                try
                {
                    return FederationContext.SchemaManager.Datasets[dataset];
                }
                catch { }
            }

            return null;
        }

        private DatabaseObjectType ParseObjectType(string objectType)
        {
            var ot = DatabaseObjectType.Unknown;

            if (!String.IsNullOrWhiteSpace(objectType))
            {
                Enum.TryParse<DatabaseObjectType>(objectType, out ot);
            }

            return ot;
        }

        private DatabaseObject ParseDatabaseObject(string objid)
        {
            if (!String.IsNullOrWhiteSpace(objid))
            {
                try
                {
                    return FederationContext.SchemaManager.GetDatabaseObjectByKey(objid);
                }
                catch { }
            }

            return null;
        }

        private void RefreshDatasetList()
        {
            datasetList.Items.Clear();

            if (SelectedDataset != null)
            {
                datasetList.Items.Add(new ListItem("(show overview)", ""));
            }
            else
            {
                datasetList.Items.Add(new ListItem("(select data set)", ""));
            }

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
        }

        private void RefreshObjectTypeList()
        {
            objectTypeList.Items.Clear();

            if (SelectedDataset != null)
            {
                if (SelectedObjectType != DatabaseObjectType.Unknown)
                {
                    objectTypeList.Items.Add(new ListItem("(show overview)", ""));
                }
                else
                {
                    objectTypeList.Items.Add(new ListItem("(select object type)", ""));
                }

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
            }
        }

        private void RefreshDatabaseObjectList()
        {
            databaseObjectList.Items.Clear();

            if (SelectedDataset != null && SelectedObjectType != DatabaseObjectType.Unknown)
            {
                if (SelectedDatabaseObject != null)
                {
                    databaseObjectList.Items.Add(new ListItem("(show overview)", ""));
                }
                else
                {
                    databaseObjectList.Items.Add(new ListItem("(select database object)", ""));
                }

                try
                {
                    SelectedDataset.LoadAllObjects(SelectedObjectType, SelectedDataset.IsMutable);

                    foreach (var d in SelectedDataset.GetAllObjects(SelectedObjectType).OrderBy(f => f.DisplayName))
                    {
                        var li = new ListItem(d.DisplayName, d.UniqueKey);
                        databaseObjectList.Items.Add(li);
                    }

                    if (databaseObjectList.Items.Count == 1)
                    {
                        databaseObjectList.Items.Clear();
                        var li = new ListItem("(no items)", "error");
                        databaseObjectList.Items.Add(li);
                    }
                }
                catch (Exception)
                {
                    databaseObjectList.Items.Clear();
                    var li = new ListItem("(not available)", "error");
                    databaseObjectList.Items.Add(li);
                }
            }
        }

        private void UpdateForm()
        {
            var ds = SelectedDataset;
            var ot = SelectedObjectType;
            var dbobj = SelectedDatabaseObject;

            HideAllViews();

            if (ds == null)
            {

            }
            else if (dbobj == null)
            {
                objectTypeList.Visible = true;
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
                    datasetsView.Items = FederationContext.SchemaManager.EnumerateDatasets(true, true);
                    datasetsView.Visible = true;
                    break;
                case SchemaView.Dataset:
                    datasetView.Item = SelectedDataset;
                    datasetView.Visible = true;
                    break;
                case SchemaView.DatabaseObjects:
                    SelectedDataset.LoadAllObjects(SelectedObjectType, SelectedDataset.IsMutable);
                    databaseObjectsView.Items = SelectedDataset.GetAllObjects(SelectedObjectType);
                    databaseObjectsView.Visible = true;
                    break;
                case SchemaView.DatabaseObject:
                    databaseObjectView.Item = SelectedDatabaseObject;
                    databaseObjectView.Visible = true;
                    summaryButton.CssClass = "selected";
                    break;
                case SchemaView.Columns:
                    columnsView.Item = (IColumns)SelectedDatabaseObject;
                    columnsView.Visible = true;
                    columnsButton.CssClass = "selected";
                    break;
                case SchemaView.Indexes:
                    indexesView.Item = (IIndexes)SelectedDatabaseObject;
                    indexesView.Visible = true;
                    indexesButton.CssClass = "selected";
                    break;
                case SchemaView.Parameters:
                    parametersView.Item = (IParameters)SelectedDatabaseObject;
                    parametersView.Visible = true;
                    parametersButton.CssClass = "selected";
                    break;
                default:
                    throw new NotImplementedException();
            }

            datasetList.SelectedValue = SelectedDataset?.Name;
            objectTypeList.SelectedValue = SelectedObjectType == DatabaseObjectType.Unknown ? null : SelectedObjectType.ToString();
            databaseObjectList.SelectedValue = SelectedDatabaseObject?.UniqueKey;

            SessionView = SelectedView;
            SessionDataset = SelectedDataset;
            SessionObjectType = SelectedObjectType;
            SessionDatabaseObject = SelectedDatabaseObject;

            OverrideUrl(Page.ResolveUrl(GetUrl(SelectedView, SelectedDataset, SelectedObjectType, SelectedDatabaseObject)));
        }

        private void HideAllViews()
        {
            introForm.Visible = false;
            datasetsView.Visible = false;
            datasetView.Visible = false;
            databaseObjectsView.Visible = false;
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

        private void ShowDatasetsView()
        {
            SelectedDataset = null;
            SelectedDatabaseObject = null;
            SelectedView = SchemaView.Datasets;
        }

        private void ShowDatasetView(DatasetBase ds)
        {
            SelectedDataset = ds;
            SelectedView = SchemaView.Dataset;
        }

        private void ShowDatabaseObjectsView(DatasetBase ds, DatabaseObjectType objectType)
        {
            SelectedDataset = ds;
            SelectedObjectType = objectType;
            SelectedView = SchemaView.DatabaseObjects;
        }

        private void ShowDatabaseObjectView(DatabaseObject dbobj)
        {
            SelectedDataset = dbobj.Dataset;
            SelectedObjectType = Jhu.Graywulf.Schema.Constants.SimpleDatabaseObjectTypes[dbobj.ObjectType];
            SelectedDatabaseObject = dbobj;
            SelectedView = SchemaView.DatabaseObject;   // TODO
        }
    }
}