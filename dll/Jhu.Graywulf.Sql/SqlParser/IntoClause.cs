using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class IntoClause : ITableReference
    {
        public TableOrViewName TableName
        {
            get { return FindDescendant<TableOrViewName>(); }
        }

        public TableReference TableReference
        {
            get { return TableName.TableReference; }
            set { TableName.TableReference = value; }
        }
    }
}
