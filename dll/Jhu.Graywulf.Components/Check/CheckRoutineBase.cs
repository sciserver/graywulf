using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public abstract class CheckRoutineBase
    {
        public abstract void Execute(TextWriter output);
    }
}
