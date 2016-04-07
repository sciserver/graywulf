using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.AccessControl;

namespace Jhu.Graywulf.Entities
{
    public class TestClassBase
    {
        protected Context CreateContext()
        {
            var context = new Context()
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=GraywulfEntitiesTest;Integrated Security=true",
                Identity = Identity.Guest
            };

            return context;
        }
    }
}
