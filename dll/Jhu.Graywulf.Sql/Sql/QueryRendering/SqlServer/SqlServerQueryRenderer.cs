using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.QueryRendering.SqlServer
{
    public class SqlServerQueryRenderer : QueryRendererBase
    {
        public static string GetCode(Node node, bool resolvedNames)
        {
            var sw = new StringWriter();
            var cg = new SqlServerQueryRenderer();

            cg.TableNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.ColumnNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.UdtMemberNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.DataTypeNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.FunctionNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.IndexNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.ConstraintNameRendering = resolvedNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.Execute(sw, node);

            return sw.ToString();
        }

        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
        }

        public override string GetQuotedIdentifier(string identifier)
        {
            return QuoteIdentifier(identifier);
        }

        public override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            if (!String.IsNullOrWhiteSpace(schemaName))
            {
                res += GetQuotedIdentifier(schemaName);
            }

            if (!String.IsNullOrWhiteSpace(tableName))
            {
                // If no schema name is specified but there's a database name,
                // SQL Server uses the database..table syntax
                if (res != String.Empty)
                {
                    res += ".";
                }

                res += GetQuotedIdentifier(tableName);
            }

            return res;
        }

        public override string GetResolvedDataTypeName(DataType dataType)
        {
            // TODO: This is a duplicate with DataType.TypeNameWithLength, merge

            var name = GetQuotedIdentifier(dataType.ObjectName.ToLowerInvariant());

            if (!dataType.HasLength)
            {
                return name;
            }
            else if (dataType.HasPrecision && !dataType.HasScale)
            {
                return String.Format("{0}({1})", name, dataType.Precision);
            }
            else if (dataType.HasScale && !dataType.HasPrecision)
            {
                return String.Format("{0}({1})", name, dataType.Scale);
            }
            else if (dataType.HasScale && dataType.HasPrecision)
            {
                return String.Format("{0}({1}, {2})", name, dataType.Precision, dataType.Scale);
            }
            else if (dataType.IsMaxLength)
            {
                return String.Format("{0}(max)", name);
            }
            else
            {
                return String.Format("{0}({1})", name, dataType.Length);
            }
        }

        public override string GetResolvedDataTypeName(string databaseName, string schemaName, string dataTypeName)
        {
            string res = String.Empty;

            // SQL Server UDTs can never have a database name but might have a schema name

            if (schemaName != null)
            {
                res += GetQuotedIdentifier(schemaName) + ".";
            }

            res += GetQuotedIdentifier(dataTypeName);

            return res;
        }

        public override string GetResolvedFunctionName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // SQL Server function must always have the schema name specified
            res += GetQuotedIdentifier(schemaName) + ".";
            res += GetQuotedIdentifier(functionName);

            return res;
        }
    }
}
