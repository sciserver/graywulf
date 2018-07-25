using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MemberAccessOperator
    {
        public override int Precedence
        {
            get { return 1; }
        }

        public override bool IsLeftAssociative
        {
            get { return true; }
        }

        public static MemberAccessOperator Create()
        {
            var ma = new MemberAccessOperator(Dot.Create());
            return ma;
        }
    }
}
