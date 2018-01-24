using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.LogicalExpressions;

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    public class RemoteQueryGenerator
    {
        // TODO: we could actually implement column type mapping right here

        public Dictionary<string, string> Execute(QueryDetails query, ColumnContext columnContext, int top)
        {
            var res = new Dictionary<string, string>();

            // Normalize all search conditions
            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(query.ParsingTree);

            // Loop through all tables
            foreach (var key in query.SourceTables.Keys)
            {
                var columns = new Dictionary<string, ColumnReference>();
                DatabaseObject table = null;
                TableReference ntr = null;
                CodeGeneratorBase cg = null;
                
                // Loop through all references to the table
                foreach (var tr in query.SourceTables[key])
                {
                    if (ntr == null)
                    {
                        table = tr.DatabaseObject;
                        
                        ntr = new TableReference(tr);
                        ntr.Alias = null;

                        cg = CodeGeneratorFactory.CreateCodeGenerator(table.Dataset);
                        InitializeCodeGenerator(cg);
                    }

                    // Remap all table reference to the first one
                    // This is to prevent different aliases etc.
                    cg.TableReferenceMap.Add(tr, ntr);

                    // Collect columns that will be returned
                    foreach (var c in tr.FilterColumnReferences(columnContext))
                    {
                        if (!columns.ContainsKey(c.ColumnName))
                        {
                            columns.Add(c.ColumnName, new ColumnReference(c));
                        }
                    }
                }

                // Generate select list
                var columnlist = cg.CreateColumnListGenerator();
                columnlist.ListType = ColumnListType.SelectWithOriginalNameNoAlias;
                columnlist.TableAlias = String.Empty;
                columnlist.Columns.AddRange(columns.Values);

                // Generate where clause
                var where = scn.GenerateWherePredicatesSpecificToTable(query.SourceTables[key]);

                var select = cg.GenerateMostRestrictiveTableQuery(
                    cg.GetResolvedTableName(table),
                    null,
                    columnlist.Execute(),
                    cg.Execute(where),
                    top);

                res.Add(key, select);
            }

            return res;
        }

        private void InitializeCodeGenerator(CodeGeneratorBase cg)
        {
            cg.TableNameRendering = NameRendering.FullyQualified;
            cg.TableAliasRendering = AliasRendering.Never;
            cg.ColumnNameRendering = NameRendering.IdentifierOnly;
            cg.ColumnAliasRendering = AliasRendering.Never;
            cg.FunctionNameRendering = NameRendering.FullyQualified;
        }
    }
}
