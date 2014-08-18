using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.ConfigUtil
{
    [Verb(Name = "Configure", Description = "Merge configuration files for a given project.")]
    class Configure : Verb
    {
        private string rootPath;
        private string configurationsFile;
        private string configurationRoot;
        private string projectName;

        private string projectPath;
        private ProjectType projectType;
        private string[] mergeList;

        [Parameter(Name = "RootPath", Description = "Generally the path to the solution's root.", Required = false)]
        public string RootPath
        {
            get { return rootPath; }
            set { rootPath = value; }
        }

        [Parameter(Name = "ConfigurationsFile", Description = "A file listing the merge info.", Required = true)]
        public string ConfigurationsFile
        {
            get { return configurationsFile; }
            set { configurationsFile = value; }
        }

        [Parameter(Name = "ProjectName", Description = "A project name listed in Configurations.xml", Required = true)]
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        public Configure()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.rootPath = String.Empty;
            this.projectName = null;
        }

        public override void Run()
        {
            ReadProjectSettings();

            var orig = ReadOriginalConfig();

            foreach (string m in mergeList)
            {
                var merge = ReadMergeConfig(m);

                //
                Console.WriteLine("Merging '{0}' into '{1}'.", m, GetOriginalConfigPath());
                Util.ConfigXmlMerger.Merge(orig, merge);
            }

            WriteTargetConfig(orig);
        }

        private void ReadProjectSettings()
        {
            // Load xml file
            var conf = new XmlDocument();
            conf.Load(Path.Combine(rootPath, configurationsFile));

            // Fing config tag
            var root = conf.DocumentElement;
            configurationRoot = root.Attributes["root"].Value;

            // Find project node
            var node = root.SelectSingleNode(String.Format("projects/project[@name='{0}']", projectName));

            projectPath = node.Attributes["path"].Value;
            if (!Enum.TryParse<ProjectType>(node.Attributes["type"].Value, true, out projectType))
            {
                throw new Exception();
            }

            // Get merge nodes
            var ms = node.SelectNodes(".//merge");

            mergeList = new string[ms.Count];
            for (int i = 0; i < ms.Count; i++)
            {
                mergeList[i] = ms[i].Attributes["file"].Value;
            }
        }

        private string GetOriginalConfigPath()
        {
            return Path.Combine(rootPath, projectPath, Constants.OriginalConfigFileNames[projectType]);
        }

        private XmlDocument ReadOriginalConfig()
        {
            var config = new XmlDocument();
            config.Load(GetOriginalConfigPath());

            return config;
        }

        private void WriteTargetConfig(XmlDocument target)
        {
            target.Save(Path.Combine(rootPath, projectPath, Constants.TargetConfigFileNames[projectType]));
        }

        private XmlDocument ReadMergeConfig(string merge)
        {
            var config = new XmlDocument();
            config.Load(Path.Combine(rootPath, configurationRoot, merge));

            return config;
        }
    }
}
