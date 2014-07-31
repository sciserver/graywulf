using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.Web.Routing;
using System.Security.Permissions;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api
{
    [ServiceContract]
    public interface ISchemaService
    {
        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets")]
        DatasetList ListDatasets();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}")]
        Dataset GetDataset(string datasetName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables")]
        TableList ListTables(string datasetName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables/{tableName}")]
        Table GetTable(string datasetName, string tableName);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/datasets/{datasetName}/tables/{tableName}/columns")]
        ColumnList ListTableColumns(string datasetName, string tableName);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class SchemaService : ServiceBase, ISchemaService
    {
        private Schema.DatasetBase GetDatasetInternal(string datasetName)
        {
            return FederationContext.SchemaManager.Datasets[datasetName];
        }

        private Schema.Table GetTableInternal(string datasetName, string tableName)
        {
            var parts = tableName.Split('.');
            var dataset = FederationContext.SchemaManager.Datasets[datasetName];
            return dataset.Tables[dataset.DatabaseName, parts[0], parts[1]];
        }

        public DatasetList ListDatasets()
        {
            return new DatasetList(FederationContext.SchemaManager.Datasets.Values);
        }

        public Dataset GetDataset(string datasetName)
        {
            return new Dataset(GetDatasetInternal(datasetName));
        }

        public TableList ListTables(string datasetName)
        {
            var dataset = GetDatasetInternal(datasetName);
            dataset.Tables.LoadAll();

            return new TableList(dataset.Tables.Values);
        }

        public Table GetTable(string datasetName, string tableName)
        {
            var table = GetTableInternal(datasetName, tableName);
            return new Table(table);
        }

        public ColumnList ListTableColumns(string datasetName, string tableName)
        {
            var table = GetTableInternal(datasetName, tableName);
            return new ColumnList(table.Columns.Values);
        }
    }
}
