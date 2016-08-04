using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Install
{
    public class LogInstaller : DBInstaller
    {
        public override void CreateSchema()
        {
            ExecuteSqlScript(Scripts.Jhu_Graywulf_Logging);
        }
    }
}
