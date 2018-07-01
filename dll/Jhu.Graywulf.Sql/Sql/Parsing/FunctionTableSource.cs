using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionTableSource : ITableSource
    {
        private TableReference tableReference;
        private string uniqueKey;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public TableValuedFunctionCall FunctionCall
        {
            get { return FindDescendant<TableValuedFunctionCall>(); }
        }

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
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
            var fts = new FunctionTableSource();

            fts.Stack.AddLast(functionCall);
            fts.Stack.AddLast(Whitespace.Create());
            fts.Stack.AddLast(Keyword.Create("AS"));
            fts.Stack.AddLast(Whitespace.Create());
            fts.Stack.AddLast(TableAlias.Create(tableAlias));

            functionCall.TableReference.Alias = tableAlias;

            return fts;
        }

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
