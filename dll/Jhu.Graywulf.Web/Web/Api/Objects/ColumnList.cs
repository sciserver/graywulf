using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name = "columnList")]
    public class ColumnList
    {
        [DataMember(Name = "columns")]
        public Column[] Columns { get; set; }

        public ColumnList()
        {
        }

        public ColumnList(IEnumerable<Jhu.Graywulf.Schema.Column> columns)
        {
            this.Columns = columns.Select(c => new Column(c)).ToArray();
        }
    }
}
