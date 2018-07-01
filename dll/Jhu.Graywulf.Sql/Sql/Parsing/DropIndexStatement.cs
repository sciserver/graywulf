using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DropIndexStatement : IStatement, ITableReference
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Schema; }
        }

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

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
