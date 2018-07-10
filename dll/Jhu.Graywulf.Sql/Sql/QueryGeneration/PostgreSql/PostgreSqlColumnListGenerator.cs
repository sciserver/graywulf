using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.PostgreSql;

namespace Jhu.Graywulf.Sql.QueryGeneration.PostgreSql
{
    public class PostgreSqlColumnListGenerator : ColumnListGeneratorBase
    {
        public PostgreSqlColumnListGenerator()
        {
            InitializeMembers();
        }

        public PostgreSqlColumnListGenerator(IEnumerable<ColumnReference> columns)
            : base(columns)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override QueryRendererBase CreateQueryRenderer()
        {
            return new PostgreSqlQueryRenderer();
        }
    }
}
