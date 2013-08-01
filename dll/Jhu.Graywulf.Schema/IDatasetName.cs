using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    public interface IDatasetName
    {
        [IgnoreDataMember]
        string DatabaseName { get; set; }
    }
}
