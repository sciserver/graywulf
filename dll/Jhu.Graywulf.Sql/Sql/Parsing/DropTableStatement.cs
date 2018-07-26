using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DropTableStatement : ITableReference, ISourceTableProvider, ITargetTableProvider
    {
        public TableOrViewIdentifier TargetTable
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        public TableReference TargetTableReference
        {
            get { return TargetTable.TableReference; }
        }

        public TableReference TableReference
        {
            get { return TargetTable.TableReference; }
            set { TargetTable.TableReference = value; }
        }
    }
}
