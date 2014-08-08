using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "column")]
    [Description("Represents a data table column.")]
    public class Column
    {
        [DataMember(Name = "name")]
        [Description("Name of the column.")]
        public string Name { get; set; }

        [DataMember(Name = "dataType")]
        [Description("Data type of the column.")]
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
