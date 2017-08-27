using System;
using System.IO;
using System.Reflection;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Parsing.Generator.CmdLineUtil
{
    [Verb(Name = "Generate", Description = "Generates a parser from a BNF and token file.")]
    class Generate
    {
        private string grammarAssembly;
        private string grammarType;
        private string output;

        [Parameter(Name = "Assembly", Description = "Grammar assembly", Required = true)]
        public string GrammarAssembly
        {
            get { return grammarAssembly; }
            set { grammarAssembly = value; }
        }

        [Parameter(Name = "Type", Description = "Grammar type", Required = true)]
        public string GrammarType
        {
            get { return grammarType; }
            set { grammarType = value; }
        }

        [Parameter(Name = "Output", Description = "Output file", Required = true)]
        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        public Generate()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.grammarAssembly = null;
            this.grammarType = null;
            this.output = null;
        }

        public void Run()
        {
            // Start parsing and record time
            DateTime start = DateTime.Now;

            using (var outfile = new StreamWriter(output))
            {
                var a = Assembly.LoadFrom(grammarAssembly);
                var t = a.GetType(grammarType);

                if (t == null)
                {
                    throw new Exception(String.Format("Type '{0}' not found in assembly '{1}'.", grammarType, a.FullName));
                }

                var g = new ParserGenerator();
                g.Execute(outfile, t);
            }

            Console.WriteLine("Parser classes generated in {0} sec.", (DateTime.Now - start).TotalSeconds);
        }
    }
}
