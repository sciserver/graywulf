﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public abstract class IdentityProviderBase
    {
        #region Member Variables

        private Context context;

        private bool isActivationRequired;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the context of the object.
        /// </summary>
        [XmlIgnore]
        public Context Context
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
        protected IdentityProviderBase()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor that creates an objects with a context set.
        /// </summary>
        /// <param name="context"></param>
        protected IdentityProviderBase(Context context)
        {
            InitializeMembers(new StreamingContext());

            this.context = context;
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
        public abstract User GetUser(string username);

        public abstract User GetUserByEmail(string email);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"></param>
        public abstract void CreateUser(User user);

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
        public abstract User VerifyPassword(string username, string password);

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

        public abstract void MakeMemberOf(User user, UserGroup group);

        public abstract void RevokeMemberOf(User user, UserGroup group);

        #endregion
        #region Authentication token functions

#if false
        public abstract Token IssueToken(User user, DateTime expiresAt);

        public abstract Token RenewToken(User user, Token token, DateTime expiresAt);

        public abstract void RevokeToken(User user, Token token);

        // TODO: create long-living token
#endif

        #endregion

    }
}
