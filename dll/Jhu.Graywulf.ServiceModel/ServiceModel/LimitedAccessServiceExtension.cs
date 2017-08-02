using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ServiceModel;

namespace Jhu.Graywulf.ServiceModel
{
    class LimitedAccessServiceExtension : IExtension<ServiceHostBase>
    {
        // TODO: this class now has to be populated manually
        // implement a config section to do it from config file

        private static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

        private Dictionary<string, HashSet<string>> groupList;
        private Dictionary<string, HashSet<string>> userList;

        internal Dictionary<string, HashSet<string>> GroupList
        {
            get { return groupList; }
        }

        internal Dictionary<string, HashSet<string>> UserList
        {
            get { return userList; }
        }

        public LimitedAccessServiceExtension()
        {
            this.groupList = new Dictionary<string, HashSet<string>>(Comparer);
            this.userList = new Dictionary<string, HashSet<string>>(Comparer);
        }

        public void Init(string configSection)
        {
            var config = (ITcpEndpointConfiguration)ConfigurationManager.GetSection(configSection);

            foreach (TcpEndpointConfiguration.LimitedAccessRole category in config.Endpoint.AccessRoles)
            {
                AddList(groupList, category.Name, category.Groups);
                AddList(userList, category.Name, category.Users);
            }
        }

        public void AddGroup(string role, string name)
        {
            Add(groupList, role, name);
        }

        public void AddUser(string role, string name)
        {
            Add(userList, role, name);
        }

        private void Add(Dictionary<string, HashSet<string>> list, string role, string name)
        {
            if (!list.ContainsKey(role))
            {
                list.Add(role, new HashSet<string>(Comparer));
            }

            if (!list[role].Contains(name))
            {
                list[role].Add(name);
            }
        }

        private void AddList(Dictionary<string, HashSet<string>> list, string role,  string entries)
        {
            if (entries != null)
            {
                var parts = entries.Split(',');

                for (int i = 0; i < parts.Length; i++)
                {
                    Add(list, role, parts[i].Trim());
                }
            }
        }

        public void Attach(ServiceHostBase owner)
        {
        }

        public void Detach(ServiceHostBase owner)
        {
        }
    }
}
