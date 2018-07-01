using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtStaticPropertyIdentifier : IDataTypeReference, IPropertyReference
    {
        public DataTypeIdentifier DataTypeIdentifier
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }

        public PropertyName PropertyName
        {
            get { return FindDescendant<PropertyName>(); }
        }

        public DataTypeReference DataTypeReference
        {
            get { return DataTypeIdentifier.DataTypeReference; }
            set { DataTypeIdentifier.DataTypeReference = value; }
        }
    }
}
