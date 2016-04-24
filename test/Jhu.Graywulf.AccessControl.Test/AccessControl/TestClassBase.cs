using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.AccessControl
{
    public class TestClassBase
    {
        protected const string TestUser = "test";

        protected static Principal CreateTestPrincipal()
        {
            return new Principal()
            {
                Identity = new Identity()
                {
                    IsAuthenticated = true,
                    Name = TestUser
                }
            };
        }

        protected static Principal CreateAnonPrincipal()
        {
            return new Principal()
            {
                Identity = new Identity()
                {
                    IsAuthenticated = false,
                }
            };
        }
    }
}
