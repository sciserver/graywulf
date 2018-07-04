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
            throw new NotImplementedException();

            /*
            var fpi = new MultiPartIdentifier();

            fpi.Append<NamePart4>(namePart4);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart3>(namePart3);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;
            */
        }

        public static MultiPartIdentifier Create(string namePart3, string namePart2, string namePart1)
        {

            throw new NotImplementedException();
            /*
            var fpi = new MultiPartIdentifier();

            fpi.Append<NamePart3>(namePart3);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;*/
        }

        public static MultiPartIdentifier Create(string namePart2, string namePart1)
        {
            throw new NotImplementedException();

            /*var fpi = new MultiPartIdentifier();

            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;*/
        }

        public static MultiPartIdentifier Create(string namePart1)
        {
            throw new NotImplementedException();
            /*var fpi = new MultiPartIdentifier();

            fpi.Append<NamePart1>(namePart1);

            return fpi;*/
        }

        public void Append<T>(string namePart)
            where T : Node, new()
        {
            if (namePart != null)
            {
                var np = new T();
                np.Stack.AddLast(Identifier.Create(namePart));
                this.Stack.AddLast(np);
            }
        }

        public override void Interpret()
        {
            base.Interpret();

            parts = FindDescendant<NamePartList>().EnumerateDescendants<Identifier>().ToArray();
        }
    }
}
