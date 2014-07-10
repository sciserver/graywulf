using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jhu.Graywulf.Data
{
    public interface ISmartCommand : IDbCommand
    {
        BatchProperties Properties { get; }
        bool RecordsCounted { get; set; }

        new ISmartDataReader ExecuteReader();
        new ISmartDataReader ExecuteReader(CommandBehavior behavior);
    }
}
