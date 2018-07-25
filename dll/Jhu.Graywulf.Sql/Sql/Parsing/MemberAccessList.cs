using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MemberAccessList
    {
        public static MemberAccessList Create(Node[] nodes)
        {
            MemberAccessList mal = null;
            MemberAccessList cmal = null;

            for (int i = 0; i < nodes.Length; i++)
            {
                var nmal = new MemberAccessList();
                nmal.Stack.AddLast(MemberAccessOperator.Create());
                nmal.Stack.AddLast(nodes[i]);

                if (mal == null)
                {
                    mal = cmal = nmal;
                }

                cmal.Stack.AddLast(nmal);
                cmal = nmal;
            }

            return mal;
        }
    }
}
