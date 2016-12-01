using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Web.Api.Util
{
    public static class SqlParser
    {
        public static bool TryParseTableName(FederationContext context, string dataset, string token, out TableReference tr)
        {
            if (token != null)
            {
                try
                {
                    var parser = new Jhu.Graywulf.SqlParser.SqlParser();
                    var tn = (Jhu.Graywulf.SqlParser.TableOrViewName)parser.Execute(new Jhu.Graywulf.SqlParser.TableOrViewName(), token);

                    tr = tn.TableReference;
                    tr.SubstituteDefaults(context.SchemaManager, dataset);

                    return true;
                }
                catch (Exception)
                {
                }
            }

            tr = null;
            return false;
        }

        public static bool TryParseTableName(FederationContext context, string dataset, string token, out string schemaName, out string tableName)
        {
            TableReference tr;
            if (TryParseTableName(context, dataset, token, out tr))
            {
                schemaName = tr.SchemaName;
                tableName = tr.DatabaseObjectName;

                return true;
            }

            schemaName = null;
            tableName = null;
            return false;
        }

        public static bool TryParseTableName(FederationContext context, string dataset, string token, out TableOrView table)
        {
            TableReference tr;
            if (TryParseTableName(context, dataset, token, out tr))
            {
                var ds = context.SchemaManager.Datasets[tr.DatasetName];
                table = (TableOrView)ds.GetObject(tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName);

                return true;
            }

            table = null;
            return false;
        }

        public static string CombineTableName(string schemaName, string tableName)
        {
            return (String.IsNullOrWhiteSpace(schemaName) ? "" : (schemaName + ".")) + tableName;
        }
    }
}
