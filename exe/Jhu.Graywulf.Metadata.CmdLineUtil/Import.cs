using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Metadata.CmdLineUtil
{
    [Verb(Name = "Import", Description = "Imports metadata from xml into the database.")]
    class Import : DatabaseVerbBase
    {
        private string inputFilename;

        [Parameter(Name = "Input", Description = "Input file", Required = true)]
        public string InputFilename
        {
            get { return inputFilename; }
            set { inputFilename = value; }
        }

        public Import()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputFilename = null;
        }

        public override void Run()
        {
            XmlDocument xml;
            var meta = File.ReadAllText(inputFilename);

            // Load input
            if (Path.GetExtension(inputFilename) == ".sql")
            {
                Console.Write("Parsing SQL script... ");

                var p = new Parser();
                xml = p.Parse(meta);
                
                Console.WriteLine("done.");
            }
            else if (Path.GetExtension(inputFilename) == ".xml")
            {
                Console.Write("Loading XML... ");

                xml = new XmlDocument();
                xml.LoadXml(meta);

                Console.WriteLine("done.");
            }
            else
            {
                throw new Exception("Unknown file extension");
            }

            var g = new Generator(GetConnectionString());
            g.LoadXml(xml);

            Console.Write("Creating metadata... ");
            g.CreateMetadata();
            Console.WriteLine("done.");
        }
    }
}
