using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Jhu.Graywulf.Web.Services
{
    class RestHttpSessionStateWrapper : IRestSessionState
    {
        public RestHttpSessionStateWrapper()
        {
        }

        public object this[string key]
        {
            get { return HttpContext.Current.Session[key]; }
            set { HttpContext.Current.Session[key] = value; }
        }
    }
}
