using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract]
    public class TableList
    {
        [DataMember(Name="tables")]
        public Table[] Tables { get; set; }
    }
}
