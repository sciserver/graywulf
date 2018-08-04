using System;
using System.Collections.Generic;
using System.Linq;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SimpleTableSource
    {
        private string uniqueKey;

        public TableOrViewIdentifier TableOrViewIdentifier
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        public TableAlias Alias
        {
            get { return FindDescendant<TableAlias>(); }
        }
        
        public override TableReference TableReference
        {
            get { return TableOrViewIdentifier.TableReference; }
            set { TableOrViewIdentifier.TableReference = value; }
        }

        public override bool IsSubquery
        {
            get { return false; }
        }

        public override bool IsMultiTable
        {
            get { return false; }
        }

        public override string UniqueKey
        { 
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public static SimpleTableSource Create(TableReference tr)
        {
            var ts = new SimpleTableSource();
            var name = TableOrViewIdentifier.Create(tr);

            ts.Stack.AddLast(name);

            if (!String.IsNullOrWhiteSpace(tr.Alias))
            {
                ts.Stack.AddLast(Whitespace.Create());
                ts.Stack.AddLast(Keyword.Create("AS"));
                ts.Stack.AddLast(Whitespace.Create());
                ts.Stack.AddLast(TableAlias.Create(tr.Alias));
            }

            return ts;
        }

        public override void Interpret()
        {
            base.Interpret();

            TableReference = TableReference.Interpret(this);
        }
    }
}
