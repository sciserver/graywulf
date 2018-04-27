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
        private Type type;

        private string dataContractName;
        private string dataContractDescription;

        private Dictionary<string, RestDataMember> dataMembers;

        public Type Type
        {
            get { return type; }
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

        public RestDataContract(Type type)
        {
            InitializeMembers();

            this.type = type;
        }

        private void InitializeMembers()
        {
            this.type = null;
            this.dataContractName = null;
            this.dataContractDescription = null;
            this.dataMembers = new Dictionary<string, RestDataMember>();
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            throw new NotImplementedException();
        }
    }
}
