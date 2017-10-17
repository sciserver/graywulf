using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Schema
{
    public class UnavailableDataset : DatasetBase
    {
        public override string ProviderName
        {
            get { return "Unavailable"; }
        }

        public override string DatabaseName
        {
            get { return null; }
            set { }
        }

        public static UnavailableDataset Create(string name, Exception ex)
        {
            return new UnavailableDataset()
            {
                Name = name,
                IsInError = true,
                ErrorMessage = ex?.Message
            };
        }

        public override string QuoteIdentifier(string identifier)
        {
            throw new NotImplementedException();
        }

        public override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override void LoadDatabaseObject<T>(T databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override bool IsObjectExisting(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>()
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<KeyValuePair<string, Column>> LoadColumns(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<KeyValuePair<string, IndexColumn>> LoadIndexColumns(Index index)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<KeyValuePair<string, Index>> LoadIndexes(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<KeyValuePair<string, Parameter>> LoadParameters(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override DatasetMetadata LoadDatasetMetadata()
        {
            throw new NotImplementedException();
        }

        protected internal override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void SaveDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void DropDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override void LoadAllColumnMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void SaveAllVariableMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void DropAllVariableMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        protected override DatasetStatistics LoadDatasetStatistics()
        {
            throw new NotImplementedException();
        }

        internal override TableStatistics LoadTableStatistics(TableOrView tableOrView)
        {
            throw new NotImplementedException();
        }

        internal override void OnRenameObject(DatabaseObject obj, string schemaName, string objectName)
        {
            throw new NotImplementedException();
        }

        internal override void OnCreateTable(Table table, bool createPrimaryKey, bool createIndexes)
        {
            throw new NotImplementedException();
        }

        internal override void OnDropObject(DatabaseObject obj)
        {
            throw new NotImplementedException();
        }

        internal override void OnCreateIndex(Index index)
        {
            throw new NotImplementedException();
        }

        internal override void OnDropIndex(Index index)
        {
            throw new NotImplementedException();
        }

        internal override void OnTruncateTable(Table table)
        {
            throw new NotImplementedException();
        }

        public override string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist)
        {
            throw new NotImplementedException();
        }

        protected override DataType CreateDataType(DataRow dr)
        {
            return base.CreateDataType(dr);
        }

        protected override DataType CreateDataType(string name)
        {
            throw new NotImplementedException();
        }

        public override IDbConnection OpenConnection()
        {
            throw new NotImplementedException();
        }
    }
}
