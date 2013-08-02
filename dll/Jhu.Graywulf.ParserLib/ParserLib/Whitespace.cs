using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token that matches a terminal
    /// defined by a regular expression.
    /// </summary>
    /// <remarks>
    /// Whitespace tokens are distinguished from normal
    /// terminals in the sense that they can be omitted
    /// when traversing the parsing tree
    /// </remarks>
    public abstract class Whitespace : Terminal
    {
        public Whitespace()
            :base()
        {
        }

        public Whitespace(Whitespace old)
            : base(old)
        {
        }
    }
}
