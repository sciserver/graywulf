﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Web.Api.Util
{
    public static class SqlParser
    {
        public static bool TryParseTableName(FederationContext context, string dataset, string token, out TableReference tr)
        {
            // TODO: review this because it hasn't been tested, just rewritten to compile the whole stack

            if (token != null)
            {
                try
                {
                    var qf = Jhu.Graywulf.Sql.Jobs.Query.QueryFactory.Create(context.Federation);
                    var parser = qf.CreateParser();
                    var nr = qf.CreateNameResolver();

                    var tn = parser.Execute<Jhu.Graywulf.Sql.Parsing.TableOrViewIdentifier>(token);
                    nr.SubstituteSourceTableDefaults(null, tn.TableReference, true);
                    tr = tn.TableReference;

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
