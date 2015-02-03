using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public abstract class DatabaseObject
    {
        [DataMember(Name = "name")]
        [Description("Name of the database object.")]
        public string Name { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "remarks")]
        public string Remarks { get; set; }

        [DataMember(Name = "example")]
        public string Example { get; set; }

        [DataMember(Name = "class")]
        public string Class { get; set; }

        protected DatabaseObject()
        {
        }

        protected DatabaseObject(Schema.DatabaseObject obj)
        {
            this.Name = obj.DisplayName;
            this.Summary = obj.Metadata.Summary;
            this.Remarks = obj.Metadata.Remarks;
            this.Example = obj.Metadata.Example;
            this.Class = obj.Metadata.Class;
        }
    }
}
