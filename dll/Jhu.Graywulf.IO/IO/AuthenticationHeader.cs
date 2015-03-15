using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.IO
{
    [Serializable]
    [DataContract]
    public class AuthenticationHeader : ICloneable
    {
        #region Private member variables

        private string name;
        private string value;

        #endregion
        #region Properties

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion
        #region Constructors and initializers

        public AuthenticationHeader()
        {
            InitializeMembers();
        }

        public AuthenticationHeader(string name, string value)
        {
            InitializeMembers();

            this.name = name;
            this.value = value;
        }

        public AuthenticationHeader(AuthenticationHeader old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.value = null;
        }

        private void CopyMembers(AuthenticationHeader old)
        {
            this.name = old.name;
            this.value = old.value;
        }

        public object Clone()
        {
            return new AuthenticationHeader(this);
        }

        #endregion
    }
}
