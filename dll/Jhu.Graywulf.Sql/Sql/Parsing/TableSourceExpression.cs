using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableSourceExpression
    {
        public IEnumerable<TableSource> EnumerateSourceTables(bool recursive)
        {
            var node = (Node)this;

            while (node != null)
            {
                var ts = node.FindDescendant<TableSourceSpecification>();
                yield return ts.SpecificTableSource;

                // Enumerate recursively, if necessary
                if (recursive && ts.SpecificTableSource.IsSubquery)
                {
                    foreach (var tts in ts.SpecificTableSource.EnumerateSubqueryTableSources(recursive))
                    {
                        yield return tts;
                    }
                }

                // Certain table sources might return additional table sources
                // This is not standard SQL, it is used with special extensions
                // such as the XMATCH syntax
                if (ts.SpecificTableSource.IsMultiTable)
                {
                    foreach (var mts in ts.SpecificTableSource.EnumerateMultiTableSources())
                    {
                        yield return mts;

                        // Enumerate recursively, if necessary
                        if (recursive && mts.IsSubquery)
                        {
                            foreach (var tts in mts.EnumerateSubqueryTableSources(recursive))
                            {
                                yield return tts;
                            }
                        }
                    }
                }

                node = node.FindDescendant<JoinedTable>();
            }
        }

        public static TableSourceExpression Create(TableSourceSpecification tableSource)
        {
            return Create(tableSource, null);
        }

        public static TableSourceExpression Create(TableSourceSpecification tableSource, JoinedTable joinedTable)
        {
            var tse = new TableSourceExpression();

            tse.Stack.AddLast(tableSource);

            if (joinedTable != null)
            {
                tse.Stack.AddLast(Whitespace.CreateNewLine());
                tse.Stack.AddLast(joinedTable);
            }

            return tse;
        }
    }
}
