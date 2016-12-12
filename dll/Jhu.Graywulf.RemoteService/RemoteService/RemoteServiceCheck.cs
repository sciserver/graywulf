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

        public override void Execute(TextWriter output)
        {
            string name;
            bool isAuthenticated;
            string authenticationType;
            
            output.WriteLine("Testing remoting server on {0}...", host);
            var sc = RemoteServiceHelper.GetControlObject(host);

            sc.WhoAmI(out name, out isAuthenticated, out authenticationType);
            output.WriteLine("Client user: {0} ({1})", name, authenticationType);

            sc.WhoAreYou(out name, out isAuthenticated, out authenticationType);
            output.WriteLine("Service user: {0} ({1})", name, authenticationType);
        }

        public override IEnumerable<CheckRoutineBase> GetCheckRoutines()
        {
            yield break;
        }
    }
}
