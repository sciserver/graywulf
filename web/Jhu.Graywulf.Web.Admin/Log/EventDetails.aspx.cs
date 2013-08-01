using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.Admin.Log
{
    public partial class EventDetails : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ef = new WebEventFactory();
                var ev = ef.LoadEvent(long.Parse(Request.QueryString["eventId"]));

                EventId.Text = ev.EventId.ToString();
                EventOrder.Text = ev.EventOrder.ToString();
                EventSeverity.Text = ev.EventSeverity.ToString();
                ExceptionType.Text = ev.ExceptionType;

                StackTrace.Text = Server.HtmlEncode(ev.StackTrace).Replace("\r\n", "<br />");
            }
        }
    }
}