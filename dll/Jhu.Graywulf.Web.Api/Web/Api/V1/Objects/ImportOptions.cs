using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Description("Represents a set of options for a table import job.")]
    public class ImportOptions
    {
        [DataMember(Name = "generateIdentityColumn", EmitDefaultValue = false)]
        [Description("True, if an identity column (primary key) is generated.")]
        [DefaultValue(false)]
        public bool GenerateIdentityColumn { get; set; }

        public ImportOptions()
        {
        }

        public ImportOptions(Jhu.Graywulf.IO.Tasks.ImportTableOptions options)
        {
            this.GenerateIdentityColumn = options.GenerateIdentityColumn;
        }

        public IO.Tasks.ImportTableOptions GetOptions()
        {
            var options = new IO.Tasks.ImportTableOptions()
            {
                GenerateIdentityColumn = GenerateIdentityColumn
            };

            return options;
        }
    }
}
