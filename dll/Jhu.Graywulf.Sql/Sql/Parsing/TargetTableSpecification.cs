using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TargetTableSpecification : ITableReference
    {
        private string uniqueKey;
        private TableOrViewIdentifier tableName;
        private UserVariable variable;
        private TableReference tableReference;

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
            get
            {
                if (tableName != null)
                {
                    return tableName.TableReference;
                }
                else
                {
                    return tableReference;
                }
            }
            set
            {
                if (tableName != null)
                {
                    tableName.TableReference = value;
                }
                else
                {
                    tableReference = value;
                }
            }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get
            {
                if (tableName != null)
                {
                    return tableName.DatabaseObjectReference;
                }
                else
                {
                    return null;
                }
            }
        }

        public TableOrViewIdentifier TableName
        {
            get { return tableName; }
        }

        public UserVariable Variable
        {
            get { return variable; }
        }

        public override void Interpret()
        {
            base.Interpret();

            this.tableName = FindDescendantRecursive<TableOrViewIdentifier>();
            this.variable = FindDescendantRecursive<UserVariable>();
        }

        public override IEnumerable<TableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
