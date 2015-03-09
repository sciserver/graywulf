using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO
{
    public class AuthenticationHeaderCollection : Dictionary<string, AuthenticationHeader>
    {
        public AuthenticationHeaderCollection()
            :base(StringComparer.InvariantCultureIgnoreCase)
        {

        }

        public AuthenticationHeaderCollection(AuthenticationHeaderCollection headers)
            :base(headers, StringComparer.InvariantCultureIgnoreCase)
        {
        }


        public void Add(AuthenticationHeader header)
        {
            this.Add(header.Name, header);
        }

        public void AddRange(AuthenticationHeader[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                this.Add(headers[i]);
            }
        }

        public void AddRange(NameValueCollection headers)
        {
            foreach (string name in headers.Keys)
            {
                this.Add(new AuthenticationHeader(name, headers[name]));
            }
        }
    }
}
