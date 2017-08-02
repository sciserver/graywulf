using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Jhu.Graywulf.ServiceModel
{
    public class LimitedAccessServiceExtension : IExtension<ServiceHostBase>
    {
        // TODO: this class now has to be populated manually
        // implement a config section to do it from config file

        private static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

        private Dictionary<string, HashSet<string>> roleList;
        private Dictionary<string, HashSet<string>> userList;

        internal Dictionary<string, HashSet<string>> RoleList
        {
            get { return roleList; }
        }

        internal Dictionary<string, HashSet<string>> UserList
        {
            get { return userList; }
        }

        public LimitedAccessServiceExtension()
        {
            this.roleList = new Dictionary<string, HashSet<string>>(Comparer);
            this.userList = new Dictionary<string, HashSet<string>>(Comparer);
        }

        public void AddRole(string category, string name)
        {
            Add(roleList, category, name);
        }

        public void AddUser(string category, string name)
        {
            Add(userList, category, name);
        }

        private void Add(Dictionary<string, HashSet<string>> list, string category, string name)
        {
            if (!list.ContainsKey(category))
            {
                list.Add(category, new HashSet<string>(Comparer));
            }

            if (!list[category].Contains(name))
            {
                list[category].Add(name);
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
