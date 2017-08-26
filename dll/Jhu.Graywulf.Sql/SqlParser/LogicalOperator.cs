using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.SqlParser
{
    public partial class LogicalOperator
    {
        public bool IsOr
        {
            get
            {
                return SqlParser.ComparerInstance.Compare(Value, "OR") == 0; 
            }
        }

        public bool IsAnd
        {
            get
            {
                return SqlParser.ComparerInstance.Compare(Value, "AND") == 0; 
            }
        }

        public static LogicalOperator Create(string keyword)
        {
            var lop = new LogicalOperator();

            lop.Stack.AddLast(Keyword.Create(keyword));

            return lop;
        }

        public static LogicalOperator CreateOr()
        {
            return Create("OR");
        }

        public static LogicalOperator CreateAnd()
        {
            return Create("AND");
        }

        public LogicalOperator CreateInverse()
        {
            if (IsOr)
            {
                return CreateAnd();
            }
            else
            {
                return CreateOr();
            }
        }
    }
}
