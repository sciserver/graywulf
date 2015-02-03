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
    [DataContract(Name="viewList")]
    [Description("Represents a list of views.")]
    public class ViewListResponse
    {
        [DataMember(Name = "views")]
        [Description("An array of views.")]
        public View[] Views { get; set; }

        public ViewListResponse()
        {
        }

        public ViewListResponse(IEnumerable<Jhu.Graywulf.Schema.View> views)
        {
            this.Views = views.Select(v => new View(v)).ToArray();
        }
    }
}
