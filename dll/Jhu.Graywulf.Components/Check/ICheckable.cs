using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Check
{
    public interface ICheckable
    {
        IList<CheckRoutineBase> GetCheckRoutines();
    }
}
