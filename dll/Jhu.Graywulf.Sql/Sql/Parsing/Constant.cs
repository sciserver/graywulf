using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Constant
    {
        public static Constant CreateNull()
        {
            var c = new Constant();
            c.Stack.AddLast(Null.Create());

            return c;
        }

        public static Constant CreateNumeric(string value)
        {
            var num = NumericConstant.Create(value);
            var con = new Constant();
            con.Stack.AddLast(num);
            return con;
        }

        public static Constant CreateNumeric(int value)
        {
            var num = NumericConstant.Create(value);
            var con = new Constant();
            con.Stack.AddLast(num);
            return con;
        }

        public static Constant CreateNumeric(double value)
        {
            var num = NumericConstant.Create(value);
            var con = new Constant();
            con.Stack.AddLast(num);
            return con;
        }

        public static Constant CreateString(string value)
        {
            var str = StringConstant.Create(value);
            var con = new Constant();
            con.Stack.AddLast(str);
            return con;
        }
    }
}
