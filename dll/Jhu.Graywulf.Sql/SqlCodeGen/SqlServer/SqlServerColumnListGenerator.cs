using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlCodeGen.SqlServer
{
    public class SqlServerColumnListGenerator : SqlColumnListGeneratorBase
    {
        public SqlServerColumnListGenerator()
        {
            InitializeMembers();
        }

        public SqlServerColumnListGenerator(TableReference table, ColumnContext context)
            : base(table, context)
        {
            InitializeMembers();
        }

        public SqlServerColumnListGenerator(TableReference table, ColumnContext context, ColumnListType listType)
            : base(table, context, listType)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override string GetQuotedIdentifier(string identifier)
        {
            return SqlServerCodeGenerator.QuoteIdentifier(identifier);
        }
    }
}
