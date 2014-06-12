using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class UserDatabaseInstanceInstaller : ContextObject
    {
        private User user;

        public UserDatabaseInstanceInstaller(User user)
            :base(user.Context)
        {
            this.user = user;
        }

        public UserDatabaseInstance CreateUserDatabaseInstance(DatabaseVersion databaseVersion)
        {
            // Load server instances that can store user databases
            var ef = new EntityFactory(Context);
            var serverInstances = ef.FindAll<ServerInstance>()
                .Where(i => i.ServerVersionReference.Guid == databaseVersion.ServerVersionReference.Guid && i.Machine.RunningState == RunningState.Running)
                .OrderBy(i => i.Machine.Number)
                .ToArray();

            if (serverInstances.Length == 0)
            {
                throw new InvalidOperationException("At least one server instance has to configured for the database version.");    // TODO
            }

            // Get current number of user databases
            var dd = databaseVersion.DatabaseDefinition;
            dd.LoadDatabaseInstances(false);
            var dbnum = dd.DatabaseInstances.Count;

            // Pick the nect server that will store the user's myDB
            var sin = (dbnum + 1) % serverInstances.Length;

            // Default slice (FULL)
            dd.LoadSlices(false);
            var sl = dd.Slices[Constants.FullSliceName];
           
            // Create physical database instance
            var ddi = new DatabaseInstanceInstaller(databaseVersion.DatabaseDefinition);
            var di = ddi.GenerateDatabaseInstance(
                serverInstances[sin],
                sl,
                databaseVersion);

            di.Save();

            // Add user database mapping
            var udi = new UserDatabaseInstance(databaseVersion)
            {
                Name = String.Format("{0}_{1}_{2}", user.Name, di.DatabaseDefinition.Federation.Name, di.DatabaseDefinition.Name),
                User = user,
                DatabaseInstance = di,
            };

            udi.Save();

            var mydb = udi.DatabaseInstance;
            mydb.Deploy();

            return udi;
        }
    }
}
