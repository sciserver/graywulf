using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Entities.AccessControl;

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

        protected static Identity CreateTestIdentity()
        {
            return new Identity()
            {
                IsAuthenticated = true,
                Name = TestUser
            };
        }

        protected static Identity CreateOtherIdentity()
        {
            return new Identity()
            {
                IsAuthenticated = true,
                Name = OtherUser
            };
        }

        protected static Context CreateContext()
        {
            var context = new Context()
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=GraywulfEntitiesTest;Integrated Security=true",
                Identity = CreateTestIdentity()
            };

            return context;
        }

        protected static void InitializeDatabase()
        {
            using (var context = CreateContext())
            {
                string script = File.ReadAllText(MapPath(@"sql\Graywulf.Entities.Test.sql"));
                context.ExecuteScriptNonQuery(script);
            }
        }
    }
}
