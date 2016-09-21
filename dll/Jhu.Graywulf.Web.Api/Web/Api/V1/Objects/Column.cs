using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "column")]
    [DataContractFormat]
    [Description("Represents a data table column.")]
    public class Column
    {
        [DataMember(Name = "name")]
        [Description("Name of the column.")]
        public string Name { get; set; }

        [DataMember(Name = "dataType")]
        [Description("Data type of the column.")]
        public string DataType { get; set; }

        [DataMember(Name = "size")]
        [Description("Size of the column in bytes.")]
        public int Size { get; set; }

        [DataMember(Name = "class")]
        [Description("Class descriptor string of the column.")]
        public string Class { get; set; }

        [DataMember(Name = "quantity")]
        [Description("Quantity descriptor string of the column.")]
        public string Quantity { get; set; }

        [DataMember(Name = "unit")]
        [Description("Unit of a quantity stored in the column.")]
        public string Unit { get; set; }

        [DataMember(Name = "summary")]
        [Description("Summary of column meanings.")]
        public string Summary { get; set; }

        [DataMember(Name = "remarks")]
        [Description("Remarks about the contents of the column.")]
        public string Remarks { get; set; }

        public Column()
        {
        }

        public Column(Jhu.Graywulf.Schema.Column column)
        {
            this.Name = column.Name;
            this.DataType = column.DataType.TypeNameWithLength;
            this.Size = column.DataType.ByteSize;
            this.Class = column.Metadata.Class;
            this.Quantity = column.Metadata.Quantity;
            this.Unit = column.Metadata.Unit;
            this.Summary = column.Metadata.Summary;
            this.Remarks = column.Metadata.Remarks;
        }
    }
}
