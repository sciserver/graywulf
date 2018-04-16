using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Install
{
    public class RegistryInstaller : DBInstaller
    {
        public RegistryInstaller()
        {
        }

        public RegistryInstaller(string connectionString)
            : base(connectionString)
        {
        }

        public override void CreateSchema()
        {
            ExecuteSqlScript(GetCreateAssemblyScript());
            ExecuteSqlScript(Scripts.Jhu_Graywulf_Registry_Tables);
            ExecuteSqlScript(Scripts.Jhu_Graywulf_Registry_Logic);
        }

        private string GetCreateAssemblyScript()
        {
            var sb = new StringBuilder(Scripts.Jhu_Graywulf_Registry_Assembly);

            // Find location of the assembly
            var filename = typeof(Jhu.Graywulf.Registry.EntityType).Assembly.Location;
            string hex, hash;

            GetFileAsHex(filename, out hex, out hash);
            sb.Replace("[$Hash]", hash);
            sb.Replace("[$Hex]", hex);
            
            return sb.ToString();
        }
    }
}
