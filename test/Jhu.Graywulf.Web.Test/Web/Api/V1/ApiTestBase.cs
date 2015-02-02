using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    public class ApiTestBase
    {
        protected ApiTestBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected void AuthenticateUser(RestClientSession session)
        {
            var auth = new AuthRequest()
            {
                Credentials = new Credentials()
                {
                    Username = "test",
                    Password = "alma"
                }
            };

            var client = session.CreateClient<IAuthService>(new Uri("http://localhost/gwauth/api/v1/auth.svc"));

            client.Authenticate(auth);
        }


    }
}
