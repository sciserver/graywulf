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
        private CheckCategory filter;
        private bool handleExceptions;
        private int succeeded;
        private int failed;

        public List<CheckRoutineBase> Routines
        {
            get { return routines; }
        }

        public CheckCategory Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        public bool HandleExceptions
        {
            get { return handleExceptions; }
            set { handleExceptions = value; }
        }

        public int Succeeded
        {
            get { return succeeded; }
        }

        public int Failed
        {
            get { return failed; }
        }

        public CheckRoutineExecutor()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.routines = new List<CheckRoutineBase>();
            this.filter = CheckCategory.All;
            this.handleExceptions = true;
        }

        public void Execute()
        {
            Execute(null);
        }

        public void Execute(TextWriter writer)
        {
            succeeded = 0;
            failed = 0;

            if (writer != null)
            {
                WriteHeader(writer);
            }

            int i = 0;
            while (i < routines.Count)
            {
                var r = routines[i];
                r.HandleExceptions = this.handleExceptions;

                if ((r.Category & filter) != 0)
                {
                    // Iterate over status messages
                    bool error = false;
                    foreach (var status in r.Execute())
                    {
                        if (writer != null)
                        {
                            switch (status.Result)
                            {
                                case CheckResult.Info:
                                    WriteInfo(writer, status);
                                    break;
                                case CheckResult.Success:
                                    WriteSuccess(writer, status);
                                    break;
                                case CheckResult.Warning:
                                    WriteWarning(writer, status);
                                    break;
                                case CheckResult.Error:
                                    WriteError(writer, status);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }

                        error |= status.Result == CheckResult.Error;
                    }

                    if (writer != null)
                    {
                        WriteSeparator(writer);
                    }

                    if (error)
                    {
                        r.Result = CheckResult.Error;
                        failed++;
                    }
                    else
                    {
                        // Schedule additional tests
                        int k = i + 1;
                        foreach (var rr in r.GetCheckRoutines())
                        {
                            routines.Insert(k, rr);
                            k++;
                        }

                        r.Result = CheckResult.Success;
                        succeeded++;
                    }
                }

                i++;
            }

            if (writer != null)
            {
                WriteFooter(writer);
            }
        }

        private void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("<pre>");
        }

        private void WriteInfo(TextWriter writer, CheckRoutineStatus status)
        {
            writer.WriteLine(status.Message);
        }

        private void WriteSuccess(TextWriter writer, CheckRoutineStatus status)
        {
            writer.Write("<font color=\"green\">Success: </font>");
            writer.WriteLine(status.Message);
        }

        private void WriteWarning(TextWriter writer, CheckRoutineStatus status)
        {
            writer.Write("<font color=\"blue\">Warning: </font>");
            writer.WriteLine(status.Message);
        }

        private void WriteError(TextWriter writer, CheckRoutineStatus status)
        {
            writer.Write("<font color=\"red\">Error: </font>");
            writer.WriteLine(status.Message);

            var ex = status.Error;
            while (ex != null)
            {
                writer.WriteLine(ex.Message);
                ex = ex.InnerException;
            }
        }

        private void WriteSeparator(TextWriter writer)
        {
            writer.WriteLine();
        }

        private void WriteFooter(TextWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("Execution of {0} tests completed. {1} succeeded, {2} failed.",
                succeeded + failed,
                succeeded,
                failed);

            writer.WriteLine("</pre>");
        }
    }
}
