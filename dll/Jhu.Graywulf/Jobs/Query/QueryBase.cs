using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public abstract class QueryBase : QueryObject
    {
        #region Property storage member variables

        private int queryTimeout;

        private DatasetBase temporaryDataset;
        private string temporarySchemaName;

        private DestinationTableParameters destination;
        private bool isDestinationTableInitialized;

        private ResultsetTarget resultsetTarget;
        private string temporaryDestinationTableName;
        private bool keepTemporaryDestinationTable;

        private string sourceDatabaseVersionName;
        private string statDatabaseVersionName;

        private List<QueryPartitionBase> partitions;

        #endregion
        #region Member variables

        [NonSerialized]
        private EntityProperty<DatabaseInstance> destinationDatabaseInstanceReference;

        [NonSerialized]
        private string partitioningTable;
        [NonSerialized]
        private string partitioningKey;

        #endregion
        #region Properties

        [DataMember]
        public int QueryTimeout
        {
            get { return queryTimeout; }
            set { queryTimeout = value; }
        }

        [DataMember]
        public DatasetBase TemporaryDataset
        {
            get { return temporaryDataset; }
            set { temporaryDataset = value; }
        }

        [DataMember]
        public string TemporarySchemaName
        {
            get { return temporarySchemaName; }
            set { temporarySchemaName = value; }
        }

        [DataMember]
        public DestinationTableParameters Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        [DataMember]
        public bool IsDestinationTableInitialized
        {
            get { return isDestinationTableInitialized; }
            set { isDestinationTableInitialized = value; }
        }

        [DataMember]
        public ResultsetTarget ResultsetTarget
        {
            get { return resultsetTarget; }
            set { resultsetTarget = value; }
        }

        [DataMember]
        public string TemporaryDestinationTableName
        {
            get { return temporaryDestinationTableName; }
            set { temporaryDestinationTableName = value; }
        }

        [DataMember]
        public bool KeepTemporaryDestinationTable
        {
            get { return keepTemporaryDestinationTable; }
            set { keepTemporaryDestinationTable = value; }
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

        [IgnoreDataMember]
        public List<QueryPartitionBase> Partitions
        {
            get { return partitions; }
        }

        [IgnoreDataMember]
        public abstract bool IsPartitioned
        {
            get;
        }

        #endregion
        #region Constructors

        protected QueryBase()
        {
            InitializeMembers(new StreamingContext());
        }

        protected QueryBase(QueryBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        public QueryBase(Context context)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
        }

        #endregion
        #region Initializer functions

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.queryTimeout = 60; // TODO ***

            this.temporaryDataset = null;
            this.temporarySchemaName = "dbo";

            this.destination = new DestinationTableParameters();
            this.isDestinationTableInitialized = false;

            this.resultsetTarget = ResultsetTarget.DestinationTable;
            this.temporaryDestinationTableName = String.Empty;
            this.keepTemporaryDestinationTable = false;

            this.sourceDatabaseVersionName = String.Empty;
            this.statDatabaseVersionName = String.Empty;

            this.partitions = new List<QueryPartitionBase>();

            this.destinationDatabaseInstanceReference = new EntityProperty<DatabaseInstance>();

            this.partitioningTable = null;
            this.partitioningKey = null;
        }

        private void CopyMembers(QueryBase old)
        {
            this.queryTimeout = old.queryTimeout;

            this.temporaryDataset = old.temporaryDataset;
            this.temporarySchemaName = old.temporarySchemaName;

            this.destination = old.destination;
            this.isDestinationTableInitialized = old.isDestinationTableInitialized;

            this.resultsetTarget = old.resultsetTarget;
            this.temporaryDestinationTableName = old.temporaryDestinationTableName;
            this.keepTemporaryDestinationTable = old.keepTemporaryDestinationTable;

            this.sourceDatabaseVersionName = old.sourceDatabaseVersionName;
            this.statDatabaseVersionName = old.statDatabaseVersionName;

            this.partitions = new List<QueryPartitionBase>(old.partitions.Select(p => (QueryPartitionBase)p.Clone()));

            this.destinationDatabaseInstanceReference = new EntityProperty<DatabaseInstance>(old.destinationDatabaseInstanceReference);

            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
        }

        #endregion

        #region Parsing functions

        public void Verify()
        {
            Parse(true);
            Interpret(true);
            Validate();
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();

            if (this.destinationDatabaseInstanceReference != null) this.destinationDatabaseInstanceReference.Context = Context;
        }

        protected override void FinishInterpret(bool forceReinitialize)
        {
            // --- Retrieve target table information
            IntoClause into = SelectStatement.FindDescendantRecursive<IntoClause>();
            if (into != null)
            {
                
                // **** TODO: test this with dataset name
                //if (into.TableReference.DatasetName != null) this.destinationTable.Table.Dataset.Name = into.TableReference.DatasetName;

                if (into.TableReference.SchemaName != null)
                {
                    this.destination.Table.SchemaName = into.TableReference.SchemaName;
                }

                if (into.TableReference.TableName != null)
                {
                    this.destination.Table.TableName = into.TableReference.TableName;
                }

                // remove into clause from query
                into.Parent.Stack.Remove(into);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public override void InitializeQueryObject(Context context, bool forceReinitialize)
        {
            if (context != null)
            {
                this.Context = context;
                UpdateContext();
            }

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    break;
                case ExecutionMode.Graywulf:
                    LoadDestinationDatabaseInstance(forceReinitialize);
                    break;
                default:
                    throw new NotImplementedException();
            }

            base.InitializeQueryObject(context, forceReinitialize);
        }

        protected void AssertValidContext()
        {
            // This call requires a live context
            System.Diagnostics.Debug.Assert(this.ExecutionMode != ExecutionMode.Graywulf || Context != null && Context.IsValid);
        }

        #endregion
        #region Query partitioning functions

        public abstract void GeneratePartitions(int availableServerCount);

        protected void AppendPartition(QueryPartitionBase partition)
        {
            partitions.Add(partition);
        }

        #endregion
        #region Query execution functions

        public void LoadDestinationDatabaseInstance(bool forceReinitialize)
        {
            if (destinationDatabaseInstanceReference.IsEmpty || forceReinitialize)
            {
                var dd = (GraywulfDataset)destination.Table.Dataset;
                if (destinationDatabaseInstanceReference.IsEmpty && !String.IsNullOrEmpty(dd.DatabaseInstanceName))
                {
                    destinationDatabaseInstanceReference.Name = dd.DatabaseInstanceName;
                    destinationDatabaseInstanceReference.LoadEntity();
                }

                destinationDatabaseInstanceReference.Value.GetConnectionString();
            }
        }

        public SqlConnectionStringBuilder GetDestinationDatabaseConnectionString()
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return new SqlConnectionStringBuilder(((SqlServerDataset)destination.Table.Dataset).ConnectionString);
                case ExecutionMode.Graywulf:
                    return destinationDatabaseInstanceReference.Value.GetConnectionString();
                default:
                    throw new NotImplementedException();
            }
        }

        public virtual void IsDestinationTableExisting()
        {
            AssertValidContext();

            if ((resultsetTarget & ResultsetTarget.DestinationTable) != 0)
            {
                bool exists = IsTableExisting(
                    GetDestinationDatabaseConnectionString().ConnectionString,
                    destination.Table.SchemaName,
                    destination.Table.TableName);

                if (exists && (destination.Operation & DestinationTableOperation.Drop) != 0)
                {
                    DropTable(
                        GetDestinationDatabaseConnectionString().ConnectionString,
                        GetDestinationDatabaseConnectionString().InitialCatalog,
                        destination.Table.SchemaName,
                        destination.Table.TableName);

                    exists = false;
                    destination.Operation = destination.Operation & ~DestinationTableOperation.Drop;
                }

                switch (destination.Operation)
                {
                    case DestinationTableOperation.Create:
                        if (exists) throw new Exception("Output table already exists.");
                        break;
                    case DestinationTableOperation.Append:
                    case DestinationTableOperation.Clear:
                        throw new NotImplementedException();
                    // **** TODO check format compatibility
                }
            }
        }

        #endregion

        protected TableStatistics ComputeTableStatistics(TableSource table, decimal binSize)
        {
            return ComputeTableStatistics(table.TableReference, table.PartitioningColumnReference.ColumnName, binSize);
        }

        protected TableStatistics ComputeTableStatistics(TableReference table, string sampledColumn, decimal binSize)
        {
            // Build table specific where clause
            var where = new StringWriter();

            var cnr = new SearchConditionNormalizer();
            cnr.Execute(SelectStatement.EnumerateQuerySpecifications().First());    // TODO: what if more than one SQ?
            var wh = cnr.GenerateWhereClauseSpecificToTable(table);
            if (wh != null)
            {
                var cg = new SqlServerCodeGenerator();
                cg.Execute(where, wh);
            };

            var stat = new TableStatistics();
            stat.Table = table;
            stat.SampledColumn = sampledColumn;

            // *** TODO: maybe move this to code generator

            string sql = String.Format(@"
SELECT COUNT(*), ROUND(CONVERT(decimal(10,6), {2}) / @BinSize, 0) * @BinSize
FROM {0} {1} WITH (NOLOCK)
{3}
GROUP BY ROUND(CONVERT(decimal(10,6), {2}) / @BinSize, 0) * @BinSize
ORDER BY ROUND(CONVERT(decimal(10,6), {2}) / @BinSize, 0) * @BinSize",
                     table.FullyResolvedName,
                     table.Alias == null ? "" : String.Format(" AS [{0}] ", table.Alias),
                     sampledColumn,
                     where.ToString());


            // --- Execute query and extract results

            //     Use assigned server instance to run statistics queries
            var si = AssignedServerInstanceReference.Value;

            using (SqlConnection cn = new SqlConnection(si.GetConnectionString().ConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@BinSize", SqlDbType.Decimal).Value = binSize;
                    cmd.CommandTimeout = queryTimeout;

                    ExecuteLongCommandReader(cmd, dr =>
                    {
                        stat.Count = 0;
                        while (dr.Read())
                        {
                            stat.BinCount.Add(dr.GetInt32(0));
                            stat.BinMin.Add(dr.GetDecimal(1));
                            stat.BinMax.Add(dr.GetDecimal(1) + binSize);
                            stat.Count += dr.GetInt32(0);
                        }
                    });
                }
            }

            return stat;
        }
    }
}
