using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services
{
    public class ServiceNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
