using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestApi : RestObject
    {
        private string hostName;
        private string basePath;
        private string title;
        private string description;
        private string version;

        private Dictionary<Type, RestServiceContract> serviceContracts;
        private Dictionary<Type, RestDataContract> dataContracts;

        public Dictionary<Type, RestServiceContract> ServiceContracts
        {
            get { return serviceContracts; }
        }

        public Dictionary<Type, RestDataContract> DataContracts
        {
            get { return dataContracts; }
        }

        public RestApi()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.hostName = "localhost";
            this.basePath = "Api/V1/";
            this.title = null;
            this.description = null;
            this.version = null;

            this.serviceContracts = new Dictionary<Type, RestServiceContract>();
            this.dataContracts = new Dictionary<Type, RestDataContract>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__hostName__", hostName);
            script.Replace("__basePath__", basePath);
            script.Replace("__apiTitle__", title);
            script.Replace("__apiDescription__", description);
            script.Replace("__apiVersion__", version);
        }
    }
}
