using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtMemberList
    {
        public static UdtMemberList Create(UdtMethodCall mc)
        {
            return new UdtMemberList(mc);
        }
    }
}
