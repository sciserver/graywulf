using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.Schema
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

        [IgnoreDataMember]
        public Quantity Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember(Name = "Quantity")]
        public string Quantity_ForXml
        {
            get { return quantity.ToString(); }
            set { quantity = Quantity.Parse(value); }
        }

        [IgnoreDataMember]
        public Unit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        [DataMember(Name = "Unit")]
        public string Unit_ForXml
        {
            get { return unit.ToString(); }
            set { unit = Unit.Parse(value); }
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
