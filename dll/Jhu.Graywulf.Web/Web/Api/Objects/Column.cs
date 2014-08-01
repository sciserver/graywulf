using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name = "column")]
    public class Column
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "dataType")]
        public string DataType { get; set; }

        public Column()
        {
        }

        public Column(Jhu.Graywulf.Schema.Column column)
        {
            this.Name = column.Name;
            this.DataType = column.DataType.NameWithLength;
        }
    }
}
