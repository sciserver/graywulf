using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Check
{
    /// <summary>
    /// Executes check routines and formats results into HTML
    /// </summary>
    public class CheckRoutineExecutor
    {
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

        public CheckRoutineExecutor()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.routines = new List<CheckRoutineBase>();
            this.handleExceptions = true;
        }

        public void Execute(TextWriter output)
        {
            output.WriteLine("<pre>");

            int success = 0;
            int failed = 0;

            int i = 0;
            while (i < routines.Count)
            {
                var r = routines[i];

                try
                {
                    output.WriteLine();
                    output.WriteLine("Test {0}/{1}:", success + failed, routines.Count);

                    r.Execute(output);

                    output.WriteLine("<font color=\"green\">Success</font>");

                    // Schedule additional tests
                    int k = i + 1;
                    foreach (var rr in r.GetCheckRoutines())
                    {
                        routines.Insert(k, rr);
                        k++;
                    }

                    success++;
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debugger.Break();
#endif

                    output.Write("<font color=\"red\">Error:</font> ");
                    output.WriteLine(ex.Message);
                    output.WriteLine("---");

                    failed++;

                    if (!handleExceptions)
                    {
                        throw;
                    }
                }

                i++;
            }

            output.WriteLine();
            output.WriteLine("Execution of {0} tests completed. {1} succeeded, {2} failed.",
                routines.Count,
                success,
                failed);

            output.WriteLine("</pre>");
        }
    }
}
