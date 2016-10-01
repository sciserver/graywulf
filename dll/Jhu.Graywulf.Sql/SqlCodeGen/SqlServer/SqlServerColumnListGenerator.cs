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

        public SqlServerColumnListGenerator(IEnumerable<ColumnReference> columns)
            : base(columns)
        {
            InitializeMembers();
        }

        public SqlServerColumnListGenerator(TableReference tr, ColumnContext context, ColumnListType listType)
            : base(tr.FilterColumnReferences(context))
        {
            InitializeMembers();

            ListType = listType;
        }

        private void InitializeMembers()
        {
        }

        protected override string QuoteIdentifier(string identifier)
        {
            return SqlServerCodeGenerator.QuoteIdentifier(identifier);
        }
    }
}
