using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Entities
{
    [Serializable]
    public abstract class ContextObject
    {
        [NonSerialized]
        private Context context;

        public Context Context
        {
            get { return context; }
            set { context = value; }
        }

        protected ContextObject()
        {
            InitializeMembers();
        }

        protected ContextObject(Context context)
        {
            InitializeMembers();

            this.context = context;
        }

        private void InitializeMembers()
        {
            this.context = null;
        }
    }
}
