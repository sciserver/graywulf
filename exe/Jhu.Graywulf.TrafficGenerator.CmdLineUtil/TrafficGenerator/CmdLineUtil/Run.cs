using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.TrafficGenerator.CmdLineUtil
{
    [Verb(Name = "Run", Description = "Runs jobs based on a traffic script.")]
    class Run : Verb
    {
        private string inputFilename;

        [Parameter(Name = "Input", Description = "Input file", Required = true)]
        public string InputFilename
        {
            get { return inputFilename; }
            set { inputFilename = value; }
        }

        public Run()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputFilename = null;
        }

        public override void Execute()
        {
        }
    }
}
