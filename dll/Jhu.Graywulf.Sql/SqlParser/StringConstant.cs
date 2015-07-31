using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
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
