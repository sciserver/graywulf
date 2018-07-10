using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.MySql;

namespace Jhu.Graywulf.Sql.QueryGeneration.MySql
{
    public class MySqlColumnListGenerator : ColumnListGeneratorBase
    {
        public MySqlColumnListGenerator()
        {
            InitializeMembers();
        }

        public MySqlColumnListGenerator(IEnumerable<ColumnReference> columns)
            : base(columns)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override QueryRendererBase CreateQueryRenderer()
        {
            return new MySqlQueryRenderer();
        }

        // TODO: add this function to the select list generation loop somehow
        private string GetConvertedColumnString(ColumnReference cr)
        {
            /*
            if (cr.DataType.IsInteger)
            {
                // Here a cast to a type that is accepted by SQL Server has to be made
                return String.Format("CAST({0} AS SIGNED) AS {1}", GetResolvedColumnName(cr), GetQuotedIdentifier(cr.ColumnName)));
            }
            else
            {
                sql.Write(GetResolvedColumnName(cr));
            }
             * */

            throw new NotImplementedException();
        }
    }
}
