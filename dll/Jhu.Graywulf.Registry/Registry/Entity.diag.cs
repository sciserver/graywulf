using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity
    {
        public virtual IList<DiagnosticMessage> RunDiagnostics()
        {
            return new List<DiagnosticMessage>();
        }
    }
}
