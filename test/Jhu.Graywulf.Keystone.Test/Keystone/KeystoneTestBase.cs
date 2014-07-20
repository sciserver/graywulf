using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Keystone
{
    public class KeystoneTestBase
    {
        private Uri baseUri = new Uri("http://192.168.170.50:5000");
        private string adminAuthToken = "e5b19f25f5d55a995a16";
        private KeystoneClient client;

        protected KeystoneClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new KeystoneClient(baseUri);
                    client.AdminAuthToken = adminAuthToken;
                }

                return client;
            }
        }

        protected void PurgeTestEntities()
        {
            var domains = Client.ListDomains();
            for (int i = 0; i < domains.Length; i++)
            {
                if (domains[i].Name.StartsWith("test"))
                {
                    Client.Delete(domains[i]);
                }
            }

            var projects = Client.ListProjects();
            for (int i = 0; i < projects.Length; i++)
            {
                if (projects[i].Name.StartsWith("test"))
                {
                    Client.Delete(projects[i]);
                }
            }

            var roles = Client.ListRoles();
            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i].Name.StartsWith("test"))
                {
                    Client.Delete(roles[i]);
                }
            }

            var users = Client.FindUsers("test*", false, false);
            for (int i = 0; i < users.Length; i++)
            {
                Client.Delete(users[i]);
            }
        }

        protected Domain CreateTestDomain()
        {
            var domain = new Domain()
            {
                Name = "test_domain",
                Description = "test domain"
            };

            return Client.Create(domain);
        }

        protected Project CreateTestProject()
        {
            var project = new Project()
            {
                Name = "test_project",
                Description = "test project",
            };

            return Client.Create(project);
        }

        protected Role CreateTestRole()
        {
            var role = new Role()
            {
                Name = "test_role",
                Description = "test role",
            };

            return Client.Create(role);
        }

        protected User CreateTestUser(string name)
        {

            var user = new User()
            {
                Name = name,
                Description = "test user",
                Password = "alma",
            };

            return Client.Create(user);
        }
    }
}
