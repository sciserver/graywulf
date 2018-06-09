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

        private bool isClass;
        private bool isDictionary;
        private bool isEnum;
        private string dataContractName;

        private Dictionary<string, RestDataMember> dataMembers;

        public RestApi Api
        {
            get { return api; }
        }

        public Type Type
        {
            get { return type; }
        }

        public bool IsClass
        {
            get { return isClass; }
            internal set { isClass = value; }
        }

        public bool IsDictionary
        {
            get { return isDictionary; }
            internal set { isDictionary = value; }
        }

        public bool IsEnum
        {
            get { return isEnum; }
            internal set { isEnum = value; }
        }

        public string DataContractName
        {
            get { return dataContractName; }
            internal set { dataContractName = value; }
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
            this.isClass = false;
            this.isEnum = false;
            this.dataContractName = null;
            this.dataMembers = new Dictionary<string, RestDataMember>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__className__", dataContractName);
            script.Replace("__classDescription__", Description);
        }
    }
}
