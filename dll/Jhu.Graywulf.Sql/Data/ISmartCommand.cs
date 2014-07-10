using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public interface ISmartCommand : IDbCommand
    {
        string Name { get; }
        DatasetMetadata Metadata { get; }

        bool RecordsCounted { get; set; }

        new ISmartDataReader ExecuteReader();
        new ISmartDataReader ExecuteReader(CommandBehavior behavior);
    }
}
