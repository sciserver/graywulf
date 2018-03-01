using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// This activity does nothing but can be used as a placeholder for todos.
    /// </summary>
    public class PlaceholderActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
