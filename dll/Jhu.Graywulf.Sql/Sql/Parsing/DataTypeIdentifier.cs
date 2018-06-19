using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DataTypeIdentifier : IDataTypeReference
    {
        private DataTypeReference dataTypeReference;

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return dataTypeReference; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
        }

        public UdtIdentifier UdtIdentifier
        {
            get { return FindDescendant<UdtIdentifier>(); }
        }

        public SystemDataTypeIdentifier SystemDataTypeIdentifier
        {
            get { return FindDescendant<SystemDataTypeIdentifier>(); }
        }

        public bool IsNullable
        {
            get
            {
                // TODO: update this becaues there can be other literals in the future
                var k = FindDescendantRecursive<Jhu.Graywulf.Parsing.Literal>();

                if (k != null && SqlParser.ComparerInstance.Compare("not", k.Value) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.dataTypeReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (DataTypeIdentifier)other;
            this.dataTypeReference = old.dataTypeReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.dataTypeReference = new DataTypeReference(this);
        }
    }
}
