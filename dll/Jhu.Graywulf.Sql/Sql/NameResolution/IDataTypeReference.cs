﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public interface IDataTypeReference : IDatabaseObjectReference
    {
        DataTypeReference DataTypeReference { get; set; }
    }
}
