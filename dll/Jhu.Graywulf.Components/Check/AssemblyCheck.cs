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
        private string path;
        private AssemblyName name;

        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Assembly;
            }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public AssemblyName Name
        {
            get { return name; }
            set { name = value; }
        }

        public AssemblyCheck(string path)
        {
            InitializeMembers();

            this.path = path;
        }

        public AssemblyCheck(string path, AssemblyName name)
        {
            InitializeMembers();

            this.path = path;
            this.name = name;
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.name = null;
        }

        public override void Execute(TextWriter output)
        {
            output.WriteLine(
                "Testing assembly: {0}",
                Path);

            var a = Assembly.ReflectionOnlyLoadFrom(Path);
            output.WriteLine("Assembly found: {0}", a.FullName);

            if (name != null)
            {
                var eq = Components.AssemblyNameComparer.Instance.Compare(a.GetName(), name) == 0;

                if (eq)
                {
                    output.WriteLine("Referenced and available assembly versions match.");
                }
                else
                {
                    output.WriteLine("Referenced and available versions don't match!");
                    throw new Exception("Assembly version mismatch.");  // *****
                }
            }
        }
    }
}
