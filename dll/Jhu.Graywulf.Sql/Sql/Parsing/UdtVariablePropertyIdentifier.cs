using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtVariablePropertyIdentifier : IVariableReference, IPropertyReference
    {
        public UserVariable UserVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public PropertyName PropertyName
        {
            get { return FindDescendant<PropertyName>(); }
        }

        public VariableReference VariableReference
        {
            get { return UserVariable.VariableReference; }
            set { UserVariable.VariableReference = value; }
        }
    }
}
