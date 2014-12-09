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
            udii.EnsureUserDatabaseInstanceExists(user, Federation.UserDatabaseVersion);
        }

        protected override DatasetBase OnGetUserDatabase(User user)
        {
            var di = Federation.UserDatabaseVersion.GetUserDatabaseInstance(user);
            var ds = di.GetDataset();

            ds.Name = Jhu.Graywulf.Registry.Constants.UserDbName;
            ds.IsCacheable = false;
            ds.IsMutable = true;

            return ds;
        }
    }
}
