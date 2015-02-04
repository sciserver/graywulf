using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Metadata.CmdLineUtil
{
    [Verb(Name = "Parse", Description = "Parses a sql script and extracts metadata.")]
    class Parse : Verb
    {
        private string inputFilename;
        private string outputFilename;

        [Parameter(Name = "Input", Description = "Input file", Required = true)]
        public string InputFilename
        {
            get { return inputFilename; }
            set { inputFilename = value; }
        }

        [Parameter(Name = "Output", Description = "Output file", Required = false)]
        public string OutputFilename
        {
            get { return outputFilename; }
            set { outputFilename = value; }
        }

        public Parse()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputFilename = null;
            this.outputFilename = null;
        }

        public override void Run()
        {
            

            // Load input
            Console.Write("Parsing SQL script... ");

            var meta = File.ReadAllText(inputFilename);

            var p = new Parser();
            var xml = p.Parse(meta);
            
            Console.WriteLine("done.");

            string filename;
            if (String.IsNullOrEmpty(outputFilename))
            {
                filename = inputFilename + ".xml";
            }
            else
            {
                filename = outputFilename;
            }

            Console.Write("Saving XML... ");

            xml.Save(filename);

            Console.WriteLine("done.");
        }
    }
}
