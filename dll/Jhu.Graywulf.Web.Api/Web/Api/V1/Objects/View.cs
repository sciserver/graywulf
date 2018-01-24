using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "view")]
    [Description("Represents a database view.")]
    public class View : DatabaseObject
    {
        [DataMember(Name = "columns")]
        [Description("An array of columns of the view.")]
        public Column[] Columns { get; set; }

        public View()
        {
        }

        public View(Jhu.Graywulf.Sql.Schema.View view)
            : base(view)
        {
            this.Columns = view.Columns.Values.Select(c => new Column(c)).ToArray();
        }
    }
}
