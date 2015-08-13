using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Whitespace
    {
        public static Whitespace Create()
        {
            var ws = new Whitespace();
            ws.Value = " ";

            return ws;
        }

        public static Whitespace CreateNewLine()
        {
            var ws = new Whitespace();
            ws.Value = "\r\n";

            return ws;
        }
    }
}
