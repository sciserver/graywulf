using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name="table")]
    [Description("Represents a database table.")]
    public class Table
    {
        [DataMember(Name="name")]
        [Description("Name of the table.")]
        public string Name { get; set; }

        [DataMember(Name = "columns")]
        [Description("An array of columns of the table.")]
        public Column[] Columns { get; set; }

        public Table()
        {
        }

        public Table(Jhu.Graywulf.Schema.Table table)
        {
            this.Name = table.DisplayName;
            this.Columns = table.Columns.Values.Select(c => new Column(c)).ToArray();
        }
    }
}
