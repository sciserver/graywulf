using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class BinaryOperator
    {
        public override int Precedence
        {
            get
            {
                switch (Stack.First.Value.Value)
                {
                    case "*":
                    case "/":
                    case "%":
                    case "&":
                    case "|":
                    case "^":
                        return 2;

                    case "+":
                    case "-":
                        return 3;
                    
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
