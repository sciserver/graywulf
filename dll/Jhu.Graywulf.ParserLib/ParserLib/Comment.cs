using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    public abstract class Comment : Terminal, ICloneable
    {
        #region Constructors and initializers

        protected Comment()
            : base()
        {
        }

        protected Comment(Comment old)
            : base(old)
        {
        }

        #endregion
    }
}
