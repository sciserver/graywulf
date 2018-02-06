using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    public class TableArchiveSettings : ICloneable
    {
        #region Private member variables

        [NonSerialized]
        private Uri uri;

        [NonSerialized]
        private Credentials credentials;

        [NonSerialized]
        private bool generateIdentityColumn;

        #endregion
        #region Properties

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Gets or sets the credentials to be used to access the source or destination URI
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        public bool GenerateIdentityColumn
        {
            get { return generateIdentityColumn; }
            set { generateIdentityColumn = value; }
        }

        #endregion
        #region Constructors and initializers

        public TableArchiveSettings()
        {
            InitializeMembers();
        }

        public TableArchiveSettings(TableArchiveSettings old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.credentials = null;
            this.generateIdentityColumn = false;
        }

        private void CopyMembers(TableArchiveSettings old)
        {
            this.uri = old.uri;
            this.credentials = old.credentials;
            this.generateIdentityColumn = old.generateIdentityColumn;
        }

        public object Clone()
        {
            return new TableArchiveSettings(this);
        }

        #endregion
    }
}
