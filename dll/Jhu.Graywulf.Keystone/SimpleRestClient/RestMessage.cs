using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SimpleRestClient
{
    public class RestMessage<T>
    {
        private T body;
        private RestHeaderCollection headers;

        public T Body
        {
            get { return body; }
            set { body = value; }
        }

        public RestHeaderCollection Headers
        {
            get { return headers; }
            internal set { headers = value; }
        }

        public RestMessage()
        {
            InitializeMembers();
        }

        public RestMessage(T body, RestHeaderCollection headers)
        {
            InitializeMembers();

            this.body = body;
            this.headers = headers;
        }

        public RestMessage(T body, params RestHeader[] headers)
        {
            InitializeMembers();

            this.body = body;

            for (int i = 0; i < headers.Length; i++)
            {
                this.headers.Add(headers[i]);
            }
        }

        private void InitializeMembers()
        {
            this.body = default(T);
            this.headers = new RestHeaderCollection();
        }
    }
}
