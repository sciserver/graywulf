using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestDataContract : RestObject
    {
        private RestApi api;
        private Type type;
        private Type elementType;

        private string dataContractName;
        private string dataContractDescription;

        private Dictionary<string, RestDataMember> dataMembers;

        public RestApi Api
        {
            get { return api; }
        }

        public Type Type
        {
            get { return type; }
        }

        public Type ElementType
        {
            get { return elementType; }
            internal set { elementType = value; }
        }

        public string DataContractName
        {
            get { return dataContractName; }
            internal set { dataContractName = value; }
        }

        public string DataContractDescription
        {
            get { return dataContractDescription; }
            internal set { dataContractDescription = value; }
        }

        public Dictionary<string, RestDataMember> DataMembers
        {
            get { return dataMembers; }
        }

        public RestDataContract(RestApi api, Type type)
        {
            InitializeMembers();

            this.api = api;
            this.type = type;
        }

        private void InitializeMembers()
        {
            this.api = null;
            this.type = null;
            this.elementType = null;
            this.dataContractName = null;
            this.dataContractDescription = null;
            this.dataMembers = new Dictionary<string, RestDataMember>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__className__", dataContractName);
            script.Replace("__classDescription__", dataContractDescription);
        }
    }
}
