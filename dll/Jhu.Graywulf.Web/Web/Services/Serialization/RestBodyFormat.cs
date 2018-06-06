using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public class RestBodyFormat
    {
        private string name;
        private string extension;
        private string mimeType;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        public RestBodyFormat()
        {
            InitializeMembers();
        }

        public RestBodyFormat(string name, string extension, string mimeType)
        {
            InitializeMembers();

            this.name = name;
            this.extension = extension;
            this.mimeType = mimeType;
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.extension = null;
            this.mimeType = null;
        }
    }
}
