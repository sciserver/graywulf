using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Jhu.Graywulf.Web.Services
{
    public class RestInstanceSessionState : IExtension<InstanceContext>, IRestSessionState
    {
        private Dictionary<string, object> items;

        public IDictionary<string, object> Items
        {
            get { return items; }
        }

        public object this[string key]
        {
            get
            {
                if (items.ContainsKey(key))
                {
                    return items[key];
                }
                else
                {
                    return null;
                }
            }
            set { items[key] = value; }
        }

        internal RestInstanceSessionState()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.items = new Dictionary<string, object>();
        }

        public void Attach(InstanceContext owner) { }

        public void Detach(InstanceContext owner) { }
    }
}
