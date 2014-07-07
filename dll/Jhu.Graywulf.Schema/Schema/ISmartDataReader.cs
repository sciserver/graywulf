using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jhu.Graywulf.Schema
{
    public interface ISmartDataReader : IDataReader
    {        
        IList<Column> GetColumns();
        long GetRowCount();
    }
}
