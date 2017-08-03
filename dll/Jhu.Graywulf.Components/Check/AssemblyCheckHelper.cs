using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Jhu.Graywulf.Check
{
    public class AssemblyCheckHelper : MarshalByRefObject
    {
        private static readonly Regex gitHashRegex = new Regex(@"\(([0-9a-f]{40})\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string baseDir;
        private Dictionary<string, AssemblyName> references;

        public AssemblyCheckHelper()
        {   
        }
        
        public bool Execute(string assemblyName, string baseDir, out string messages)
        {
            var output = new StringWriter();
            bool error = false;

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AppDomain_ReflectionOnlyAssemblyResolve;

            this.baseDir = baseDir;
            var assembly = Assembly.ReflectionOnlyLoad(assemblyName);
            VerifyReferencedAssemblies(output, assembly, ref error);

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AppDomain_ReflectionOnlyAssemblyResolve;

            messages = output.ToString();
            return error;
        }

        private void VerifyReferencedAssemblies(TextWriter output, Assembly a, ref bool error)
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
                    var path = Path.Combine(baseDir, an.Name + ".dll");

                    try
                    {
                        if (File.Exists(path))
                        {
                            output.WriteLine("   Assembly found: {0}", path);

                            // Load as bytes to prevent locking the file
                            aa = Assembly.ReflectionOnlyLoad(System.IO.File.ReadAllBytes(path));
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
                            var eq = Components.AssemblyNameComparer.Instance.Compare(aan, an);

                            if (eq == 0)
                            {
                                output.WriteLine("   <font color=\"green\">OK:</font> Referenced and available assembly versions match.");
                            }
                            else if (eq < 0)
                            {
                                error = true;
                                output.WriteLine("   <font color=\"red\">Error:</font> Referenced and available versions don't match!");
                                output.WriteLine("   Referenced: {0}, Found: {1}", an.Version, aan.Version);
                            }
                            else
                            {
                                output.WriteLine("   <font color=\"blue\">Warning:</font> Available assembly is newer than the referenced version!");
                                output.WriteLine("   Referenced: {0}, Found: {1}", an.Version, aan.Version);
                            }
                        }

                        var cad = aa.GetCustomAttributesData();

                        if (cad != null)
                        {
                            var d = cad.Where(i => i.AttributeType.Name == "AssemblyTitleAttribute").FirstOrDefault();

                            if (d != null && d.ConstructorArguments.Count > 0 && d.ConstructorArguments[0].Value != null)
                            {
                                var desc = (string)d.ConstructorArguments[0].Value;
                                var m = gitHashRegex.Match(desc);

                                if (m.Success)
                                {
                                    output.WriteLine("   Git commit hash: {0}", m.Groups[1].Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        output.WriteLine("   <font color=\"red\">Error:</font> {0}", ex.Message);
                    }
                    finally
                    {

                    }

                    // Call recursively
                    if (aa != null)
                    {
                        VerifyReferencedAssemblies(output, aa, ref error);
                    }
                }
            }
        }

        private Assembly AppDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly aa;
            var an = new AssemblyName(args.Name);
            var path = Path.Combine(baseDir, an.Name + ".dll");

            if (File.Exists(path))
            {
                // Load as bytes to prevent locking the file
                aa = Assembly.ReflectionOnlyLoad(System.IO.File.ReadAllBytes(path));
            }
            else
            {
                aa = Assembly.ReflectionOnlyLoad(an.FullName);
            }

            return aa;
        }
    }
}
