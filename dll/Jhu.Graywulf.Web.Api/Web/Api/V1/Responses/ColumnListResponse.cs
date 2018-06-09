using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a list of data table columns.")]
    public class ColumnListResponse
    {
        [DataMember(Name = "columns")]
        [Description("An array of data table columns.")]
        public Column[] Columns { get; set; }

        public ColumnListResponse()
        {
        }

        public ColumnListResponse(IEnumerable<Jhu.Graywulf.Sql.Schema.Column> columns)
        {
            this.Columns = columns.Select(c => new Column(c)).ToArray();
        }
    }
}
