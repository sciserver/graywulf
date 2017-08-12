using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Check
{
    public class TypeCheck : CheckRoutineBase
    {
        private string typename;

        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Assembly;
            }
        }

        public TypeCheck(string typename)
        {
            this.typename = typename;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Creating plugin type {0}", typename);
            var t = Type.GetType(typename);
            yield return ReportSuccess("Plugin type {0} created.", typename);
        }
    }
}
