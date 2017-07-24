using System;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    public interface IScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        JobInfo GetJobInfo(Guid workflowInstanceId);

        /// <summary>
        /// Returns a server instance containing all necessary database instances
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersion"></param>
        /// <param name="databaseInstances"></param>
        /// <returns></returns>
        Guid GetNextServerInstance(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances);
        
        /// <summary>
        /// Returns all servers containing all necessary database instances
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersion"></param>
        /// <param name="databaseInstances"></param>
        /// <returns></returns>
        Guid[] GetServerInstances(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances);

        Guid GetNextDatabaseInstance(Guid databaseDefinition, string databaseVersion);

        Guid[] GetDatabaseInstances(Guid databaseDefinition, string databaseVersion);

        Guid[] GetDatabaseInstances(Guid serverInstance, Guid databaseDefinition, string databaseVersion);
    }
}
