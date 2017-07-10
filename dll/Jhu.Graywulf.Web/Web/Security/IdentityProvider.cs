using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// When implmented in derived classes, offers function to manipulate and
    /// authenticate users.
    /// </summary>
    public abstract class IdentityProvider : ICheckable
    {
        #region Member Variables

        private RegistryContext context;
        private bool isActivationRequired;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the context of the object.
        /// </summary>
        [XmlIgnore]
        public RegistryContext Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// Gets or sets whether user accounts must be activated after registration.
        /// </summary>
        public bool IsActivationRequired
        {
            get { return isActivationRequired; }
            set { isActivationRequired = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that initializes private members to their
        /// defaul values.
        /// </summary>
        protected IdentityProvider()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor that creates an objects with a context set.
        /// </summary>
        /// <param name="context"></param>
        protected IdentityProvider(RegistryContext context)
        {
            InitializeMembers(new StreamingContext());

            this.context = context;
            this.isActivationRequired = true;
        }

        /// <summary>
        /// Creates a new identity provider based on the domain configuration.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IdentityProvider Create(Domain domain)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(domain.IdentityProvider))
            {
                type = Type.GetType(domain.IdentityProvider);
            }

            // There is no fall-back alternative if configuration is incorrect
            if (type == null)
            {
                throw new Exception("Cannot load IdentityProvider specified in domain settings.");    // TODO ***
            }

            return (IdentityProvider)Activator.CreateInstance(type, new object [] { domain });
        }

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.context = null;

            this.isActivationRequired = true;
        }

        #endregion
        #region User account manipulation

        /// <summary>
        /// Returns the user by its unique identifier.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <remarks>
        /// The unique identifier is either username or email address.
        /// </remarks>
        public abstract User GetUserByUserName(string username);

        /// <summary>
        /// Returns a user identified by e-mail. This function can be used
        /// for password recovery emails.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public abstract User GetUserByEmail(string email);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"></param>
        public void CreateUser(User user, string password)
        {
            // Set user activity status
            // Set user activity status depending on settings
            if (IsActivationRequired)
            {
                user.Deactivate();
            }
            else
            {
                user.Activate();
            }

            OnCreateUser(user, password);
        }

        /// <summary>
        /// Called by CreateUser. When implemented in derived classes, creates a new
        /// user.
        /// </summary>
        /// <param name="user"></param>
        protected abstract void OnCreateUser(User user, string password);

        /// <summary>
        /// Appends an identity (OpenID, etc.) to the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public abstract UserIdentity AddUserIdentity(User user, GraywulfIdentity identity);

        /// <summary>
        /// Modifies an existing user.
        /// </summary>
        /// <param name="user"></param>
        public abstract void ModifyUser(User user);

        /// <summary>
        /// Deletes an existing user.
        /// </summary>
        /// <param name="user"></param>
        public abstract void DeleteUser(User user);

        /// <summary>
        /// Checks whether the given username is already exists.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsNameExisting(string username);

        /// <summary>
        /// Checks whether the given email address already exists.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public abstract bool IsEmailExisting(string email);

        #endregion
        #region User activation

        /// <summary>
        /// Checks whether the current user is active.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public abstract bool IsUserActive(User user);

        /// <summary>
        /// Returns a user associated with the activation code.
        /// </summary>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public abstract User GetUserByActivationCode(string activationCode);

        /// <summary>
        /// Activates a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="activationCode"></param>
        public abstract void ActivateUser(User user);

        /// <summary>
        /// Deactivates a user.
        /// </summary>
        /// <param name="user"></param>
        public abstract void DeactivateUser(User user);

        #endregion
        #region Password functions

        /// <summary>
        /// Verifies user's credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract AuthenticationResponse VerifyPassword(AuthenticationRequest request);

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public abstract void ChangePassword(User user, string oldPassword, string newPassword);

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        public abstract void ResetPassword(User user, string newPassword);

        #endregion
        #region User group membership

        public abstract void LoadRoles(GraywulfPrincipal principal);

        /// <summary>
        /// Makes a user a member of a given group.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public abstract void MakeMemberOf(string user, string group, string role);

        /// <summary>
        /// Removes a user from a given group.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public abstract void RevokeMemberOf(string user, string group, string role);

        #endregion
        #region Check routines

        public abstract IEnumerable<CheckRoutineBase> GetCheckRoutines();

        #endregion
    }
}
