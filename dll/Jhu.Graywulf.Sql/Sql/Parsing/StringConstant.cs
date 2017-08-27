using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StringConstant
    {
        public string TrimmedValue
        {
            get { return this.Value.Trim('\''); }
            set { this.Value = '\'' + value + '\''; }
        }
    }
}
