using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SelectList
    {
        public IEnumerable<ColumnExpression> EnumerateColumnExpressions()
        {
            return EnumerateDescendantsRecursive<ColumnExpression>();
        }

        public static SelectList CreateStar()
        {
            var sl = new SelectList();
            sl.Stack.AddLast(ColumnExpression.CreateStar());
            return sl;
        }

        public static SelectList CreateStar(TableReference tableReference)
        {
            var sl = new SelectList();
            sl.Stack.AddLast(ColumnExpression.CreateStar(tableReference));
            return sl;
        }

        public static SelectList Create(QuerySpecification querySpecification)
        {
            SelectList root = null;
            SelectList last = null;

            foreach (var tr in querySpecification.SourceTableReferences.Values)
            {
                Create(ref root, ref last, tr);
            }

            return root;
        }

        public static SelectList Create(TableReference tableReference)
        {
            SelectList root = null;
            SelectList last = null;

            Create(ref root, ref last, tableReference);

            return root;
        }

        public static SelectList Create(ColumnReference cr)
        {
            var ncr = new ColumnReference(cr);

            // Columns from subquery expressions have no names, only aliases
            if (ncr.ColumnName == null)
            {
                ncr.ColumnName = cr.ColumnAlias;
                ncr.ColumnAlias = null;
            }
            
            var nsl = new SelectList();
            var nce = new ColumnExpression();
            nce.ColumnReference = ncr;
            nsl.Stack.AddLast(nce);

            var nex = new Expression();
            nex.Stack.AddLast(ColumnIdentifier.Create(ncr));
            nce.Stack.AddLast(nex);

            return nsl;
        }

        private static void Create(ref SelectList root, ref SelectList last, TableReference tableReference)
        {
            foreach (var cr in tableReference.ColumnReferences)
            {
                // Create new expression
                var nsl = SelectList.Create(cr);

                // If root is uninitialized, initialize now
                if (root == null)
                {
                    root = nsl;
                }

                // Append to list if not the first
                if (last != null)
                {
                    last.Append(nsl);
                }

                last = nsl;
            }

            if (root == null)
            {
                throw new InvalidOperationException();
            }
        }

        public void Append(SelectList last)
        {
            SelectList ll, sl;

            ll = sl = this;
            while ((ll = sl.FindDescendant<SelectList>()) != null)
            {
                sl = ll;
            }

            sl.Stack.AddLast(Comma.Create());
            sl.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.Create()));
            sl.Stack.AddLast(last);
        }

        public void Replace(SelectList last)
        {
            var n = Stack.First;

            while (n != null)
            {
                if (n.Value is SelectList)
                {
                    n.Value = last;
                }

                n = n.Next;
            }
        }
    }
}
