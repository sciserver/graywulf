using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using Jhu.Graywulf.Web.Services;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Web.Api.V1
{
    public class ApiTestBase : TestClassBase
    {
        protected ApiTestBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected void AuthenticateTestUser(RestClientSession session)
        {
            AuthenticateUser(session, "test");
        }

        protected void AuthenticateUser(RestClientSession session, string user)
        {
            var auth = new AuthRequest()
            {
                Credentials = new Credentials()
                {
                    Username = user,
                    Password = "almafa"
                }
            };

            var authuri = String.Format("http://{0}{1}/api/v1/auth.svc", Environment.MachineName, Jhu.Graywulf.Test.AppSettings.WebAuthPath);
            var client = session.CreateClient<IAuthService>(new Uri(authuri));

            client.Authenticate(auth);
        }


    }
}
