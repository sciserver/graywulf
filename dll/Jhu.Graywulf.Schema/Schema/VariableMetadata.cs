using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class VariableMetadata : Metadata, ICloneable
    {
        [NonSerialized]
        private string @class;

        [NonSerialized]
        private Quantity quantity;

        [NonSerialized]
        private Unit unit;

        [NonSerialized]
        private string format;

        [DataMember]
        public string Class
        {
            get { return @class; }
            set { @class = value; }
        }

        [DataMember]
        public Quantity Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public Unit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        [DataMember]
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public VariableMetadata()
        {
            InitializeMembers();
        }

        public VariableMetadata(VariableMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.@class = String.Empty;
            this.quantity = new Quantity();
            this.unit = new Unit();
            this.format = "{0}";
        }

        private void CopyMembers(VariableMetadata old)
        {
            this.@class = old.@class;
            this.quantity = old.quantity;
            this.unit = old.unit;
            this.format = old.format;
        }

        public object Clone()
        {
            return new VariableMetadata(this);
        }
    }
}
