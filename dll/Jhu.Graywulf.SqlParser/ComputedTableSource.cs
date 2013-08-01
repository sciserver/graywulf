using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public class ComputedTableSource : Jhu.Graywulf.ParserLib.Node
    {
        private TableReference tableReference;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public override bool Match(ParserLib.Parser parser)
        {
            throw new NotImplementedException();
        }
    }
}
