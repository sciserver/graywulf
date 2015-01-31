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
        public static AppDomainHelper CreateInstanceAndUnwrap(AppDomain ad)
        {
            var adh = (AppDomainHelper)ad.CreateInstanceAndUnwrap(
                typeof(AppDomainHelper).Assembly.FullName,
                typeof(AppDomainHelper).FullName);

            adh.InitializeAppDomain();

            return adh;
        }

        public AppDomainHelper()
        {
        }

        public override object InitializeLifetimeService()
        {
            // Prevent object from automatic destruction
            return null;
        }

        public void InitializeAppDomain()
        {
            var ad = AppDomain.CurrentDomain;

            ad.UnhandledException += new UnhandledExceptionEventHandler(ad_UnhandledException);
        }

        #region AppDomain event handlers

        void ad_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            Console.WriteLine("UNHANDLED EXCEPTION");

            // **** TODO: handle app domain level exception here
            // to prevent failing of the entire scheduler
            throw (Exception)e.ExceptionObject;
        }

        #endregion

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
