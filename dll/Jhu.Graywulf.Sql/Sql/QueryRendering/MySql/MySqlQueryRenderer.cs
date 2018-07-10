using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.QueryRendering.MySql
{
    public class MySqlQueryRenderer : QueryRendererBase
    {
        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("`{0}`", identifier);
        }

        public override string GetQuotedIdentifier(string identifier)
        {
            return QuoteIdentifier(UnquoteIdentifier(identifier));
        }

        #region Identifier formatting functions

        public override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

            res += GetQuotedIdentifier(tableName);

            return res;
        }

        public override string GetResolvedDataTypeName(DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override string GetResolvedDataTypeName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

            res += GetQuotedIdentifier(functionName);

            return res;
        }

        public override string GetResolvedFunctionName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

            res += GetQuotedIdentifier(functionName);

            return res;
        }

        #endregion
    }
}
