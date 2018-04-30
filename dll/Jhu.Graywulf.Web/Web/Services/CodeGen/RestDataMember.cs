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
    public class RestDataMember : RestObject
    {
        private RestDataContract dataContract;
        private PropertyInfo property;

        private string dataMemberName;

        public RestDataContract DataContract
        {
            get { return dataContract; }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public string DataMemberName
        {
            get { return dataMemberName; }
            internal set { dataMemberName = value; }
        }
        
        public RestDataMember(RestDataContract dataContract, PropertyInfo property)
        {
            InitializeMembers();

            this.dataContract = dataContract;
            this.property = property;
        }

        private void InitializeMembers()
        {
            this.dataContract = null;
            this.property = null;
            this.dataMemberName = null;
        }

        public override void SubstituteTokens(StringBuilder script)
        {
            script.Replace("__memberName__", dataMemberName);
            script.Replace("__memberDescription__", Description);
        }
    }
}
