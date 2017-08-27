using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SelectList
    {
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
            // Create new expression
            var nsl = new SelectList();

            var nce = new ColumnExpression();
            nce.ColumnReference = new ColumnReference(cr);
            nsl.Stack.AddLast(nce);

            var nex = new Expression();
            nce.Stack.AddLast(nex);

            var nav = new AnyVariable();
            nex.Stack.AddLast(nav);

            nav.Stack.AddLast(ColumnIdentifier.Create(new ColumnReference(cr)));

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

        public SelectList SubstituteStars()
        {
            var ce = FindDescendant<ColumnExpression>();
            var subsl = FindDescendant<SelectList>();

            SelectList sl = null;
            if (ce.ColumnReference.IsStar)
            {
                // Build select list from the column list of
                // the referenced table, then replace current node
                if (ce.TableReference.IsUndefined)
                {
                    sl = SelectList.Create(FindAscendant<QuerySpecification>());
                }
                else
                {
                    sl = SelectList.Create(ce.TableReference);
                }

                if (subsl != null)
                {
                    sl.Append(subsl.SubstituteStars());
                }

                return sl;
            }
            else
            {
                if (subsl != null)
                {
                    Replace(subsl.SubstituteStars());
                }

                return this;
            }
        }
    }
}
