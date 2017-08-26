using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.SqlParser
{
    public partial class QueryOperator
    {
        public static QueryOperator CreateUnion()
        {
            var qo = new QueryOperator();

            qo.Stack.AddLast(Keyword.Create("UNION"));

            return qo;
        }

        public static QueryOperator CreateUnionAll()
        {
            var qo = new QueryOperator();

            qo.Stack.AddLast(Keyword.Create("UNION"));
            qo.Stack.AddLast(Whitespace.Create());
            qo.Stack.AddLast(Keyword.Create("ALL"));
            
            return qo;
        }

        public static QueryOperator CreateIntersect()
        {
            var qo = new QueryOperator();

            qo.Stack.AddLast(Keyword.Create("INTERSECT"));

            return qo;
        }

        public static QueryOperator CreateExcept()
        {
            var qo = new QueryOperator();

            qo.Stack.AddLast(Keyword.Create("EXCEPT"));

            return qo;
        }
    }
}
