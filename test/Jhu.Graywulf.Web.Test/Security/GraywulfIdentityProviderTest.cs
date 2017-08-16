using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Web.Security
{
    [TestClass]
    public class GraywulfIdentityProviderTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            StopLogger();
        }

        [TestMethod]
        public void ManipulateUserTest()
        {
            using (var context = ContextManager.Instance.CreateContext(TransactionMode.ManualCommit))
            {
                var ip = new GraywulfIdentityProvider(context.Domain);

                // Create a new user and set password
                var user = new User(context.Domain);

                user.Name = "testuser";
                user.Email = "testuser@graywulf.org";

                ip.CreateUser(user, "alma");

                user.FirstName = "Modified";
                ip.ModifyUser(user);

                ip.ChangePassword(user, "alma", "alma2");

                Assert.IsTrue(user.DeploymentState == DeploymentState.Undeployed);

                ip.ActivateUser(user);

                Assert.IsTrue(user.DeploymentState == DeploymentState.Deployed);

                ip.DeactivateUser(user);

                Assert.IsTrue(user.DeploymentState == DeploymentState.Undeployed);

                ip.DeleteUser(user);
            }
        }
    }
}
