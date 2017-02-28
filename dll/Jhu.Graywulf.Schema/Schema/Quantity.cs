using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Quantity
    {
        [NonSerialized]
        private List<string> parts;

        [IgnoreDataMember]
        public List<string> Parts
        {
            get { return parts; }
        }

        public Quantity()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            parts = new List<string>();
        }

        public static Quantity Parse(string quantityString)
        {
            var quantity = new Quantity();
            var parts = quantityString.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            quantity.parts.AddRange(parts);

            return quantity;
        }

        public override string ToString()
        {
            return string.Join("; ", parts);
        }
    }
}
