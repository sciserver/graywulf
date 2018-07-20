using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class PropertyName : IPropertyReference
    {
        private PropertyReference propertyReference;

        public PropertyReference PropertyReference
        {
            get { return propertyReference; }
            set { propertyReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.propertyReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (PropertyName)other;
            this.propertyReference = old.propertyReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.propertyReference = PropertyReference.Interpret(this);
        }

        public static PropertyName Create(string propertyName)
        {
            var id = Identifier.Create(propertyName);
            var pn = new PropertyName();
            pn.Stack.AddLast(id);
            return pn;
        }

        public static PropertyName Create(PropertyReference propertyReference)
        {
            var pn = Create(propertyReference.PropertyName);
            pn.propertyReference = propertyReference;
            return pn;
        }
    }
}
