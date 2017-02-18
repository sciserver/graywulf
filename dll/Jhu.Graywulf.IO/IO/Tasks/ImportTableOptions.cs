using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    [DataContract]
    public class ImportTableOptions
    {
        [IgnoreDataMember]
        private bool generateIdentityColumn;

        [DataMember]
        public bool GenerateIdentityColumn
        {
            get { return generateIdentityColumn; }
            set { generateIdentityColumn = value; }
        }

        public ImportTableOptions()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.generateIdentityColumn = false;
        }
    }
}
