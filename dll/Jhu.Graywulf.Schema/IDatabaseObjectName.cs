using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    public interface IDatabaseObjectName
    {
        [DataMember]
        string DatabaseName { get; set; }

        [DataMember]
        string SchemaName { get; set; }

        [DataMember]
        string ObjectName { get; set; }
    }
}
