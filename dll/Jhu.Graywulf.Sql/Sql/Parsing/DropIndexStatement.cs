using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DropIndexStatement :  ITableReference
    {
        public TableOrViewIdentifier TargetTable
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return TargetTable.DatabaseObjectReference; }
        }

        public TableReference TableReference
        {
            get { return TargetTable.TableReference; }
            set { TargetTable.TableReference = value; }
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
