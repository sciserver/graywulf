using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Check
{
    public class TypeCheck : CheckRoutineBase
    {
        private string typename;

        public TypeCheck(string typename)
        {
            this.typename = typename;
        }

        public override void Execute(System.IO.TextWriter output)
        {
            output.WriteLine("Creating plugin type {0}", typename);

            var t = Type.GetType(typename);
        }
    }
}
