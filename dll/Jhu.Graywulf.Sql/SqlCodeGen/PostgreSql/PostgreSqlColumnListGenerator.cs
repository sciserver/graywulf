using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlCodeGen.PostgreSql
{
    public class PostgreSqlColumnListGenerator : SqlColumnListGeneratorBase
    {
        public PostgreSqlColumnListGenerator(TableReference table, ColumnContext context, ColumnListType listType)
            : base(table, context, listType)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override string GetQuotedIdentifier(string identifier)
        {
            return PostgreSqlCodeGenerator.QuoteIdentifier(identifier);
        }
    }
}
