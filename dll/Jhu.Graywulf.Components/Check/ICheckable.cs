using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public interface ICheckable
    {
        IEnumerable<CheckRoutineBase> GetCheckRoutines();
    }
}
