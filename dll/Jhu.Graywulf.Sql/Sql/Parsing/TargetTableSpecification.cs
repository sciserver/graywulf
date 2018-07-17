using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TargetTableSpecification : ITableReference, IVariableReference
    {
        private string uniqueKey;

        public TableOrViewIdentifier TableOrViewIdentifier
        {
            get { return FindDescendantRecursive<TableOrViewIdentifier>(); }
        }

        public UserVariable Variable
        {
            get { return FindDescendantRecursive<UserVariable>(); }
        }

        public override bool IsSubquery
        {
            get { return false; }
        }

        public override bool IsMultiTable
        {
            get { return false; }
        }

        public override string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public override TableReference TableReference
        {
            get { return TableOrViewIdentifier?.TableReference; }
            set
            {
                var table = TableOrViewIdentifier;
                if (table != null)
                {
                    table.TableReference = value;
                }
            }
        }

        public VariableReference VariableReference
        {
            get { return Variable?.VariableReference; }
            set
            {
                var variable = Variable;
                if (variable != null)
                {
                    variable.VariableReference = value;
                }
            }
        }

        public override void Interpret()
        {
            base.Interpret();

            if (TableReference != null)
            {
                TableReference = TableReference.Interpret(this);
            }

            if (VariableReference != null)
            {
                VariableReference = VariableReference.Interpret(this);
            }
        }
    }
}
