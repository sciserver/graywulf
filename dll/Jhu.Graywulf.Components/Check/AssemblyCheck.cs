using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public class AssemblyCheck : CheckRoutineBase
    {
        private string baseDir;
        private Assembly assembly;
        private Dictionary<string, AssemblyName> references;

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

            var references = new Dictionary<string, AssemblyName>();

            VerifyReferencedAssemblies(output, assembly);
        }

        private void VerifyReferencedAssemblies(TextWriter output, Assembly a)
        {
            if (references == null)
            {
                references = new Dictionary<string, AssemblyName>();
            }

            foreach (var an in a.GetReferencedAssemblies())
            {
                if (!references.ContainsKey(an.FullName) &&
                    !Util.AssemblyReflector.IsSystem(an))
                {
                    references.Add(an.FullName, an);

                    output.WriteLine("+  Testing referenced assembly: {0}", an.FullName);

                    // TODO: what if reference is an exe?
                    Assembly aa = null;
                    var path = Path.Combine(BaseDir, an.Name + ".dll");

                    try
                    {
                        if (File.Exists(path))
                        {
                            output.WriteLine("   Assembly found: {0}", path);
                            aa = Assembly.ReflectionOnlyLoadFrom(path);
                        }
                        else
                        {
                            output.WriteLine("   <font color=\"blue\">Warning:</font> Assembly not found, will attempt automatic discovery.");
                            aa = Assembly.ReflectionOnlyLoad(an.FullName);
                            output.WriteLine("   Assembly found: {0}", aa.Location);
                        }

                        var aan = aa.GetName();

                        if (aan != null)
                        {
                            var eq = Components.AssemblyNameComparer.Instance.Compare(aa.GetName(), an) == 0;

                            if (eq)
                            {
                                output.WriteLine("   <font color=\"green\">OK:</font> Referenced and available assembly versions match.");
                            }
                            else
                            {
                                output.WriteLine("   <font color=\"red\">Error:</font> Referenced and available versions don't match!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine("   <font color=\"red\">Error:</font> {0}", ex.Message);
                    }

                    // Call recursively
                    if (aa != null)
                    {
                        VerifyReferencedAssemblies(output, aa);
                    }
                }
            }
        }
    }
}
