using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.Check
{
    public class CheckPageBase : PageBase
    {
        private CheckRoutineExecutor checks;

        protected CheckRoutineExecutor Checks
        {
            get { return checks; }
        }

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

            // Generate a text exception
            if (Request.QueryString["break"] == null ? false : bool.Parse(Request.QueryString["break"]))
            {
                throw new Exception("Test exception thrown.");
            }

            // Handle exceptions, only display them
            checks.HandleExceptions = Request.QueryString["throw"] == null ? true : !bool.Parse(Request.QueryString["throw"]);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            Response.Expires = -1;

            checks.Execute(Response.Output);

            Response.End();
        }
    }
}
