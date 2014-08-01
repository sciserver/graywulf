using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name="tableList")]
    public class TableListResponse
    {
        [DataMember(Name = "tables")]
        public Table[] Tables { get; set; }

        public TableListResponse()
        {
        }

        public TableListResponse(IEnumerable<Jhu.Graywulf.Schema.Table> tables)
        {
            this.Tables = tables.Select(t => new Table(t)).ToArray();
        }
    }
}
