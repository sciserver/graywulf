using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public abstract class CheckRoutineBase
    {
        private bool handleExceptions;
        private List<CheckRoutineStatus> statuses;
        private CheckResult result;

        public bool HandleExceptions
        {
            get { return handleExceptions; }
            set { handleExceptions = value; }
        }

        public IList<CheckRoutineStatus> Statuses
        {
            get { return statuses; }
        }

        public CheckResult Result
        {
            get { return result; }
            internal set { result = value; }
        }

        public abstract CheckCategory Category { get; }

        protected CheckRoutineBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.handleExceptions = true;
            this.statuses = null;
        }

        public IEnumerable<CheckRoutineStatus> Execute()
        {
            var statuses = new List<CheckRoutineStatus>();
            var stenum = OnExecute().GetEnumerator();

            while (true)
            {
                bool more;
                CheckRoutineStatus status;

                try
                {
                    more = stenum.MoveNext();

                    if (more)
                    {
                        status = stenum.Current;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (handleExceptions)
                    {
                        status = ReportError(ex);
                    }
                    else
                    {
#if BREAKDEBUG                  
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
#endif

                        throw ex;
                    }
                }

                statuses.Add(status);
                yield return status;
            }
        }

        public IEnumerable<CheckRoutineBase> GetCheckRoutines()
        {
            return OnGetCheckRoutines();
        }

        protected CheckRoutineStatus ReportInfo(string message, params object[] args)
        {
            return new CheckRoutineStatus()
            {
                Message = String.Format(message, args),
                Result = CheckResult.Info,
            };
        }

        protected CheckRoutineStatus ReportSuccess(string message, params object[] args)
        {
            return new CheckRoutineStatus()
            {
                Message = String.Format(message, args),
                Result = CheckResult.Success,
            };
        }

        protected CheckRoutineStatus ReportWarning(string message, params object[] args)
        {
            return new CheckRoutineStatus()
            {
                Message = String.Format(message, args),
                Result = CheckResult.Warning,
            };
        }

        protected CheckRoutineStatus ReportError(string message, params object[] args)
        {
            return new CheckRoutineStatus()
            {
                Message = String.Format(message, args),
                Result = CheckResult.Error,
            };
        }

        protected CheckRoutineStatus ReportError(Exception ex)
        {
            return new CheckRoutineStatus()
            {
                Message = ex.Message,
                Result = CheckResult.Error,
                Error = ex
            };
        }

        protected abstract IEnumerable<CheckRoutineStatus> OnExecute();

        protected virtual IEnumerable<CheckRoutineBase> OnGetCheckRoutines()
        {
            yield break;
        }
    }
}
