using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Metadata
{
    public enum ObjectType
    {
        Table,
        View,
        Procedure,
        Function
    }

    public enum ParameterType
    {
        Unknown,
        Column,
        Param
    }
}
