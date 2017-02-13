using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Schema
{
    public class Quantity
    {
        private List<string> parts;

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
            var parts = quantityString.Split(new char[] { ';' , ' ' },StringSplitOptions.RemoveEmptyEntries);

            quantity.parts.AddRange(parts);

            return quantity;
        }

        public override string ToString()
        {
            return string.Join("; ", parts);
        }
    }
}
