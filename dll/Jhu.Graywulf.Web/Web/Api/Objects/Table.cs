using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="table")]
    public class Table
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        public Table(Jhu.Graywulf.Schema.Table table)
        {
            this.Name = table.DisplayName;
        }
    }
}
