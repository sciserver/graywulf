using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class SqlQueryParameters
    {
        #region Private member variables

        /// <summary>
        /// Type name of the query factory class
        /// </summary>
        private string queryFactoryTypeName;

        /// <summary>
        /// The original query to be executed
        /// </summary>
        private string queryString;

        /// <summary>
        /// Batch name for automatic output table naming
        /// </summary>
        private string batchName;

        /// <summary>
        /// Query name for automatic output table naming
        /// </summary>
        private string queryName;

        private string federationName;

        /// <summary>
        /// Database version to be used to execute the queries (PROD)
        /// </summary>
        private string sourceDatabaseVersionName;

        /// <summary>
        /// Database version to be used to calculate statistics (STAT)
        /// </summary>
        private string statDatabaseVersionName;

        /// <summary>
        /// Destination tables including target table naming patterns.
        /// Output table names are either automatically generated or
        /// taken from the INTO clauses.
        /// </summary>
        private DestinationTable destination;

        /// <summary>
        /// Individual query time-out, overall job timeout is enforced by
        /// the scheduler in a different way.
        /// </summary>
        private int queryTimeout;

        private int maxPartitions;

        /// <summary>
        /// Determines if queries are dumped into files during execution
        /// </summary>
        private bool dumpSql;

        /// <summary>
        /// The dataset to be assumed when no DATASET: part in
        /// table names appear.
        /// </summary>
        private SqlServerDataset defaultSourceDataset;

        /// <summary>
        /// The dataset to be assumes when no DATASET: part in
        /// SELECT INTO and CREATE TABLE statements appear
        /// </summary>
        private SqlServerDataset defaultOutputDataset;

        /// <summary>
        /// A list of custom datasets, i.e. those that are not
        /// configured centrally, for example MyDB
        /// </summary>
        private List<DatasetBase> customDatasets;

        /// <summary>
        /// Query execution mode, either single server or Graywulf cluster
        /// </summary>
        private ExecutionMode executionMode;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the type name string of the query factory class
        /// </summary>
        [DataMember]
        public string QueryFactoryTypeName
        {
            get { return queryFactoryTypeName; }
            set { queryFactoryTypeName = value; }
        }

        /// <summary>
        /// Gets or sets the query string of the query job.
        /// </summary>
        [DataMember]
        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

        [DataMember]
        public string BatchName
        {
            get { return batchName; }
            set { batchName = value; }
        }

        [DataMember]
        public string QueryName
        {
            get { return queryName; }
            set { queryName = value; }
        }

        [DataMember]
        public string FederationName
        {
            get { return federationName; }
            set { federationName = value; }
        }

        [DataMember]
        public string SourceDatabaseVersionName
        {
            get { return sourceDatabaseVersionName; }
            set { sourceDatabaseVersionName = value; }
        }

        [DataMember]
        public string StatDatabaseVersionName
        {
            get { return statDatabaseVersionName; }
            set { statDatabaseVersionName = value; }
        }

        /// <summary>
        /// Gets or sets the destination table naming pattern of the query
        /// </summary>
        [DataMember]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of individual queries
        /// </summary>
        /// <remarks>
        /// The overall timeout period is enforced by the scheduler.
        /// </remarks>
        [DataMember]
        public int QueryTimeout
        {
            get { return queryTimeout; }
            set { queryTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of partitions
        /// </summary>
        [DataMember]
        public int MaxPartitions
        {
            get { return maxPartitions; }
            set { maxPartitions = value; }
        }

        /// <summary>
        /// Gets or sets whether SQL scripts are dumped to files during query execution.
        /// </summary>
        [DataMember]
        public bool DumpSql
        {
            get { return dumpSql; }
            set { dumpSql = value; }
        }


        /// <summary>
        /// Gets or sets the default dataset, i.e. the one that's assumed
        /// when no dataset part is specified in table names.
        /// </summary>
        [DataMember]
        public SqlServerDataset DefaultSourceDataset
        {
            get { return defaultSourceDataset; }
            set { defaultSourceDataset = value; }
        }

        /// <summary>
        /// Gets or sets the default dataset for table output
        /// </summary>
        [DataMember]
        public SqlServerDataset DefaultOutputDataset
        {
            get { return defaultOutputDataset; }
            set { defaultOutputDataset = value; }
        }

        /// <summary>
        /// Gets a list of custom datasets.
        /// </summary>
        /// <remarks>
        /// In case of Graywulf execution mode, this stores
        /// the datasets not in the default list (remote datasets,
        /// for instance)
        /// </remarks>
        [IgnoreDataMember]
        public List<DatasetBase> CustomDatasets
        {
            get { return customDatasets; }
            private set { customDatasets = value; }
        }

        [DataMember(Name = "CustomDatasets")]
        [XmlArray]
        public DatasetBase[] CustomDatasets_ForXml
        {
            get { return customDatasets.ToArray(); }
            set { customDatasets = new List<DatasetBase>(value); }
        }

        /// <summary>
        /// Gets or sets query execution mode.
        /// </summary>
        /// <remarks>
        /// Graywulf or single server
        /// </remarks>
        [DataMember]
        public ExecutionMode ExecutionMode
        {
            get { return executionMode; }
            set { executionMode = value; }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryParameters()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryParameters(SqlQueryParameters old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.queryFactoryTypeName = null;
            this.queryString = null;
            this.batchName = null;
            this.queryName = null;

            this.federationName = String.Empty;
            this.sourceDatabaseVersionName = String.Empty;
            this.statDatabaseVersionName = String.Empty;

            this.destination = null;
            
            this.queryTimeout = 60;
            this.maxPartitions = 0;
            this.dumpSql = false;

            this.defaultSourceDataset = null;
            this.defaultOutputDataset = null;
            this.customDatasets = new List<DatasetBase>();

            this.executionMode = ExecutionMode.SingleServer;
        }

        private void CopyMembers(SqlQueryParameters old)
        {
            this.queryFactoryTypeName = old.queryFactoryTypeName;
            this.queryString = old.queryString;
            this.batchName = old.batchName;
            this.queryName = old.queryName;

            this.federationName = old.federationName;
            this.sourceDatabaseVersionName = old.sourceDatabaseVersionName;
            this.statDatabaseVersionName = old.statDatabaseVersionName;

            this.destination = old.destination;
            
            this.queryTimeout = old.queryTimeout;
            this.maxPartitions = old.maxPartitions;
            this.dumpSql = old.dumpSql;

            this.defaultSourceDataset = old.defaultSourceDataset;
            this.defaultOutputDataset = old.defaultOutputDataset;
            this.customDatasets = new List<DatasetBase>(old.customDatasets);

            this.executionMode = old.executionMode;
        }

        #endregion
    }
}
