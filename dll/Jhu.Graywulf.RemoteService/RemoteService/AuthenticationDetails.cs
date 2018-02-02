using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.RemoteService
{
    public class AuthenticationDetails
    {
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AuthenticationType { get; set; }
    }
}
