using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name="tableList")]
    [Description("Represents a list of data tables.")]
    public class TableListResponse
    {
        [DataMember(Name = "tables")]
        [Description("An array of tables.")]
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
