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
        public static void ParseTableName(FederationContext context, string token, out TableReference tr)
        {
            var parser = new Jhu.Graywulf.SqlParser.SqlParser();
            var tn = (Jhu.Graywulf.SqlParser.TableOrViewName)parser.Execute(new Jhu.Graywulf.SqlParser.TableOrViewName(), token);

            tr = tn.TableReference;
            tr.SubstituteDefaults(context.SchemaManager, context.MyDBDataset.Name);
        }

        public static void ParseTableName(FederationContext context, string token, out string schemaName, out string tableName)
        {
            TableReference tr;
            ParseTableName(context, token, out tr);
            schemaName = tr.SchemaName;
            tableName = tr.DatabaseObjectName;
        }

        public static void ParseTableName(FederationContext context, string token, out TableOrView table)
        {
            TableReference tr;
            ParseTableName(context, token, out tr);

            var ds = context.SchemaManager.Datasets[tr.DatasetName];
            table = (TableOrView)ds.GetObject(tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName);
        }
    }
}
