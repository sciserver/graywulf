using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Parsing
{
    public class TokenStack : LinkedList<Token>
    {
        public void Exchange(Token old, Token other)
        {
            if (old != other)
            {
                var i = Find(old);
                AddAfter(i, other);
                other.Parent = old.Parent;
                Remove(i);
            }
        }
    }
}
