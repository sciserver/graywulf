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
    public class Table : DatabaseObject
    {
        [DataMember(Name = "columns")]
        [Description("An array of columns of the table.")]
        public Column[] Columns { get; set; }

        public Table()
        {
        }

        public Table(Jhu.Graywulf.Sql.Schema.Table table)
            :base(table)
        {
            this.Columns = table.Columns.Values.Select(c => new Column(c)).ToArray();
        }
    }
}
