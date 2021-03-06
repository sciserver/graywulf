﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;
using Jhu.Graywulf.Sql.Schema.SqlServer;

namespace Jhu.Graywulf.Sql.Schema
{
    public class GraywulfUserDatabaseFactory : UserDatabaseFactory, ICheckable
    {

        // TODO: this is not currently used
        // TODO: add support for multiple MyDBs, group dbs, scratch, anything

        public GraywulfUserDatabaseFactory(FederationContext federationContext)
            : base(federationContext)
        {
        }

        protected void EnsureUserDatabaseInstanceExists(User user)
        {
            // TODO: update this to support multiple user databases
            var udii = new UserDatabaseInstanceInstaller(RegistryContext);
            udii.EnsureUserDatabaseInstanceExists(user, FederationContext.Federation.UserDatabaseVersion);
        }

        protected override void EnsureUserDatabaseExists(User user, SqlServerDataset dataset)
        {
            // TODO
            // Here we silently assume that the database is physically created
        }

        protected override void EnsureUserDatabaseConfigured(User user, SqlServerDataset dataset)
        {
            // Nothing to do here
        }

        protected override Dictionary<string, SqlServerDataset> OnGetUserDatabases(User user)
        {
            var di = FederationContext.Federation.UserDatabaseVersion.GetUserDatabaseInstance(user);

            if (di == null)
            {
                EnsureUserDatabaseInstanceExists(user);
                di = FederationContext.Federation.UserDatabaseVersion.GetUserDatabaseInstance(user);
            }

            var ds = di.GetDataset();

            ds.Name = Registry.Constants.UserDbName;
            ds.IsCacheable = false;
            ds.IsMutable = true;

            return new Dictionary<string, SqlServerDataset>(SchemaManager.Comparer)
            {
                { ds.Name, ds }
            };
        }

        protected override Dictionary<string, ServerInstance> OnGetUserDatabaseServerInstances(User user)
        {
            var di = FederationContext.Federation.UserDatabaseVersion.GetUserDatabaseInstance(user);

            return new Dictionary<string, ServerInstance>(SchemaManager.Comparer)
            {
                { Registry.Constants.UserDbName, di.ServerInstance }
            };
        }

        public override IEnumerable<Check.CheckRoutineBase> GetCheckRoutines()
        {
            // This class should work out of the box
            yield break;
        }
    }
}
