using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    public abstract class Comment : Terminal
    {
        public Comment()
            : base()
        {
        }

        public Comment(Comment old)
            : base(old)
        {
        }
    }
}
