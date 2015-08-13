using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryPartition : QueryPartitionBase, ICloneable
    {
        #region Constructors and initializers

        public SqlQueryPartition()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryPartition(SqlQuery query, Context context)
            : base(query, context)
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryPartition(SqlQueryPartition old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(SqlQueryPartition old)
        {
        }

        public override object Clone()
        {
            return new SqlQueryPartition(this);
        }

        #endregion

        public override void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            base.PrepareExecuteQuery(context, scheduler);

            SubstituteDatabaseNames(SelectStatement, AssignedServerInstance, Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames(SelectStatement, TemporaryDatabaseInstanceReference.Value.GetDataset(), Query.TemporaryDataset.DefaultSchemaName);
        }

        protected override string GetExecuteQueryText()
        {
            // Take a copy of the parsing tree
            var selectStatement = new SelectStatement(SelectStatement);
            CodeGenerator.RewriteQueryForExecute(selectStatement, PartitioningKeyFrom, PartitioningKeyTo);
            return CodeGenerator.Execute(selectStatement);
        }
    }
}
