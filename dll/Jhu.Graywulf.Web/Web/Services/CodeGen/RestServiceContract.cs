using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceModel;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestServiceContract : RestObject
    {
        private RestApi api;
        private Type type;

        private string serviceName;
        private string serviceUrl;
        private string serviceVersion;
        private string serviceDescription;

        private Dictionary<string, Dictionary<string, RestOperationContract>> uriTemplates;

        public RestApi Api
        {
            get { return api; }
        }

        public Type Type
        {
            get { return type; }
        }

        public string ServiceName
        {
            get { return serviceName; }
            internal set { serviceName = value; }
        }

        public string ServiceUrl
        {
            get { return serviceUrl; }
            internal set { serviceUrl = value; }
        }

        public string ServiceVersion
        {
            get { return serviceVersion; }
            internal set { serviceVersion = value; }
        }

        public string ServiceDescription
        {
            get { return serviceDescription; }
            internal set { serviceDescription = value; }
        }

        public Dictionary<string, Dictionary<string, RestOperationContract>> UriTemplates
        {
            get { return uriTemplates; }
        }

        public RestServiceContract(RestApi api, Type serviceContract)
        {
            InitializeMembers();

            this.api = api;
            this.type = serviceContract;
        }

        private void InitializeMembers()
        {
            this.api = null;
            this.type = null;
            this.serviceName = null;
            this.serviceUrl = null;
            this.serviceVersion = null;
            this.serviceDescription = null;
            this.uriTemplates = new Dictionary<string, Dictionary<string, RestOperationContract>>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__serviceName__", serviceName);
            script.Replace("__serviceUrl__", serviceUrl);
            script.Replace("__serviceVersion__", serviceVersion);
            script.Replace("__serviceDescription__", serviceDescription);
        }
    }
}
