using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.CodeGeneration.SqlServer
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

        public SqlServerColumnListGenerator(TableOrView table)
            : base(table)
        {
            InitializeMembers();
        }

        public SqlServerColumnListGenerator(Index index)
            : base(index)
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
