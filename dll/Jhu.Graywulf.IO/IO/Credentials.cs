using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.IO
{
    [Serializable]
    [DataContract(Name = "Credentials", Namespace = "")]
    public class Credentials : IDisposable
    {
        #region Private member variables

        /// <summary>
        /// Holds username used for authentication, if necessary
        /// </summary>
        private string userName;

        /// <summary>
        /// Holds password used for authentication, if necessary
        /// </summary>
        private string password;

        /// <summary>
        /// Holds a ticket that can be used to identify the user, if necessary
        /// </summary>
        private byte[] ticket;

        /// <summary>
        /// Holds headers used for authentication, if necessary
        /// </summary>
        private Dictionary<string, string> headers;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the username to access the URI
        /// </summary>
        [DataMember]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the password to access the URI
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Gets or sets the ticket used to access the URI
        /// </summary>
        [DataMember]
        public byte[] Ticket
        {
            get { return ticket; }
            set { ticket = value; }
        }

        /// <summary>
        /// Gets a collection of headers that is used to authenticate the
        /// request to access the URI
        /// </summary>
        [DataMember]
        public Dictionary<string, string> AuthenticationHeaders
        {
            get { return headers; }
        }

        #endregion
        #region Constructors and initializers

        public Credentials()
        {
            InitializeMembers(new StreamingContext());
        }

        public Credentials(Credentials old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.userName = null;
            this.password = null;
            this.ticket = null;
            this.headers = new Dictionary<string, string>();
        }

        private void CopyMembers(Credentials old)
        {
            this.userName = old.userName;
            this.password = old.password;
            this.ticket = old.ticket;
            this.headers = Util.DeepCloner.CloneDictionary(old.headers);
        }

        public object Clone()
        {
            return new Credentials(this);
        }

        public void Dispose()
        {
        }

        #endregion

        /// <summary>
        /// Gets username and password as network credentials for HTTP, FTP etc.
        /// </summary>
        /// <returns></returns>
        public ICredentials GetNetworkCredentials()
        {
            // TODO: Hopefully credentials from URIs are read automatically, test
            if (userName != null)
            {
                return new NetworkCredential(userName, password);
            }
            else
            {
                return null;
            }
        }

        public WebHeaderCollection GetWebHeaders()
        {
            if (headers != null && headers.Count > 0)
            {
                var whc = new WebHeaderCollection();

                foreach (string name in headers.Keys)
                {
                    whc.Add(name, headers[name]);
                }

                return whc;
            }
            else
            {
                return null;
            }

        }
    }
}
