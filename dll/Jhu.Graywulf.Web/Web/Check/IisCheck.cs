using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.Check
{
    public class IisCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Iis; }
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Web server version is IIS{0}", Util.IIS.MajorVersion);
            yield return ReportInfo(".Net framework version is {0}", Environment.Version.ToString());
            yield return ReportInfo("Executing tests under the account {0}\\{1} on {2}",
                Environment.UserDomainName,
                Environment.UserName,
                Environment.MachineName);
            yield return ReportSuccess("OK");
        }
    }
}
