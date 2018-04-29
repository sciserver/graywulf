using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services
{
    public class TestItemFormatterAttribute : StreamingRawFormatAttribute
    {
        protected override StreamingRawFormatterBase OnCreateFormatter()
        {
            return new TestItemFormatter();
        }
    }
}
