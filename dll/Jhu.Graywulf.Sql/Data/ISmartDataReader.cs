﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public interface ISmartDataReader : IDataReader
    {
        long RecordCount { get; }

        IList<Column> GetColumns();
    }
}
