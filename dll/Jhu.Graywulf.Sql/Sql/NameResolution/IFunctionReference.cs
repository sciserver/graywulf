﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public interface IFunctionReference : IDatabaseObjectReference
    {
        FunctionReference FunctionReference { get; set; }
    }
}
