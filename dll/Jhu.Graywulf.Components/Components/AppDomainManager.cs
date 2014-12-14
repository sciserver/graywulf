using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements logic to load assemblies into separate AppDomains.
    /// </summary>
    /// <remarks>
    /// The domains are picked from the already existing domains if
    /// the assemblies are already loaded there. A new AppDomain is
    /// created every time when an assembly has to be loaded that
    /// has an incompatible version with the already loaded ones.
    /// </remarks>
    public class AppDomainManager
    {
        #region Singleton

        private static AppDomainManager instance;

        public static AppDomainManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppDomainManager();
                }

                return instance;
            }
        }

        #endregion

        private class AppDomainHandle
        {
            public AppDomain AppDomain { get; set; }
            public AppDomainHelper Helper { get; set; }
        }

        private string baseDirectory;
        private Dictionary<int, AppDomainHandle> appDomains;

        public string BaseDirectory
        {
            get { return baseDirectory; }
            set { baseDirectory = value; }
        }

        private AppDomainManager()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.baseDirectory = Components.AppSettings.AssemblyPath;
            this.appDomains = new Dictionary<int, AppDomainHandle>();
        }

        /// <summary>
        /// Returns an AppDomain for the given type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <remarks>
        /// If the implementing assembly is not found in any of the AppDomains, a
        /// new AppDomain can be created.
        /// 
        /// This function is not safe, because it doesn't parse the typeName correctly!
        /// </remarks>
        public bool GetAppDomainForType(string typeName, bool create, out AppDomain appDomain)
        {
            // TODO: when looking up the appropriate directory, only assembly of the
            // generic type is taken into account, no generic parameters are
            // considered.

            var an = GetAssemblyNameFromTypeName(typeName);
            var assemblyName = new AssemblyName(an);
            return GetAppDomainForAssembly(assemblyName, create, out appDomain);
        }

        public bool GetAppDomainForAssembly(string assemblyName, bool create, out AppDomain appDomain)
        {
            var an = new AssemblyName(assemblyName);
            return GetAppDomainForAssembly(an, create, out appDomain);
        }

        public bool GetAppDomainForAssembly(AssemblyName assemblyName, bool create, out AppDomain appDomain)
        {
            appDomain = FindAppDomainWithAssembly(assemblyName);

            if (appDomain == null && create)
            {
                appDomain = CreateAppDomainForAssembly(assemblyName).AppDomain;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UnloadAppDomain(int id)
        {
            AppDomain.Unload(appDomains[id].AppDomain);
            appDomains.Remove(id);
        }

        /// <summary>
        /// Creates a new AppDomain with an application base containing the given assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private AppDomainHandle CreateAppDomainForAssembly(AssemblyName assemblyName)
        {
            var applicationBase = FindDirectoryWithAssembly(assemblyName);

            if (applicationBase == null)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

                throw new FileNotFoundException("Assembly not found.");     // *** TODO
            }

            return CreateAppDomain(applicationBase);
        }

        /// <summary>
        /// Returns the AppDomain already containg the given assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private AppDomain FindAppDomainWithAssembly(AssemblyName assemblyName)
        {
            // Check all domains to see whether assembly is already loaded
            foreach (var h in appDomains.Values)
            {
                var ans = h.Helper.GetAssemblyNames();

                for (int i = 0; i < ans.Length; i++)
                {
                    if (AssemblyNameComparer.Instance.Compare(ans[i], assemblyName) == 0)
                    {
                        // This AppDomain already contains the assembly
                        return h.AppDomain;
                    }
                }
            }

            // No AppDomains containing the given assembly found
            return null;
        }

        /// <summary>
        /// Look into each directory under binBasePath to find the required assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        /// <remarks>
        /// The function looks into the binBasePath directory and its first level
        /// subdirectories. If the assembly is found, no further checking is done,
        /// so every subdirectory must contain all referenced assemblies.
        /// </remarks>
        private string FindDirectoryWithAssembly(AssemblyName assemblyName)
        {
            // *** TODO: this might be wrong if we open another app domain from an
            // app domain opened by the domain manager because relative paths
            // might get incorrectly combined!

            // Because base directory might be relative to the current path
            // we have to combine it with the base path of the app domain
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDirectory);

            // See if the binBasePath contains the required assembly
            if (HasDirectoryAssembly(path, assemblyName))
            {
                return path;
            }

            // Check first level subdirectories
            foreach (var dir in Directory.GetDirectories(path))
            {
                if (HasDirectoryAssembly(dir, assemblyName))
                {
                    return dir;
                }
            }

            // No DLL containing the assembly found
            return null;
        }

        /// <summary>
        /// Checks if directory contains a given assembly
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private bool HasDirectoryAssembly(string path, AssemblyName assemblyName)
        {
            // TODO: add support to EXE files!
            foreach (var dll in Directory.GetFiles(path, "*.dll"))
            {
                AssemblyName an;
                
                // Try to get assembly name from the dll file
                // If the dll is not a .net assembly, it might fail
                try
                {
                    an = AssemblyName.GetAssemblyName(dll);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                if (AssemblyNameComparer.Instance.Compare(assemblyName, an) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private AppDomainHandle CreateAppDomain(string baseDirectory)
        {
            var friendlyName = String.Format("Graywulf_{0}", Guid.NewGuid());

            // Shadow copying assablies is necessary for the scheduler to work, however it is not supported
            // under Windows 2003. Since web servers still run this, a workaround has to be made here.
            string shadowcopy;
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT &&
                System.Environment.OSVersion.Version.Major >= 6)
            {
                shadowcopy = "true";
            }
            else
            {
                shadowcopy = "false";
            }

            var setup = new AppDomainSetup()
            {
                ApplicationBase = baseDirectory,
                ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                DisallowApplicationBaseProbing = false,
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                ShadowCopyFiles = shadowcopy,
                
                //CachePath = "",
                //DynamicBase = "",
                //PrivateBinPath = "",
                //PrivateBinPathProbe = "",
                //ShadowCopyDirectories = "",
            };

            // Create the domain
            var ad = AppDomain.CreateDomain(friendlyName, null, setup);

            // Create and unwrap the handler that allows interaction with the domain
            var helper = AppDomainHelper.CreateInstanceAndUnwrap(ad);

            // Create and return the handle to the new domain
            var handle = new AppDomainHandle()
            {
                AppDomain = ad,
                Helper = helper,
            };

            // Save handle for future use
            appDomains.Add(ad.Id, handle);

            return handle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string GetAssemblyNameFromTypeName(string typeName)
        {
            // This function supposed to work correctly as far as generic types concerned.

            // Find the first comma that's not escaped and not inside brackets
            var inEscape = false;
            int inBrackets = 0;

            for (int i = 0; i < typeName.Length; i++)
            {
                if (inEscape)
                {
                    inEscape = false;
                    continue;
                }

                switch (typeName[i])
                {
                    case '\\':
                        inEscape = true;
                        break;
                    case '[':
                        inBrackets++;
                        break;
                    case ']':
                        inBrackets--;
                        break;
                    case ',':
                        // Found the first comma
                        return typeName.Substring(i + 1).Trim();
                    case '+':
                    case '.':
                    default:
                        break;
                }
            }

            return null;
        }

    }
}
