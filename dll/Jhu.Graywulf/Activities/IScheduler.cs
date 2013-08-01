using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Activities
{
    public interface IScheduler
    {
        void GetContextInfo(Guid workflowInstanceId, out Guid userGuid, out string userName, out Guid jobGuid, out string jobID);
        Guid GetServerInstance(string[] dds, string databaseVersion);
        Guid[] GetServerInstances(string[] dds, string databaseVersion);
    }
}
