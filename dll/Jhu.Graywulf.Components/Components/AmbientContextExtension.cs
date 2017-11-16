using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Jhu.Graywulf.Components
{
    class AmbientContextExtension : IExtension<InstanceContext>
    {
        private AmbientContextStore store;

        public AmbientContextStore Store
        {
            get { return store; }
        }

        public AmbientContextExtension()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.store = new Components.AmbientContextStore();
        }

        public void Attach(InstanceContext owner)
        {
        }

        public void Detach(InstanceContext owner)
        {
        }
    }
}
