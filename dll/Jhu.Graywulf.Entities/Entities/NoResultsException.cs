using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Entities
{
    [Serializable]
    public class NoResultsException : Exception
    {
        public NoResultsException(string message)
            : base(message)
        {
        }
    }
}
