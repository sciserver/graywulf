using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Admin.Log
{
    public partial class LogEventDetails : UserControlBase
    {
        private Event @event;

        public Event Event
        {
            get { return @event; }
            set
            {
                @event = value;
                UpdateForm();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void UpdateForm()
        {
            var ef = new EntityFactory(RegistryContext);

            if (@event.EntityGuid != null && @event.EntityGuid != Guid.Empty)
            {
                EntityLink.EntityReference.Value = ef.LoadEntity(@event.EntityGuid);
            }

            if (@event.EntityGuidFrom != null && @event.EntityGuidFrom != Guid.Empty)
            {
                EntityFromLink.EntityReference.Value = ef.LoadEntity(@event.EntityGuidFrom);
            }

            if (@event.EntityGuidTo != null && @event.EntityGuidTo != Guid.Empty)
            {
                EntityToLink.EntityReference.Value = ef.LoadEntity(@event.EntityGuidTo);
            }
        }
    }
}