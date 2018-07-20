using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtPropertyAccess : IPropertyReference
    {
        public PropertyName PropertyName
        {
            get { return FindDescendant<PropertyName>(); }
        }

        public override PropertyReference PropertyReference
        {
            get { return PropertyName.PropertyReference; }
            set { PropertyName.PropertyReference = value; }
        }

        public static UdtPropertyAccess Create(PropertyReference pr)
        {
            var pp = new UdtPropertyAccess();
            pp.Stack.AddLast(Dot.Create());
            pp.Stack.AddLast(PropertyName.Create(pr));

            return pp;
        }
    }
}
