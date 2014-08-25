using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    public abstract class UserDatabaseFactory
    {
        private Federation federation;

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

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(UserDatabaseFactory);
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

        public abstract DatasetBase GetUserDatabase(User user);
    }
}
