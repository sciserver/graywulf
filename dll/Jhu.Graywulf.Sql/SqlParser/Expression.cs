using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Expression
    {
        /// <summary>
        /// Returns true if the expression is a single column name
        /// </summary>
        /// <returns></returns>
        public bool IsSingleColumn
        {
            get
            {
                if (Stack.Count == 1)
                {
                    var av = FindDescendant<AnyVariable>();
                    if (av != null)
                    {
                        var ci = av.FindDescendant<ColumnIdentifier>();
                        if (ci != null)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool IsConstantNumber
        {
            get
            {
                if (Stack.Count == 1)
                {
                    var av = FindDescendant<Number>();
                    if (av != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }



        public static Expression Create(ColumnIdentifier ci)
        {
            var nex = new Expression();
            var avr = new AnyVariable();

            avr.Stack.AddLast(ci);
            nex.Stack.AddLast(avr);

            return nex;
        }

        public static Expression Create(Variable var)
        {
            var nex = new Expression();
            var avr = new AnyVariable();

            avr.Stack.AddLast(var);
            nex.Stack.AddLast(avr);

            return nex;
        }

        public static Expression Create(FunctionCall fun)
        {
            var nex = new Expression();
            nex.Stack.AddLast(fun);
            return nex;
        }

        public static Expression Create(UdtFunctionCall fun)
        {
            var nex = new Expression();
            nex.Stack.AddLast(fun);
            return nex;
        }

        public static Expression CreateNumber(string number)
        {
            var nex = new Expression();
            var num = Number.Create(number);
            nex.Stack.AddLast(num);
            return nex;
        }
    }
}
