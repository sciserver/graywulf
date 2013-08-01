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

        public UserDatabaseInstance GenerateUserDatabaseInstance(DatabaseVersion databaseVersion)
        {
            // TODO: this part probably needs some optimization

            // Load server instances that can store user databases
            var ef = new EntityFactory(Context);
            var sis = ef.FindAll<ServerInstance>()
                .Where(i => i.ServerVersionReference.Guid == databaseVersion.ServerVersionReference.Guid)
                .OrderBy(i => i.Machine.Number)
                .ToArray();

            if (sis.Length == 0)
            {
                throw new InvalidOperationException("At least one server instance has to configured for the database version.");    // TODO
            }

            // Get current number of user databases
            var dd = databaseVersion.DatabaseDefinition;
            dd.LoadDatabaseInstances(false);
            var dbnum = dd.DatabaseInstances.Count;

            // Find a server that is available
            int off = 0;
            int sin = 0;
            while (off < sis.Length)
            {
                sin = (dbnum + 1 + off) % sis.Length;
                bool ok = true;

                var diag = sis[sin].RunDiagnostics();

                foreach (var d in diag)
                {
                    if (d.Status != DiagnosticMessageStatus.OK)
                    {
                        ok = false;
                        break;
                    }
                }


                if (ok)
                {
                    break;
                }

                off++;
            }

            // Default slice (FULL)
            dd.LoadSlices(false);
            var sl = dd.Slices[Constants.FullSliceName];
           
            // Create physical database instance
            var ddi = new DatabaseInstanceInstaller(databaseVersion.DatabaseDefinition);
            var di = ddi.GenerateDatabaseInstance(
                sis[sin],
                sl,
                databaseVersion);

            di.Save();

            // Add user database mapping
            var udi = new UserDatabaseInstance(user);
            udi.DatabaseVersion = databaseVersion;
            udi.DatabaseInstance = di;

            udi.Save();

            return udi;
        }
    }
}
