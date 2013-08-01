using System;
using System.Collections.Generic;
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
using Jhu.Graywulf.Web.Admin;

namespace Jhu.Graywulf.Web.Admin.Log
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return "~/Log/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["userGuid"] != null)
            {
                LogFilterControl.Filter.UserGuid = Guid.Parse(Request.QueryString["userGuid"]);
            }

            if (Request.QueryString["jobGuid"] != null)
            {
                LogFilterControl.Filter.JobGuid = Guid.Parse(Request.QueryString["jobGuid"]);
            }

            if (Request.QueryString["entityGuid"] != null)
            {
                LogFilterControl.Filter.EntityGuid = Guid.Parse(Request.QueryString["entityGuid"]);
            }

            if (Request.QueryString["contextGuid"] != null)
            {
                LogFilterControl.Filter.ContextGuid = Guid.Parse(Request.QueryString["contextGuid"]);
            }
        }

        protected void eventDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            WebEventFactory f = new WebEventFactory();

            f.Filter = LogFilterControl.Filter;

            e.ObjectInstance = f;
        }

        protected void Refresh_Click(object sender, EventArgs e)
        {
            eventList.DataBind();
        }
    }
}