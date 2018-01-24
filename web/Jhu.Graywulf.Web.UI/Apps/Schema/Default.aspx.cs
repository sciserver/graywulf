using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Default : FederationPageBase
    {
        public enum SchemaView
        {
            Default,
            DatasetList,
            Dataset,
            DatabaseObject,
            DatabaseObjectList,
            Columns,
            Parameters,
            Indexes,
            Peek
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

        public static string GetUrl(SchemaView view, string objid)
        {
            return GetUrl(view, null, null, objid);
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

        private DatasetBase selectedDataset;
        private DatabaseObjectType selectedObjectType;
        private DatabaseObject selectedDatabaseObject;

        #endregion
        #region Properties

        public SchemaView SessionView
        {
            get { return ParseSchemaView((string)Session["Jhu.Graywulf.Web.UI.App.Schema.View"]); }
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
            get { return (SchemaView)(ViewState["SelectedView"] ?? SchemaView.Default); }
            set { ViewState["SelectedView"] = value; }
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
                datasetList.SelectedValue = value?.Name;
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
                    selectedObjectType = Jhu.Graywulf.Sql.Schema.Constants.SimpleDatabaseObjectTypes[SelectedDatabaseObject.ObjectType];
                }

                return selectedObjectType;
            }
            set
            {
                selectedObjectType = value;
                objectTypeList.SelectedValue = value == DatabaseObjectType.Unknown ? null : value.ToString();
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
                databaseObjectList.SelectedValue = value?.UniqueKey;
            }
        }

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectedView =
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
        }

        protected void ObjectTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objectType = objectTypeList.SelectedValue;

            if (!String.IsNullOrWhiteSpace(objectType))
            {
                ShowDatabaseObjectListView(SelectedDataset, ParseObjectType(objectType));
            }
            else if (SelectedDataset != null)
            {
                ShowDatasetView(SelectedDataset);
            }
            else
            {
                ShowDatasetListView();
            }
        }

        protected void DatabaseObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objid = databaseObjectList.SelectedValue;

            if (!String.IsNullOrWhiteSpace(objid))
            {
                var dbobj = ParseDatabaseObject(objid);
                ShowDatabaseObjectView(dbobj, SelectedView);
            }
            else if (SelectedDataset != null && SelectedObjectType != DatabaseObjectType.Unknown)
            {
                ShowDatabaseObjectListView(SelectedDataset, SelectedObjectType);
            }
            else if (SelectedDataset != null)
            {
                ShowDatasetView(SelectedDataset);
            }
            else
            {
                ShowDatasetListView();
            }
        }

        protected void ViewButton_Command(object sender, CommandEventArgs e)
        {
            SelectedView = ParseSchemaView(e.CommandName);
        }

        protected void SchemaView_Command(object sender, CommandEventArgs e)
        {
            SchemaView view;

            if (Enum.TryParse<SchemaView>(e.CommandName, true, out view))
            {
                switch (view)
                {
                    case SchemaView.DatasetList:
                        ShowDatasetListView();
                        break;
                    case SchemaView.Dataset:
                        if (String.IsNullOrWhiteSpace((string)e.CommandArgument))
                        {
                            ShowDatasetView(SelectedDataset);
                        }
                        else
                        {
                            ShowDatasetView(ParseDataset((string)e.CommandArgument));
                        }
                        break;
                    case SchemaView.DatabaseObjectList:
                        ShowDatabaseObjectListView(SelectedDataset, SelectedObjectType);
                        break;
                    case SchemaView.DatabaseObject:
                        ShowDatabaseObjectView(ParseDatabaseObject((string)e.CommandArgument), SelectedView);
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

            if (SelectedDataset == null)
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
                if (SelectedObjectType == DatabaseObjectType.Unknown)
                {
                    objectTypeList.Items.Add(new ListItem("(select object type)", ""));
                }

                if (SelectedDataset != null)
                {
                    objectTypeList.Items.Add(new ListItem("Tables", DatabaseObjectType.Table.ToString()));
                    objectTypeList.Items.Add(new ListItem("Views", DatabaseObjectType.View.ToString()));

                    if (SchemaManager.Comparer.Compare(SelectedDataset.Name, Registry.Constants.CodeDbName) == 0)
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

                    foreach (var d in SelectedDataset.GetAllObjects(SelectedObjectType).OrderBy(o => o, new DatabaseObjectLexicographicComparer()))
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

            switch (SelectedView)
            {
                case SchemaView.Default:
                case SchemaView.DatasetList:
                    datasetListView.Items = FederationContext.SchemaManager.EnumerateDatasets(true, true);
                    datasetListView.Visible = true;
                    datasetListButton.CssClass = "selected";
                    break;
                case SchemaView.Dataset:
                    datasetView.Item = SelectedDataset;
                    datasetView.Visible = true;
                    datasetButton.CssClass = "selected";
                    break;
                case SchemaView.DatabaseObjectList:
                    SelectedDataset.LoadAllObjects(SelectedObjectType, SelectedDataset.IsMutable);
                    databaseObjectListView.Items = SelectedDataset.GetAllObjects(SelectedObjectType).OrderBy(o => o, new DatabaseObjectLexicographicComparer());
                    databaseObjectListView.Visible = true;
                    objectListButton.CssClass = "selected";
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
                case SchemaView.Peek:
                    peekView.Item = (TableOrView)SelectedDatabaseObject;
                    peekView.Visible = true;
                    peekButton.CssClass = "selected";
                    break;
                default:
                    throw new NotImplementedException();
            }

            datasetButton.Visible = ds != null;
            objectTypeListDiv.Visible = ds != null;
            objectListButton.Visible = ot != DatabaseObjectType.Unknown;
            databaseObjectListDiv.Visible = ot != DatabaseObjectType.Unknown;

            datasetList.SelectedValue = ds?.Name;

            if (ds != null)
            {
                objectTypeList.SelectedValue = ot == DatabaseObjectType.Unknown ? "" : ot.ToString();
            }

            if (ot != DatabaseObjectType.Unknown)
            {
                databaseObjectList.SelectedValue = dbobj?.UniqueKey;
            }

            if (dbobj != null)
            {
                toolbarSpan.Visible = false;
                summaryButton.Visible = true;
                columnsButton.Visible = (dbobj is IColumns);
                indexesButton.Visible = (dbobj is IIndexes);
                parametersButton.Visible = (dbobj is IParameters);
                peekButton.Visible = (dbobj is TableOrView);
            }
            else
            {
                toolbarSpan.Visible = true;
            }

            SessionView = SelectedView;
            SessionDataset = SelectedDataset;
            SessionObjectType = SelectedObjectType;
            SessionDatabaseObject = SelectedDatabaseObject;

            OverrideUrl(Page.ResolveUrl(GetUrl(SelectedView, SelectedDataset, SelectedObjectType, SelectedDatabaseObject)));
        }

        private void HideAllViews()
        {
            introForm.Visible = false;
            datasetListView.Visible = false;
            datasetView.Visible = false;
            databaseObjectListView.Visible = false;
            databaseObjectView.Visible = false;
            columnsView.Visible = false;
            parametersView.Visible = false;
            indexesView.Visible = false;
            peekView.Visible = false;

            objectTypeListDiv.Visible = false;
            databaseObjectListDiv.Visible = false;

            summaryButton.Visible = false;
            columnsButton.Visible = false;
            indexesButton.Visible = false;
            parametersButton.Visible = false;
            peekButton.Visible = false;

            datasetListButton.CssClass = "";
            datasetButton.CssClass = "";
            objectListButton.CssClass = "";
            summaryButton.CssClass = "";
            columnsButton.CssClass = "";
            indexesButton.CssClass = "";
            parametersButton.CssClass = "";
            peekButton.CssClass = "";
        }

        private void ShowDatasetListView()
        {
            SelectedView = SchemaView.DatasetList;
            SelectedDataset = null;
            SelectedObjectType = DatabaseObjectType.Unknown;
            selectedDatabaseObject = null;
        }

        private void ShowDatasetView(DatasetBase ds)
        {
            SelectedView = SchemaView.Dataset;
            SelectedDataset = ds;
            SelectedObjectType = DatabaseObjectType.Unknown;
            SelectedDatabaseObject = null;
        }

        private void ShowDatabaseObjectListView(DatasetBase ds, DatabaseObjectType objectType)
        {
            SelectedView = SchemaView.DatabaseObjectList;
            SelectedDataset = ds;
            SelectedObjectType = objectType;
            SelectedDatabaseObject = null;
        }

        private void ShowDatabaseObjectView(DatabaseObject dbobj, SchemaView view)
        {
            switch (view)
            {
                case SchemaView.DatabaseObject:
                    break;
                case SchemaView.Columns:
                    if (!(dbobj is IColumns))
                    {
                        goto default;
                    }
                    break;
                case SchemaView.Indexes:
                    if (!(dbobj is IIndexes))
                    {
                        goto default;
                    }
                    break;
                case SchemaView.Parameters:
                    if (!(dbobj is IParameters))
                    {
                        goto default;
                    }
                    break;
                case SchemaView.Peek:
                    if (!(dbobj is TableOrView))
                    {
                        goto default;
                    }
                    break;
                default:
                    if (String.IsNullOrEmpty(dbobj.Metadata.Summary))
                    {
                        view = SchemaView.Columns;
                    }
                    else
                    {
                        view = SchemaView.DatabaseObject;
                    }
                    break;
            }

            SelectedView = view;
            SelectedDataset = dbobj.Dataset;
            SelectedObjectType = Jhu.Graywulf.Sql.Schema.Constants.SimpleDatabaseObjectTypes[dbobj.ObjectType];
            SelectedDatabaseObject = dbobj;
        }
    }
}