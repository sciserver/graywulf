using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.Security.Permissions;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    [Description("Gives access to the datasets and database schema.")]
    public interface ISchemaService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets")]
        [Description("Returns a list of all available datasets.")]
        DatasetListResponse ListDatasets();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}")]
        [Description("Returns information about a single dataset")]
        Dataset GetDataset(
            [Description("Name of the dataset.")]
            string datasetName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables")]
        [Description("Returns a list of the tables of a dataset.")]
        TableListResponse ListTables(
            [Description("Name of the dataset.")]
            string datasetName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables/{tableName}")]
        [Description("Returns information about a single table.")]
        Table GetTable(
            [Description("Name of the dataset.")]
            string datasetName,
            [Description("Name of the table.")]
            string tableName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables/{tableName}/columns")]
        [Description("Returns the list of columns of a table")]
        ColumnListResponse ListTableColumns(
            [Description("Name of the dataset.")]
            string datasetName,
            [Description("Name of the table.")]
            string tableName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/views")]
        [Description("Returns a list of the views of a dataset.")]
        ViewListResponse ListViews(
            [Description("Name of the dataset.")]
            string datasetName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/views/{viewName}")]
        [Description("Returns information about a single view.")]
        View GetView(
            [Description("Name of the dataset.")]
            string datasetName,
            [Description("Name of the view.")]
            string viewName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/views/{viewName}/columns")]
        [Description("Returns the list of columns of a view")]
        ColumnListResponse ListViewColumns(
            [Description("Name of the dataset.")]
            string datasetName,
            [Description("Name of the view.")]
            string viewName);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class SchemaService : RestServiceBase, ISchemaService
    {
        #region Dataset functions

        private Schema.DatasetBase GetDatasetInternal(string datasetName)
        {
            return FederationContext.SchemaManager.Datasets[datasetName];
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public DatasetListResponse ListDatasets()
        {
            // Load all datasets and refresh cache by getting all items in list
            FederationContext.SchemaManager.Datasets.LoadAll();

            return new DatasetListResponse(FederationContext.SchemaManager.Datasets.Values);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public Dataset GetDataset(string datasetName)
        {
            return new Dataset(GetDatasetInternal(datasetName));
        }

        #endregion
        #region Table functions

        private Schema.Table GetTableInternal(string datasetName, string tableName)
        {
            var parts = tableName.Split('.');
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            return dataset.Tables[dataset.DatabaseName, parts[0], parts[1]];
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public TableListResponse ListTables(string datasetName)
        {
            var dataset = GetDatasetInternal(datasetName);
            dataset.Tables.LoadAll();

            return new TableListResponse(dataset.Tables.Values);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public Table GetTable(string datasetName, string tableName)
        {
            var table = GetTableInternal(datasetName, tableName);
            return new Table(table);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public ColumnListResponse ListTableColumns(string datasetName, string tableName)
        {
            var table = GetTableInternal(datasetName, tableName);
            return new ColumnListResponse(table.Columns.Values);
        }

        #endregion
        #region View functions

        private Schema.View GetViewInternal(string datasetName, string viewName)
        {
            var parts = viewName.Split('.');
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            return dataset.Views[dataset.DatabaseName, parts[0], parts[1]];
        }

        public ViewListResponse ListViews(string datasetName)
        {
            var dataset = GetDatasetInternal(datasetName);
            dataset.Views.LoadAll();

            return new ViewListResponse(dataset.Views.Values);
        }

        public View GetView(string datasetName, string viewName)
        {
            var view = GetViewInternal(datasetName, viewName);
            return new View(view);
        }

        public ColumnListResponse ListViewColumns(string datasetName, string viewName)
        {
            var view = GetViewInternal(datasetName, viewName);
            return new ColumnListResponse(view.Columns.Values);
        }

        #endregion
    }
}
