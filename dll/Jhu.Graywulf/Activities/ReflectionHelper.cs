using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// Queries information about types (workflows and activities)
    /// by loading them into a separate AppDomain
    /// </summary>
    [Serializable]
    public class ReflectionHelper : IDisposable
    {
        AppDomain appDomain;
        ReflectionHelperInternal rh;

        private string assemblyFilename;
        private string typeName;

        /// <summary>
        /// Gets or sets the full path to the assembly implementing the workflow.
        /// </summary>
        public string AssemblyFilename
        {
            get { return assemblyFilename; }
            set { assemblyFilename = value; }
        }

        /// <summary>
        /// Gets or sets the fully qualified type name of the workflow, i.e. with namespace information.
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public ReflectionHelper()
        {
            InitializeMembers();
        }

        public ReflectionHelper(string assemblyFilename, string typeName)
        {
            InitializeMembers();

            this.assemblyFilename = assemblyFilename;
            this.typeName = typeName;
        }

        private void InitializeMembers()
        {
            this.appDomain = null;
            this.rh = null;

            this.assemblyFilename = null;
            this.typeName = null;
        }

        private void CreateAppDomain()
        {
            if (appDomain == null)
            {
                AppDomainSetup setup = new AppDomainSetup();

                string filename = Assembly.GetExecutingAssembly().Location;
                setup.ApplicationBase = Path.GetDirectoryName(filename);
                setup.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                setup.DisallowApplicationBaseProbing = false;

                appDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setup);

                rh = (ReflectionHelperInternal)appDomain.CreateInstanceFromAndUnwrap(
                    typeof(ReflectionHelperInternal).Assembly.Location,
                    typeof(ReflectionHelperInternal).FullName,
                    false, BindingFlags.CreateInstance, null, null, null, null);
            }
        }

        private void DestroyAppDomain()
        {
            if (appDomain != null)
            {
                rh.Dispose();
                rh = null;
                AppDomain.Unload(appDomain);
            }
        }

        public Dictionary<string, Jhu.Graywulf.Registry.JobParameter> GetWorkflowParameters(string typeName)
        {
            CreateAppDomain();
            return rh.GetWorkflowParameters(typeName);
        }

        public List<string> GetWorkflowCheckpoints(string typeName)
        {
            CreateAppDomain();
            return rh.GetWorkflowCheckpoints(typeName);
        }

        public bool IsTypeOfInterface(string typeName, string interfaceName)
        {
            CreateAppDomain();
            return rh.IsTypeOfInterface(typeName, interfaceName);
        }

        public HashSet<string> GetTypeInheritedTypes(string typeName)
        {
            CreateAppDomain();
            return rh.GetWokflowParametersInheritedTypes(typeName);
        }

        public void Dispose()
        {
            DestroyAppDomain();
        }
    }
}
