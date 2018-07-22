using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtStaticPropertyAccess : IDataTypeReference, IPropertyReference
    {
        public DataTypeIdentifier DataTypeIdentifier
        {
            get { return Parent.FindDescendant<DataTypeIdentifier>(); }
        }

        public DataTypeReference DataTypeReference
        {
            get { return DataTypeIdentifier.DataTypeReference; }
            set { DataTypeIdentifier.DataTypeReference = value; }
        }

        public static UdtStaticPropertyAccess Create(DataTypeReference dr, PropertyReference pr)
        {
            var pp = new UdtStaticPropertyAccess();
            pp.Stack.AddLast(DataTypeIdentifier.Create(dr));
            pp.Stack.AddLast(DoubleColon.Create());
            pp.Stack.AddLast(PropertyName.Create(pr));

            return pp;
        }
    }
}
