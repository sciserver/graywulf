using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public class AssemblyCheck : CheckRoutineBase
    {
        private string baseDir;
        private Assembly assembly;

        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Assembly;
            }
        }

        public string BaseDir
        {
            get { return baseDir; }
            set { baseDir = value; }
        }

        public Assembly Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }

        public AssemblyCheck(string baseDir)
        {
            InitializeMembers();

            this.baseDir = baseDir;
        }

        public AssemblyCheck(string baseDir, Assembly assembly)
        {
            InitializeMembers();

            this.baseDir = baseDir;
            this.assembly = assembly;
        }

        private void InitializeMembers()
        {
            this.baseDir = null;
            this.assembly = null;
        }

        public override void Execute(TextWriter output)
        {
            output.WriteLine("Testing assembly: {0}", assembly.FullName);
            output.WriteLine("Discovering references...");

            string messages;

            var setup = new AppDomainSetup()
            {
                ApplicationBase = baseDir,
                ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                DisallowApplicationBaseProbing = false,
                DisallowBindingRedirects = true,
                DisallowCodeDownload = true,
                ShadowCopyFiles = "true",
            };
            var ad = AppDomain.CreateDomain("Domain fro AssemblyCheckHelper", null, setup);
            var ah = (AssemblyCheckHelper)ad.CreateInstanceAndUnwrap(
                typeof(AssemblyCheckHelper).Assembly.FullName,
                typeof(AssemblyCheckHelper).FullName,
                true,
                BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                null,
                null,
                null);

            var error = ah.Execute(assembly.FullName, baseDir, out messages);
            output.Write(messages);

            AppDomain.Unload(ad);
            
            if (error)
            {
                throw new Exception("Error resolving referenced assemblies.");
            }
        }
    }
}
