using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public class ComputedTableSource : Jhu.Graywulf.Parsing.Node, ITableSource, ICloneable
    {
        private TableReference tableReference;

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return tableReference; }
        }

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
        {
            get { return false; }
        }

        public ComputedTableSource()
            : base()
        {
        }

        public ComputedTableSource(ComputedTableSource old)
            : base(old)
        {
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (ComputedTableSource)other;

            this.tableReference = old.tableReference;
        }

        public override object Clone()
        {
            return new ComputedTableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            throw new NotImplementedException();
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
