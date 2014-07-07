using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jhu.Graywulf.Schema
{
    public interface ISmartCommand : IDbCommand
    {
        IList<Column> GetColumns();
        long GetRowCount();
        new ISmartDataReader ExecuteReader();
        new ISmartDataReader ExecuteReader(CommandBehavior behavior);
    }
}
