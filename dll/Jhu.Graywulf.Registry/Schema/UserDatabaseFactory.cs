using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    public abstract class UserDatabaseFactory : ContextObject
    {
        #region Static cache implementation

        private static Cache<string, DatasetBase> userDatabaseCache;

        static UserDatabaseFactory()
        {
            userDatabaseCache = new Cache<string, DatasetBase>();
        }

        #endregion
        #region

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

        public abstract void EnsureUserDatabaseExists(User user);

        public SqlServer.SqlServerDataset GetUserDatabase(User user)
        {
            DatasetBase ds;

            // Check in cache first, otherwise load and store in cache
            if (!userDatabaseCache.TryGetValue(user.Name, out ds))
            {
                ds = OnGetUserDatabase(user);
                userDatabaseCache.TryAdd(user.Name, ds, DateTime.Now.AddMinutes(10));
            }

            return (SqlServer.SqlServerDataset)ds;
        }

        public ServerInstance GetUserDatabaseServerInstance(User user)
        {
            return OnGetUserDatabaseServerInstance(user);
        }

        protected abstract SqlServer.SqlServerDataset OnGetUserDatabase(User user);

        protected abstract ServerInstance OnGetUserDatabaseServerInstance(User user);
    }
}
