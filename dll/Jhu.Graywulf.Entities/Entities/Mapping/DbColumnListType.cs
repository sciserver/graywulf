using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.Mapping
{
    internal enum DbColumnListType
    {
        Select,
        Insert,
        InsertValues,
        Update,
        Where
    }
}
