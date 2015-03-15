using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class AuthServiceTest : ApiTestBase
    {
        [TestMethod]
        public void AuthenticateTest()
        {
            using (var session = new RestClientSession())
            {
                AuthenticateTestUser(session);
            }
        }
    }
}
