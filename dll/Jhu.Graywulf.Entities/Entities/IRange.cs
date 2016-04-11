using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities
{
    public interface IRange
    {
        object From { get; set; }
        object To { get; set; }
    }
}
