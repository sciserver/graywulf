using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class PropertyReference : ReferenceBase
    {
        #region Property storage variables

        private string propertyName;

        #endregion
        #region Properties

        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        public override string UniqueName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        #endregion
        #region Constructors and initializers

        public PropertyReference()
        {
            InitializeMembers();
        }

        public PropertyReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }

        public PropertyReference(PropertyReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.propertyName = null;
        }

        private void CopyMembers(PropertyReference old)
        {
            this.propertyName = old.propertyName;
        }

        public override object Clone()
        {
            return new PropertyReference(this);
        }

        #endregion

        public static PropertyReference Interpret(PropertyName pn)
        {
            var pr = new PropertyReference(pn)
            {
                PropertyName = RemoveIdentifierQuotes(pn.Value)
            };

            return pr;
        }
    }
}
