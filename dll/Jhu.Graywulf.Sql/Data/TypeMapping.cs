using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Data
{
    public class TypeMapping : ICloneable
    {
        public delegate object TypeMappingDelegate(object value);

        #region Private member variables

        private Type from;
        private Type to;
        private TypeMappingDelegate mapping;

        #endregion
        #region Properties

        public Type From
        {
            get { return from; }
            set { from = value; }
        }

        public Type To
        {
            get { return to; }
            set { to = value; }
        }

        public TypeMappingDelegate Mapping
        {
            get { return mapping; }
            set { mapping = value; }
        }

        #endregion
        #region Constructors and initializers

        public TypeMapping()
        {
            InitializeMembers();
        }

        public TypeMapping(TypeMapping old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.from = null;
            this.to = null;
            this.mapping = null;
        }

        private void CopyMembers(TypeMapping old)
        {
            this.from = old.from;
            this.to = old.to;
            this.mapping = old.mapping;
        }

        public object Clone()
        {
            return new TypeMapping(this);
        }

        #endregion
    }
}
