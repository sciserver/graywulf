using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Instantiated in AppDomains created by the AppDomainManager to
    /// communicate with the domain.
    /// </summary>
    class AppDomainHelper : MarshalByRefObject
    {
        private int id;

        public int ID
        {
            get { return id; }
        }

        public static AppDomainHelper CreateInstanceAndUnwrap(AppDomain ad)
        {
            var adh = (AppDomainHelper)ad.CreateInstanceAndUnwrap(
                typeof(AppDomainHelper).Assembly.FullName,
                typeof(AppDomainHelper).FullName);

            return adh;
        }

        public AppDomainHelper()
        {
            InitializeAppDomain();
        }

        private void InitializeMembers()
        {
            this.id = -1;
        }

        public override object InitializeLifetimeService()
        {
            // Prevent object from automatic destruction
            return null;
        }

        private void InitializeAppDomain()
        {
            var ad = AppDomain.CurrentDomain;
            id = ad.Id;
        }
        
        /// <summary>
        /// Returns the names of assemblies already loaded into the
        /// current AppDomain
        /// </summary>
        /// <returns></returns>
        public AssemblyName[] GetAssemblyNames()
        {
            var aa = AppDomain.CurrentDomain.GetAssemblies();

            var ans = new AssemblyName[aa.Length];
            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = aa[i].GetName();
            }

            return ans;
        }
    }
}
