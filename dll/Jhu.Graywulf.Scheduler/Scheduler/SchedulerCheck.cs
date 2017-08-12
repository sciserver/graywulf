using System;
using System.Collections.Generic;
using System.IO;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerCheck : CheckRoutineBase
    {
        private string host;

        public override CheckCategory Category
        {
            get { return CheckCategory.Service; }
        }

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public SchedulerCheck()
        {
            InitializeMembers();
        }

        public SchedulerCheck(string host)
        {
            InitializeMembers();

            this.host = host;
        }

        private void InitializeMembers()
        {
            this.host = null;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            string name;
            bool isAuthenticated;
            string authenticationType;

            yield return ReportInfo("Testing scheduler service on {0}...", host);

            var sc = ServiceHelper.CreateChannel<ISchedulerControl>(host, "Control", Scheduler.Configuration.Endpoint);

            sc.WhoAmI(out name, out isAuthenticated, out authenticationType);
            yield return ReportInfo("Client user: {0} ({1})", name, authenticationType);

            sc.WhoAreYou(out name, out isAuthenticated, out authenticationType);
            yield return ReportInfo("Service user: {0} ({1})", name, authenticationType);

            yield return ReportSuccess("OK");
        }

        protected override IEnumerable<CheckRoutineBase> OnGetCheckRoutines()
        {
            yield break;
        }
    }
}
