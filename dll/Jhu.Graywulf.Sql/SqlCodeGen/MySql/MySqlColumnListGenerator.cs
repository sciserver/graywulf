using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlCodeGen.MySql
{
    public class MySqlColumnListGenerator : SqlColumnListGeneratorBase
    {
        public MySqlColumnListGenerator(TableReference table, ColumnContext context, ColumnListType listType)
            : base(table, context, listType)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override string GetQuotedIdentifier(string identifier)
        {
            return MySqlCodeGenerator.QuoteIdentifier(identifier);
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
