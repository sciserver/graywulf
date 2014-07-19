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

        protected KeystoneClient Client
        {
            get
            {
                return new KeystoneClient(baseUri)
                {
                    AdminAuthToken = adminAuthToken
                };
            }
        }

        protected void PurgeTestUsers()
        {
            var users = Client.FindUsers("test*", false, false);
            for (int i = 0; i < users.Length; i++)
            {
                Client.DeleteUser(users[i].ID);
            }
        }

        protected User CreateTestUser(string name)
        {

            var user = new User()
            {
                Name = name,
                Description = "test user",
                Password = "alma"
            };

            return Client.CreateUser(user);
        }
    }
}
