using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public abstract class RestObject
    {
        private string description;

        public string Description
        {
            get { return description; }
            internal set { description = value; }
        }

        protected RestObject()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.description = null;
        }

        public abstract void SubstituteTokens(StringBuilder script);
    }
}
