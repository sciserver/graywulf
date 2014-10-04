using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    public abstract class UserDatabaseFactory
    {
        #region Static cache implementation

        private static Cache<string, DatasetBase> userDatabaseCache;

        static UserDatabaseFactory()
        {
            userDatabaseCache = new Cache<string, DatasetBase>();
        }

        #endregion
        #region Private member variables

        private Federation federation;

        #endregion
        #region Properties

        public Federation Federation
        {
            get { return federation; }
            set { federation = value; }
        }

        #endregion
        #region Constructors and initializers

        // TODO: add assigned server instance
        protected UserDatabaseFactory(Federation federation)
        {
            InitializeMembers(new StreamingContext());

            this.federation = federation;
        }

        public static UserDatabaseFactory Create(Federation federation)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(federation.UserDatabaseFactory))
            {
                type = Type.GetType(federation.UserDatabaseFactory);
            }

            // There is no fall-back alternative if configuration is incorrect
            if (type == null)
            {
                throw new Exception("Cannot load UserDatabaseFactory specified in federation settings.");    // TODO ***
            }

            return (UserDatabaseFactory)Activator.CreateInstance(type, new object[] { federation });
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

        public DatasetBase GetUserDatabase(User user)
        {
            DatasetBase ds;

            // Check in cache first, otherwise load and store in cache
            if (!userDatabaseCache.TryGetValue(user.Name, out ds))
            {
                ds = OnGetUserDatabase(user);
                userDatabaseCache.TryAdd(user.Name, ds, DateTime.Now.AddMinutes(10));
            }

            return ds;
        }

        protected abstract DatasetBase OnGetUserDatabase(User user);
    }
}
