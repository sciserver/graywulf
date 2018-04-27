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
    public class RestMessageParameter : RestObject
    {
        private RestOperationContract operation;
        private ParameterInfo parameter;
        private RestDataContract dataContract;

        private string parameterName;
        private string parameterDescription;

        public RestOperationContract Operation
        {
            get { return operation; }
        }

        public ParameterInfo Parameter
        {
            get { return parameter; }
        }

        public string ParameterName
        {
            get { return parameterName; }
            internal set { parameterName = value; }
        }

        public RestDataContract DataContract
        {
            get { return dataContract; }
            internal set { dataContract = value; }
        }
        
        public string ParameterDescription
        {
            get { return parameterDescription; }
            internal set { parameterDescription = value; }
        }

        public RestMessageParameter(RestOperationContract operation, ParameterInfo parameter)
        {
            InitializeMembers();

            this.operation = operation;
            this.parameter = parameter;
        }

        private void InitializeMembers()
        {
            this.operation = null;
            this.parameter = null;
            this.dataContract = null;
            this.parameterName = null;
            this.parameterDescription = null;
        }
        
        public bool IsQueryParameter()
        {
            return operation.UriTemplate.IsQueryParameter(parameterName);
        }

        public bool IsPathParameter()
        {
            return operation.UriTemplate.IsPathParameter(parameterName);
        }

        public bool IsBodyParameter()
        {
            return operation.UriTemplate.IsBodyParameter(parameterName);
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__parameterName__", parameterName);
            script.Replace("__parameterDescription__", parameterDescription);
        }
    }
}
