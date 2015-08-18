using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.SqlClrUtil
{
    class SqlClrReflector : IDisposable
    {
        private Dictionary<string, Assembly> assemblies;
        private List<SqlObject> objects;
        private Dictionary<Type, string> types;

        public Dictionary<string, Assembly> Assemblies
        {
            get { return assemblies; }
        }

        public List<SqlObject> Objects
        {
            get { return objects; }
        }

        public Dictionary<Type, string> Types
        {
            get { return types; }
        }

        public SqlClrReflector(Assembly assembly)
        {
            InitializeMembers();

            ReflectAssembly(assembly);

            foreach (var a in assemblies.Values)
            {
                Console.WriteLine("Using assembly: {0}", a.Location);
            }
        }

        private void InitializeMembers()
        {
            this.assemblies = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
            this.objects = new List<SqlObject>();
            this.types = new Dictionary<Type, string>(Constants.SqlTypes);
        }

        public void Dispose()
        {
        }

        private void ReflectAssembly(Assembly a)
        {
            CollectReferences(a);

            foreach (var type in a.GetTypes())
            {
                var obj = SqlObject.FromType(type);

                if (obj != null)
                {
                    objects.Add(obj);

                    if (obj is SqlUserDefinedType)
                    {
                        types.Add(type, ((SqlUserDefinedType)obj).GetSql());
                    }
                }
                else
                {
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        obj = SqlObject.FromMethod(method);
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                }
            }
        }

        private void CollectReferences(Assembly assembly)
        {
            var name = assembly.GetName().Name;

            if (!assemblies.ContainsKey(name))
            {
                foreach (var a in assembly.GetReferencedAssemblies())
                {
                    if (!a.Name.StartsWith("System") && !a.Name.StartsWith("ms"))
                    {
                        var dir = Path.GetDirectoryName(assembly.Location);
                        var aaa = LoadAssembly(dir, a);

                        if (aaa != null)
                        {
                            CollectReferences(aaa);
                        }
                    }
                }

                assemblies.Add(name, assembly);
            }
        }

        private Assembly LoadAssembly(string dir, AssemblyName name)
        {
            Assembly a = null;
            
            // Attempt default location
            try
            {
                a = Assembly.Load(name);
            }
            catch { }

            if (a == null)
            {
                // Try from the directory of referencing
                try
                {
                    a = Assembly.LoadFrom(Path.Combine(dir, name.Name + ".dll"));
                }
                catch { }
            }

            return a;
        }

        public void ScriptCreate(TextWriter writer)
        {
            foreach (var schema in CollectSchemaNames())
            {
                ScriptCreateSchema(writer, schema);
            }

            foreach (var a in assemblies.Values)
            {
                ScriptCreateAssembly(writer, a);
            }

            foreach (var obj in objects.OrderBy(i => i.Rank))
            {
                obj.ScriptCreate(this, writer);
            }
        }

        public void ScriptDrop(TextWriter writer)
        {
            foreach (var obj in objects.OrderByDescending(i => i.Rank))
            {
                obj.ScriptDrop(this, writer);
            }

            foreach (var a in assemblies.Values.Reverse())
            {
                ScriptDropAssembly(writer, a);
            }

            foreach (var schema in CollectSchemaNames())
            {
                ScriptDropSchema(writer, schema);
            }
        }

        private void ScriptCreateSchema(TextWriter writer, string schema)
        {
            if (!Constants.SystemSchemas.Contains(UnquoteIdentifier(schema)))
            {
                writer.Write(
@"
CREATE SCHEMA [{0}]

GO

",
                    schema);
            }
        }

        private void ScriptDropSchema(TextWriter writer, string schema)
        {
            if (!Constants.SystemSchemas.Contains(UnquoteIdentifier(schema)))
            {
                writer.Write(
@"
DROP SCHEMA [{0}]

GO

",
                   schema);
            }
        }

        private void ScriptCreateAssembly(TextWriter writer, Assembly a)
        {
            writer.Write(
@"
CREATE ASSEMBLY [{0}]
    AUTHORIZATION [dbo]
    FROM 0x",
                a.GetName().Name);

            using (var infile = new FileStream(a.Location, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[0x10000];
                int res;

                while ((res = infile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < res; i++)
                    {
                        writer.Write(BitConverter.ToString(buffer, i, 1));
                    }
                }
            }

            writer.WriteLine();
            writer.WriteLine("GO");
            writer.WriteLine();
            writer.WriteLine();
        }

        private void ScriptDropAssembly(TextWriter writer, Assembly a)
        {
            writer.Write(@"
DROP ASSEMBLY [{0}]

GO

",
                a.GetName().Name);  
        }

        private HashSet<string> CollectSchemaNames()
        {
            var res = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var obj in objects)
            {
                if (!res.Contains(obj.Schema))
                {
                    res.Add(obj.Schema);
                }
            }

            return res;
        }

        protected string UnquoteIdentifier(string identifier)
        {
            return identifier.Trim('[', ']', '"');
        }
    }
}
