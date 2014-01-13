/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Xml.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>User</b> entity
    /// </summary>
    public partial class User : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string title;
        private string firstName;
        private string middleName;
        private string lastName;
        private Gender gender;
        private string nonValidatedEmail;
        private string email;
        private DateTime dateOfBirth;
        private string company;
        private string jobTitle;
        private string address;
        private string address2;
        private string state;
        private string stateCode;
        private string city;
        private string country;
        private string countryCode;
        private string zipCode;
        private string workPhone;
        private string homePhone;
        private string cellPhone;
        private int timeZone;
        private bool integrated;
        private string ntlmUser;
        private byte[] passwordHash;
        private string activationCode;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.User; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        /// <summary>
        /// Gets or sets the title (Mr., Dr. etc) of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 10)]
        [DefaultValue("")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Gets or sets the first name of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        /// <summary>
        /// Gets or sets the middle name (or initials) of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }

        /// <summary>
        /// Gets or sets the last name of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        /// <summary>
        /// Gets or sets the gender of the person.
        /// </summary>
        [DBColumn]
        [DefaultValue(Gender.Male)]
        public Gender Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        /// <summary>
        /// Gets or sets the email of the user that has not been validated yet.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string NonValidatedEmail
        {
            get { return nonValidatedEmail; }
            set { nonValidatedEmail = value; }
        }

        /// <summary>
        /// Gets or sets the e-mail address of the user.
        /// </summary>
        /// <remarks>
        /// This can be used for authentication to the
        /// Cluster Management Console instead of a username.
        /// </remarks>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        /// Gets or sets the date of birth of the person.
        /// </summary>
        [DBColumn]
        [DefaultDateTime("1950-01-01")]
        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        /// <summary>
        /// Gets or sets the affiliation of the user.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string Company
        {
            get { return company; }
            set { company = value; }
        }

        /// <summary>
        /// Gets or sets the job title of the user.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string JobTitle
        {
            get { return jobTitle; }
            set { jobTitle = value; }
        }

        /// <summary>
        /// Gets or sets the street address of the <b>User</b>.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// Gets or sets the street address of the <b>User</b>, second line.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }

        /// <summary>
        /// Gets or sets the name of the state of the address of the user.
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets the two-letter code of the state.
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value; }
        }

        /// <summary>
        /// Gets or sets the city part of the address of the user.
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        /// <summary>
        /// Gets or sets the country part of the address of the user.
        /// </summary>
        [DBColumn(Size = 128)]
        [DefaultValue("")]
        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        /// <summary>
        /// Gets or sets the two-letter code of the country of the user.
        /// </summary>
        [DBColumn(Size = 2)]
        [DefaultValue("")]
        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }

        /// <summary>
        /// Gets or sets the zip code part of the address of the user.
        /// </summary>
        [DBColumn(Size = 10)]
        [DefaultValue("")]
        public string ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        /// <summary>
        /// Gets or sets the work phone number of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string WorkPhone
        {
            get { return workPhone; }
            set { workPhone = value; }
        }

        /// <summary>
        /// Gets or sets the home phone number of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string HomePhone
        {
            get { return homePhone; }
            set { homePhone = value; }
        }

        /// <summary>
        /// Gets or sets the cell phone number of the <b>User</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string CellPhone
        {
            get { return cellPhone; }
            set { cellPhone = value; }
        }

        /// <summary>
        /// Gets or sets the time zone of the user.
        /// </summary>
        /// <remarks>
        /// Expressed as the difference from GMT in number of minutes.
        /// </remarks>
        [DBColumn]
        [DefaultValue(0)]
        public int TimeZone
        {
            get { return timeZone; }
            set { timeZone = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the user uses integrated windows authentication
        /// to access the Cluster Management Console.
        /// </summary>
        /// <remarks>
        /// If this is set to <b>true</b> the <see cref="NtlmUser"/> property must be set to the
        /// windows username including domain information, e.g. <i>GRAYWULF\johndoe</i>. Otherwise
        /// the <see cref="NtlmUser"/> property is ignored at authentication.
        /// </remarks>
        [DBColumn]
        [DefaultValue(false)]
        public bool Integrated
        {
            get { return integrated; }
            set { integrated = value; }
        }

        /// <summary>
        /// Gets or set the windows domain username associated with the <b>User</b>
        /// </summary>
        /// <remarks>
        /// If the value of the <see cref="Integrated"/> property is set to <b>true</b> the
        /// <b>User</b> is authenticated with windows authentication. In this case this property should
        /// match the <b>User's</b> windows domain username including any domain information, e.g. <i>GRAYWULF\johndoe</i>.
        /// </remarks>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string NtlmUser
        {
            get { return ntlmUser; }
            set { ntlmUser = value; }
        }

        /// <summary>
        /// Gets the password hash value stored in the database.
        /// </summary>
        /// <remarks>Use the <see cref="SetPassword"/> function to generate a password hash from string.</remarks>
        [DBColumn(Size = 1024)]
        public byte[] PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string ActivationCode
        {
            get { return activationCode; }
            set { activationCode = value; }
        }

        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public Cluster Cluster
        {
            get { return (Cluster)Parent; }
        }

        [XmlIgnore]
        public Domain Domain
        {
            get { return (Domain)Parent; }
        }

        [XmlIgnore]
        public Dictionary<string, UserGroupMembership> UserGroupMemberships
        {
            get { return GetChildren<UserGroupMembership>(); }
            set { SetChildren<UserGroupMembership>(value); }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors and initializer function

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public User()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public User(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public User(Cluster parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public User(Domain parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>User</b> objects.
        /// </summary>
        /// <param name="old">The <b>User</b> to copy from.</param>
        public User(User old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            this.title = String.Empty;
            this.firstName = String.Empty;
            this.middleName = String.Empty;
            this.lastName = String.Empty;
            this.gender = Registry.Gender.Male;
            this.nonValidatedEmail = String.Empty;
            this.email = String.Empty;
            this.dateOfBirth = new DateTime(1950, 1, 1);
            this.company = String.Empty;
            this.jobTitle = String.Empty;
            this.address = String.Empty;
            this.address2 = String.Empty;
            this.state = String.Empty;
            this.stateCode = String.Empty;
            this.city = String.Empty;
            this.country = String.Empty;
            this.countryCode = String.Empty;
            this.zipCode = String.Empty;
            this.workPhone = String.Empty;
            this.homePhone = String.Empty;
            this.cellPhone = String.Empty;
            this.timeZone = 0;
            this.integrated = false;
            this.ntlmUser = String.Empty;
            this.passwordHash = null;
            this.activationCode = String.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>User</b> object to create the deep copy from.</param>
        private void CopyMembers(User old)
        {
            this.title = old.title;
            this.firstName = old.firstName;
            this.middleName = old.middleName;
            this.lastName = old.lastName;
            this.gender = old.gender;
            this.nonValidatedEmail = old.nonValidatedEmail;
            this.email = old.email;
            this.dateOfBirth = old.dateOfBirth;
            this.company = old.company;
            this.jobTitle = old.jobTitle;
            this.address = old.address;
            this.address2 = old.address2;
            this.state = old.state;
            this.stateCode = old.stateCode;
            this.city = old.city;
            this.country = old.country;
            this.countryCode = old.countryCode;
            this.zipCode = old.zipCode;
            this.workPhone = old.workPhone;
            this.homePhone = old.homePhone;
            this.cellPhone = old.cellPhone;
            this.timeZone = old.timeZone;
            this.integrated = old.integrated;
            this.ntlmUser = old.ntlmUser;
            this.passwordHash = Jhu.Graywulf.Util.DeepCloner.CopyArray(old.passwordHash);
        }

        public override object Clone()
        {
            return new User(this);
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] 
            {
                typeof(UserGroupMembership),
            };
        }

        #endregion
        #region Authentication and Password Generation Logic

        /// <summary>
        /// Sets the users password by computing the password hash.
        /// </summary>
        /// <param name="password">Password string</param>
        /// <remarks>
        /// This function does not write into the database. Call <b>Save</b> in order
        /// to save the new password.
        /// </remarks>
        public void SetPassword(string password)
        {
            passwordHash = ComputePasswordHash(password);
        }

        /// <summary>
        /// Computes the SHA512 hash from a password string
        /// </summary>
        /// <param name="password">The password string.</param>
        /// <returns>The binary hash.</returns>
        public static byte[] ComputePasswordHash(string password)
        {
            HashAlgorithm hashalg = new SHA512Managed();
            return hashalg.ComputeHash(Encoding.Unicode.GetBytes(password));
        }

        #endregion
        #region Database mappings and MyDB


        /*
         * TODO: delete
        public DatabaseInstance GetUserDatabaseInstance(DatabaseVersion databaseVersion)
        {
            LoadUserDatabaseInstances(true);
            var udi = UserDatabaseInstances.Values.FirstOrDefault(i => i.DatabaseVersionReference.Guid == databaseVersion.Guid);

            if (udi != null)
            {
                return udi.DatabaseInstance;
            }
            else
            {
                return null;
            }
        }
        */

        #endregion
        #region Email functions

        public void GenerateActivationCode()
        {
            // By default, user account is set to inactive
            // Generate activation code
            Random rnd = new Random();
            string code = "";
            for (int i = 0; i < 10; i++)
            {
                code += rnd.Next(10).ToString();
            }

            activationCode = code;
        }

        #endregion

    }
}
