using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SimpleRestClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class AuthServiceTest : ApiTestBase
    {
        private IAuthService Client
        {
            get
            {
                return CreateClient<IAuthService>(new Uri("http://localhost/gwauth/api/auth.svc"));
            }
        }

        [TestMethod]
        public void AuthenticateTest()
        {
            var auth = new AuthRequest()
            {
                Auth = new Auth()
                {
                     Username = "test",
                     Password = "alma"
                }
            };

            Client.Authenticate(auth);
        }
    }
}
