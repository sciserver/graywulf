using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestOperationContract : RestObject
    {
        private RestServiceContract service;
        private MethodInfo method;

        private string operationName;
        private string httpMethod;
        private RestUriTemplate uriTemplate;

        private RestMessageParameter bodyParameter;
        private RestMessageParameter returnParameter;
        private List<RestMessageParameter> parameters;

        public RestServiceContract Service
        {
            get { return service; }
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public string OperationName
        {
            get { return operationName; }
            internal set { operationName = value; }
        }

        public string HttpMethod
        {
            get { return httpMethod; }
            internal set { httpMethod = value; }
        }

        public RestUriTemplate UriTemplate
        {
            get { return uriTemplate; }
            internal set { uriTemplate = value; }
        }

        public RestMessageParameter BodyParameter
        {
            get { return bodyParameter; }
            internal set { bodyParameter = value; }
        }

        public RestMessageParameter ReturnParameter
        {
            get { return returnParameter; }
            internal set { returnParameter = value; }
        }

        public List<RestMessageParameter> Parameters
        {
            get { return parameters; }
        }

        public RestOperationContract(RestServiceContract service, MethodInfo method)
        {
            InitializeMembers();

            this.service = service;
            this.method = method;
        }

        private void InitializeMembers()
        {
            this.service = null;
            this.method = null;
            this.operationName = null;
            this.httpMethod = null;
            this.uriTemplate = null;
            this.bodyParameter = null;
            this.returnParameter = null;
            this.parameters = new List<RestMessageParameter>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__operationName__", operationName);
            script.Replace("__operationDescription__", Description);
            script.Replace("__httpMethod__", httpMethod);
            script.Replace("__uriTemplate__", uriTemplate.Value);
        }
    }
}
