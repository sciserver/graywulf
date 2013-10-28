using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Types
{
    public interface IVariable
    {
        int ID { get; set; }
        string Name { get; set; }
        DataType DataType { get; set; }
        IVariableMetadata Metadata { get; set; }
    }
}
