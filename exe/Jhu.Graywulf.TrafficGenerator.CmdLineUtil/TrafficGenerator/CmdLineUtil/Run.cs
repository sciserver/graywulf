using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.TrafficGenerator.CmdLineUtil
{
    [Verb(Name = "Parse", Description = "Parses a sql script and extracts metadata.")]
    class Run : Verb
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

        public Run()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputFilename = null;
            this.outputFilename = null;
        }

        public override void Execute()
        {
        }
    }
}
