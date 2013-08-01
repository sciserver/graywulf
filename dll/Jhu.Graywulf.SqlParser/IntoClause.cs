using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class IntoClause : ITableReference
    {
        TableOrViewName tableName;

        public TableReference TableReference
        {
            get { return tableName.TableReference; }
            set { tableName.TableReference = value; }
        }

        /* TODO: delete
        public Token TableNameToken
        {
            get { return tableName.Token; }
        }*/

        public IntoClause()
        {
        }

        private void InitializeMembers()
        {
            this.tableName = null;
        }

        public override Node Interpret()
        {
            this.tableName = FindDescendant<TableOrViewName>();

            return base.Interpret();
        }
    }
}
