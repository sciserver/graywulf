using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Types
{
    public interface IVariableMetadata
    {
        string Summary { get; set; }
        string Unit { get; set; }
        string Content { get; set; }
    }
}
