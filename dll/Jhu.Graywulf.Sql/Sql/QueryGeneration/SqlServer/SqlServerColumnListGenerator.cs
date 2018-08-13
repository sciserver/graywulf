using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryTraversal;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.SqlServer;

namespace Jhu.Graywulf.Sql.QueryGeneration.SqlServer
{
    public class SqlServerColumnListGenerator : ColumnListGeneratorBase
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

        protected override QueryRendererBase CreateQueryRenderer()
        {
            return new SqlServerQueryRenderer();
        }
    }
}
