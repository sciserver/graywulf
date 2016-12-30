using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.Schema
{
    public abstract class UserDatabaseFactory : ContextObject, ICheckable
    {
        #region Static cache implementation

        private static Cache<string, Dictionary<string, SqlServerDataset>> userDatabaseCache;

        static UserDatabaseFactory()
        {
            userDatabaseCache = new Cache<string, Dictionary<string, SqlServerDataset>>(SchemaManager.Comparer);
        }

        #endregion
        #region Private member variables

        private Federation federation;

        #endregion
        #region Properties

        protected Federation Federation
        {
            get { return federation; }
        }

        #endregion
        #region Constructors and initializers

        public static UserDatabaseFactory Create(Federation federation)
        {
            return Create(federation.UserDatabaseFactory, federation);
        }

        public static UserDatabaseFactory Create(string typeName, Federation federation)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(typeName))
            {
                type = Type.GetType(typeName);
            }

            // There is no fall-back alternative if configuration is incorrect
            if (type == null)
            {
                throw new Exception("Cannot load UserDatabaseFactory.");    // TODO ***
            }

            return (UserDatabaseFactory)Activator.CreateInstance(type, new object[] { federation });
        }

        // TODO: add assigned server instance
        protected UserDatabaseFactory(Federation federation)
            :base(federation.Context)
        {
            InitializeMembers(new StreamingContext());

            this.federation = federation;
        }

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.federation = null;
        }

        #endregion

        protected abstract void EnsureUserDatabaseExists(User user, SqlServerDataset dataset);

        protected abstract void EnsureUserDatabaseConfigured(User user, SqlServerDataset dataset);

        public Dictionary<string, SqlServerDataset> GetUserDatabases(User user)
        {
            Dictionary<string, SqlServerDataset> dbs;

            // Check in cache first, otherwise load and store in cache
            if (!userDatabaseCache.TryGetValue(user.Name, out dbs))
            {
                dbs = OnGetUserDatabases(user);

                foreach (var ds in dbs.Values)
                {
                    try
                    {
                        EnsureUserDatabaseExists(user, ds);
                        EnsureUserDatabaseConfigured(user, ds);
                    }
                    catch (Exception ex)
                    {
                        ds.IsInError = true;
                        ds.ErrorMessage = ex.Message;
                    }
                }

                userDatabaseCache.TryAdd(user.Name, dbs, DateTime.Now.AddMinutes(10));
            }

            foreach (var db in dbs.Values)
            {
                if (!db.IsCacheable)
                {
                    db.FlushCache();
                }
            }

            return dbs;
        }

        public Dictionary<string, ServerInstance> GetUserDatabaseServerInstances(User user)
        {
            return OnGetUserDatabaseServerInstances(user);
        }

        protected abstract Dictionary<string, SqlServerDataset> OnGetUserDatabases(User user);

        protected abstract Dictionary<string, ServerInstance> OnGetUserDatabaseServerInstances(User user);

        #region Check routines

        public abstract IEnumerable<CheckRoutineBase> GetCheckRoutines();

        #endregion
    }
}
