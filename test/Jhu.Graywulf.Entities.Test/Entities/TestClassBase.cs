using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Entities
{
    public class TestClassBase
    {
        protected const string TestUser = "test";
        protected const string OtherUser = "other";

        protected static string MapPath(string path)
        {
            return Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\", path);
        }

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

        protected static Principal CreateOtherPrincipal()
        {
            return new Principal()
            {
                Identity = new Identity()
                {
                    IsAuthenticated = true,
                    Name = OtherUser
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

        protected static Context CreateContext()
        {
            var context = new Context()
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=GraywulfEntitiesTest;Integrated Security=true",
                Principal = CreateTestPrincipal()
            };

            return context;
        }

        protected static void InitializeDatabase()
        {
            using (var context = CreateContext())
            {
                string script = File.ReadAllText(MapPath(@"sql\Jhu.Graywulf.Entities.Test.sql"));
                context.ExecuteScriptNonQuery(script);
            }
        }
    }
}
