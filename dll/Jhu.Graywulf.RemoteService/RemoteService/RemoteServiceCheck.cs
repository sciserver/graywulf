using System;
using System.Collections.Generic;
using System.IO;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.RemoteService
{
    public class RemoteServiceCheck : CheckRoutineBase
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

        public RemoteServiceCheck()
        {
            InitializeMembers();
        }

        public RemoteServiceCheck(string host)
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
            yield return ReportInfo("Testing remoting server on {0}...", host);

            using (var sc = RemoteServiceHelper.GetControlObject(host, TimeSpan.FromSeconds(1)))
            {
                AuthenticationDetails details;

                details = Util.TaskHelper.Wait(sc.Value.WhoAmIAsync());
                yield return ReportInfo("Client user: {0} ({1})", details.Name, details.AuthenticationType);

                details = Util.TaskHelper.Wait(sc.Value.WhoAreYouAsync());
                yield return ReportInfo("Service user: {0} ({1})", details.Name, details.AuthenticationType);
            }

            yield return ReportSuccess("OK");
        }

        protected override IEnumerable<CheckRoutineBase> OnGetCheckRoutines()
        {
            yield break;
        }
    }
}
