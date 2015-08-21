using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableOrViewName : ITableReference
    {
        private TableReference tableReference;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.tableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (TableOrViewName)other;

            this.tableReference = old.tableReference;
        }

        public static TableOrViewName Create(TableReference tr)
        {
            var res = new TableOrViewName();
            res.tableReference = tr;

            if (!String.IsNullOrWhiteSpace(tr.DatasetName))
            {
                res.Stack.AddLast(DatasetName.Create(tr.DatasetName));
                res.Stack.AddLast(Colon.Create());
            }

            // Here one would add the database name but we omit that since we
            // primarily refer to databases via datasets

            if (!String.IsNullOrWhiteSpace(tr.SchemaName))
            {
                res.Stack.AddLast(SchemaName.Create(tr.SchemaName));
                res.Stack.AddLast(Dot.Create());
            }

            res.Stack.AddLast(TableName.Create(tr.DatabaseObjectName));

            return res;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableReference = new TableReference(this);
        }
    }
}
