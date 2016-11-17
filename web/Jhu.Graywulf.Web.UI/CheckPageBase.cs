using System;
using System.Collections.Generic;
using System.Web;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.Check
{
    public class CheckPageBase : PageBase
    {
        private CheckRoutineExecutor checks;
        private bool reportStatus;

        protected override void OnError(EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            checks = new CheckRoutineExecutor();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterChecks(checks.Routines);

            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            foreach (var apptype in application.Apps)
            {
                var app = (AppBase)Activator.CreateInstance(apptype);
                app.RegistryContext = RegistryContext;
                app.Initialize(application);
                app.RegisterChecks(checks.Routines);
            }

            // Generate a text exception
            if (Request.QueryString["break"] == null ? false : bool.Parse(Request.QueryString["break"]))
            {
                throw new Exception("Test exception thrown.");
            }

            // Set HTTP status (will buffer output)
            reportStatus = Request.QueryString["status"] == null ? false : bool.Parse(Request.QueryString["status"]);

            // Handle exceptions, only display them
            checks.HandleExceptions = Request.QueryString["throw"] == null ? true : !bool.Parse(Request.QueryString["throw"]);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            Response.Buffer = reportStatus;
            Response.BufferOutput = reportStatus;
            Response.Expires = -1;

            checks.Execute(Response.Output);

            if (reportStatus)
            {
                if (checks.Failed == 0)
                {
                    Response.StatusCode = 200;
                }
                else
                {
                    Response.StatusCode = 500;
                }
            }

            Response.Flush();
            Response.End();
        }

        protected virtual void RegisterChecks(List<CheckRoutineBase> checks)
        {
        }
    }
}
