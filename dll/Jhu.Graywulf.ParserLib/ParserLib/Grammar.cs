using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// This dummy class is used to define a grammar
    /// </summary>
    public class Grammar
    {
        

        // ---

        public delegate string Symbol();
        public delegate string Terminal();
        public delegate string Whitespace();
        public delegate string Comment();
        public delegate Expression Rule();

        public static Expression Sequence(params Expression[] expression)
        {
            return null;
        }

        public static Expression Must(params Expression[] expressions)
        {
            return null;
        }

        public static Expression May(Expression expressions)
        {
            return null;
        }

        public static Expression Literal(string literal)
        {
            return null;
        }

        public static Expression Keyword(string keyword)
        {
            return null;
        }
    }
}
