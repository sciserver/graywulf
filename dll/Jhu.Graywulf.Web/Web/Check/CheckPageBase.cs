using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            checks = new CheckRoutineExecutor(this);
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

            // Send an email to a specified address
            if (Request.QueryString["email"] != null)
            {
                checks.Routines.Add(new EmailCheck(Request.QueryString["email"]));
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            checks.Execute();

            base.OnLoadComplete(e);
        }
    }
}
