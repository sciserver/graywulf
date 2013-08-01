using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableAlias
    {
        public static TableAlias Create(string alias)
        {
            var res = new TableAlias();

            res.Stack.AddLast(Identifier.Create(alias));

            return res;
        }

        public override bool AcceptCodeGenerator(CodeGenerator cg)
        {
            return ((SqlCodeGen.SqlCodeGeneratorBase)cg).WriteTableAlias(this);
        }
    }
}
