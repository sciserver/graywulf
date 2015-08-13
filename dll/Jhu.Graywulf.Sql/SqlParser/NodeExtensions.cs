using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public static class NodeExtensions
    {
        // Ezek már name resolution után futnak csak, mert a tablereference-eket
        // referencia és nem érték alapján hasonlítják össze
        // Át kell tenni a name resolverbe, mert nem ide valók ???
        public static IEnumerable<TableReference> EnumerateTableReferences(this Node node)
        {
            List<TableReference> tableReferences = new List<TableReference>();

            FindTableReferences(node, tableReferences);

            return tableReferences;
        }

        private static void FindTableReferences(Node node, List<TableReference> tableReferences)
        {
            foreach (object o in node.Nodes)
            {
                if (o is ColumnIdentifier)
                {
                    ColumnIdentifier ci = (ColumnIdentifier)o;

                    if (!tableReferences.Contains(ci.TableReference))
                    {
                        tableReferences.Add(ci.TableReference);
                    }
                }

                if (o is Node && !(o is Subquery))
                {
                    FindTableReferences((Node)o, tableReferences);
                }
            }
        }
    }
}
