using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionTableSource : IFunctionReference
    {
        private TableReference tableReference;
        private string uniqueKey;

        public FunctionReference FunctionReference
        {
            get { return FunctionCall.FunctionReference; }
            set { FunctionCall.FunctionReference = value; }
        }
        
        public override TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public override string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public TableAlias Alias
        {
            get { return FindDescendant<TableAlias>(); }
        }

        public TableValuedFunctionCall FunctionCall
        {
            get { return FindDescendant<TableValuedFunctionCall>(); }
        }

        public override bool IsSubquery
        {
            get { return false; }
        }

        public override bool IsMultiTable
        {
            get { return false; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (FunctionTableSource)other;

            this.tableReference = old.tableReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableReference = TableReference.Interpret(this);
        }

        public static FunctionTableSource Create(TableValuedFunctionCall functionCall, string tableAlias)
        {
            throw new NotImplementedException();

            /*
            var fts = new FunctionTableSource();

            fts.Stack.AddLast(functionCall);
            fts.Stack.AddLast(Whitespace.Create());
            fts.Stack.AddLast(Keyword.Create("AS"));
            fts.Stack.AddLast(Whitespace.Create());
            fts.Stack.AddLast(TableAlias.Create(tableAlias));

            functionCall.TableReference.Alias = tableAlias;

            return fts;
            */
        }
    }
}
