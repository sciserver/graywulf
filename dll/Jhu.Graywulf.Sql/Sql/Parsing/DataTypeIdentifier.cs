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
        
        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
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
            this.dataTypeReference = DataTypeReference.Interpret(this);
        }

        public static DataTypeIdentifier Create(DataTypeReference dr)
        {
            throw new NotImplementedException();
        }
    }
}
