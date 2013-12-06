using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Types
{
    public interface IColumn : IVariable
    {
        bool IsIdentity { get; set; }
        bool IsKey { get; set; }
        bool IsHidden { get; set; }
    }
}
