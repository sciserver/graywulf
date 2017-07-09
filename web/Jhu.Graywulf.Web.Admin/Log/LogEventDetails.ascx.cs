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
            SetEntityLink(Logging.Constants.UserDataEntityGuid, EntityLink);
            SetEntityLink(Logging.Constants.UserDataEntityGuidFrom, EntityFromLink);
            SetEntityLink(Logging.Constants.UserDataEntityGuidTo, EntityToLink);
        }

        private void SetEntityLink(string key, Web.Admin.Controls.EntityLink link)
        {
            if (@event.UserData.ContainsKey(key) && (Guid)@event.UserData[key] != Guid.Empty)
            {
                var ef = new EntityFactory(RegistryContext);
                link.EntityReference.Value = ef.LoadEntity((Guid)@event.UserData[key]);
            }
        }
    }
}