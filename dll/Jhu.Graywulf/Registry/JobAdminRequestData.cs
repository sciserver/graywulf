/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class represents a request sent to the system administrators or other users
    /// by job workflows independently entering into the suspended state.
    /// </summary>
    /// <remarks>
    /// <b>JobAdminRequestData</b> objects are created by the <b>AdminRequestActivity</b>
    /// activities and persisted in the database as XML along with the <see cref="JobInstance"/>.
    /// </remarks>
    public class JobAdminRequestData
    {
        #region Member Variables

        private string title;
        private string message;
        private List<string> options;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the title of the administrator request.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Gets or sets the message sent to the administrator detailing the request.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Gets a collection of string with the possible answers to the request that the
        /// administrator has to choose from.
        /// </summary>
        public List<string> Options
        {
            get { return options; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that initializes private members.
        /// </summary>
        public JobAdminRequestData()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor that creates a deep copy of the passed <b>JobAdminRequestData</b> class.
        /// </summary>
        /// <param name="old">The original object to copy data from.</param>
        public JobAdminRequestData(JobAdminRequestData old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private members.
        /// </summary>
        private void InitializeMembers()
        {
            this.title = string.Empty;
            this.message = string.Empty;
            this.options = new List<string>();
        }

        /// <summary>
        /// Copy private members from another object.
        /// </summary>
        /// <param name="old">The original object to copy data from.</param>
        private void CopyMembers(JobAdminRequestData old)
        {
            this.title = old.title;
            this.message = old.message;
            this.options = old.options == null ? null : new List<string>(old.options);
        }

        #endregion
    }
}
