using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FourPartIdentifier
    {
        public string NamePart4
        {
            get { return FindDescendant<NamePart4>()?.Value; }
        }

        public string NamePart3
        {
            get { return FindDescendant<NamePart3>()?.Value; }
        }

        public string NamePart2
        {
            get { return FindDescendant<NamePart2>()?.Value; }
        }

        public string NamePart1
        {
            get { return FindDescendant<NamePart1>()?.Value; }
        }

        public static FourPartIdentifier Create(string namePart4, string namePart3, string namePart2, string namePart1)
        {
            var fpi = new FourPartIdentifier();

            fpi.Append<NamePart4>(namePart4);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart3>(namePart3);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;
        }

        public static FourPartIdentifier Create(string namePart3, string namePart2, string namePart1)
        {
            var fpi = new FourPartIdentifier();

            fpi.Append<NamePart3>(namePart3);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;
        }

        public static FourPartIdentifier Create(string namePart2, string namePart1)
        {
            var fpi = new FourPartIdentifier();

            fpi.Append<NamePart2>(namePart2);
            fpi.Stack.AddLast(Dot.Create());
            fpi.Append<NamePart1>(namePart1);

            return fpi;
        }

        public static FourPartIdentifier Create(string namePart1)
        {
            var fpi = new FourPartIdentifier();

            fpi.Append<NamePart1>(namePart1);

            return fpi;
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
    }
}
