using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MultiPartIdentifier
    {
        private Identifier[] parts;

        public int PartCount
        {
            get { return parts?.Length ?? 0; }
        }
        
        public string NamePart3
        {
            get
            {
                return (parts.Length > 2) ? parts[parts.Length - 3].Value : null;
            }
        }

        public string NamePart2
        {
            get
            {
                return (parts.Length > 1) ? parts[parts.Length - 2].Value : null;
            }
        }

        public string NamePart1
        {
            get
            {
                return (parts.Length > 0) ? parts[parts.Length - 1].Value : null;
            }
        }

        public Identifier GetPart(int index)
        {
            return parts?[index];
        }

        public static MultiPartIdentifier Create(string namePart4, string namePart3, string namePart2, string namePart1)
        {
            var fpi = new MultiPartIdentifier();

            var npl = fpi.Append(null, namePart4);
            npl = fpi.Append(npl, namePart3);
            npl = fpi.Append(npl, namePart2);
            npl = fpi.Append(npl, namePart1);

            return fpi;
        }

        public static MultiPartIdentifier Create(string namePart3, string namePart2, string namePart1)
        {
            var fpi = new MultiPartIdentifier();

            var npl = fpi.Append(null, namePart3);
            npl = fpi.Append(npl, namePart2);
            npl = fpi.Append(npl, namePart1);

            return fpi;
        }

        public static MultiPartIdentifier Create(string namePart2, string namePart1)
        {
            var fpi = new MultiPartIdentifier();
            
            var npl = fpi.Append(null, namePart2);
            npl = fpi.Append(npl, namePart1);

            return fpi;
        }

        public static MultiPartIdentifier Create(string namePart1)
        {
            var fpi = new MultiPartIdentifier();

            var npl = fpi.Append(null, namePart1);

            return fpi;
        }

        public NamePartList Append(NamePartList npl, string identifier)
        {
            var nn = new NamePartList();

            if (npl == null)
            {
                Stack.AddLast(nn);
            }
            else
            {
                npl.Stack.AddLast(Dot.Create());
                npl.Stack.AddLast(nn);
            }

            nn.Stack.AddLast(Identifier.Create(identifier));

            return nn;
        }

        public override void Interpret()
        {
            base.Interpret();

            parts = FindDescendant<NamePartList>().EnumerateDescendants<Identifier>().ToArray();
        }
    }
}
