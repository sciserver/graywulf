using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SimpleRestClient
{
    public class RestHeader
    {
        private string name;
        private string value;

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public RestHeader(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
