using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DropIndexStatement :  ITableReference, IIndexReference, ISourceTableProvider, ITargetTableProvider
    {
        public TableOrViewIdentifier TargetTable
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        public TableReference TargetTableReference
        {
            get { return TargetTable.TableReference; }
        }

        public IndexName IndexName
        {
            get { return FindDescendant<IndexName>(); }
        }

        public TableReference TableReference
        {
            get { return TargetTable.TableReference; }
            set { TargetTable.TableReference = value; }
        }

        public IndexReference IndexReference
        {
            get { return IndexName.IndexReference; }
            set { IndexName.IndexReference = value; }
        }
    }
}
