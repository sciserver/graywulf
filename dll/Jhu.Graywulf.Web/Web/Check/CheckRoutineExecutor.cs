using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.Check
{
    public class CheckRoutineExecutor
    {
        private PageBase page;
        private List<CheckRoutineBase> routines;
        private bool handleExceptions;

        public List<CheckRoutineBase> Routines
        {
            get { return routines; }
        }

        public bool HandleExceptions
        {
            get { return handleExceptions; }
            set { handleExceptions = value; }
        }

        public CheckRoutineExecutor(PageBase page)
        {
            InitializeMembers();

            this.page = page;
        }

        private void InitializeMembers()
        {
            this.page = null;
            this.routines = new List<CheckRoutineBase>();
            this.handleExceptions = true;
        }

        public void Execute()
        {
            page.Response.Expires = -1;

            page.Response.Output.WriteLine("<pre>");

            page.Response.Output.WriteLine("Executing {0} tests under the account {1}\\{2} on {3}",
                routines.Count,
                Environment.UserDomainName,
                Environment.UserName,
                Environment.MachineName);

            int success = 0;
            int failed = 0;
            
            foreach (var r in routines)
            {
                try
                {
                    page.Response.Output.WriteLine();
                    page.Response.Output.WriteLine("Test {0}/{1}:", success + failed, routines.Count);

                    r.Execute(page);

                    page.Response.Output.WriteLine("<font color=\"green\">Success</font>");

                    success++;
                }
                catch (Exception ex)
                {
                    page.Response.Output.Write("<font color=\"red\">Error:</font> ");
                    page.Response.Output.WriteLine(ex.Message);
                    page.Response.Output.WriteLine("---");
                    
                    failed++;

                    if (!handleExceptions)
                    {
                        throw;
                    }
                }
            }

            page.Response.Output.WriteLine();
            page.Response.Output.WriteLine("Execution of {0} tests completed. {1} succeeded, {2} failed.",
                routines.Count,
                success,
                failed);

            page.Response.Output.WriteLine("</pre>");

            page.Response.End();
        }
    }
}
