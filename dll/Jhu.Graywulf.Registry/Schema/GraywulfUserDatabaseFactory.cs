using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Schema
{
    public class GraywulfUserDatabaseFactory : UserDatabaseFactory
    {
        public GraywulfUserDatabaseFactory(Federation federation)
            : base(federation)
        {
        }

        public override void EnsureUserDatabaseExists(User user)
        {
            var udii = new UserDatabaseInstanceInstaller(Federation.Context);
            udii.EnsureUserDatabaseInstanceExists(user, Federation.MyDBDatabaseVersion);
        }

        public override DatasetBase GetUserDatabase(User user)
        {
            var di = Federation.MyDBDatabaseVersion.GetUserDatabaseInstance(user);
            var ds = di.GetDataset();

            ds.Name = Jhu.Graywulf.Registry.Constants.MyDbName;
            ds.IsCacheable = false;
            ds.IsMutable = true;

            return ds;
        }
    }
}
