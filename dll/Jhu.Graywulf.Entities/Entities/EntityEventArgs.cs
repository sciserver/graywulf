using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities
{
    public class EntityEventArgs
    {
        private bool cancel;
        private object state;

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        public object State
        {
            get { return state; }
            set { state = value; }
        }

        public EntityEventArgs()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.cancel = false;
            this.state = null;
        }
    }
}
