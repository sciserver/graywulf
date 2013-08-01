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
        private static StringComparer comparer = StringComparer.InvariantCulture;

        private string rootPath;
        private string configurationsFile;
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

            XmlDocument orig = ReadOriginalConfig();

            foreach (string m in mergeList)
            {
                XmlDocument merge = ReadMergeConfig(m);

                MergeConfigs(orig, merge);
            }

            WriteTargetConfig(orig);
        }

        private void ReadProjectSettings()
        {
            // Load xml file
            XmlDocument conf = new XmlDocument();
            conf.Load(Path.Combine(rootPath, configurationsFile));

            // Find project node
            XmlNode node = conf.SelectSingleNode(String.Format("/projects/project[@name='{0}']", projectName));

            projectPath = node.Attributes["path"].Value;
            if (!Enum.TryParse<ProjectType>(node.Attributes["type"].Value, true, out projectType))
            {
                throw new Exception();
            }

            // Get merge nodes
            XmlNodeList ms = node.SelectNodes(".//merge");

            mergeList = new string[ms.Count];
            for (int i = 0; i < ms.Count; i++)
            {
                mergeList[i] = ms[i].Attributes["file"].Value;
            }
        }

        private XmlDocument ReadOriginalConfig()
        {
            XmlDocument config = new XmlDocument();
            config.Load(Path.Combine(rootPath, projectPath, Constants.OriginalConfigFileNames[projectType]));

            return config;
        }

        private void WriteTargetConfig(XmlDocument target)
        {
            target.Save(Path.Combine(rootPath, projectPath, Constants.TargetConfigFileNames[projectType]));
        }

        private XmlDocument ReadMergeConfig(string merge)
        {
            XmlDocument config = new XmlDocument();
            config.Load(Path.Combine(rootPath, merge));

            return config;
        }

        private void MergeConfigs(XmlDocument orig, XmlDocument merge)
        {
            MergeNodes(orig.DocumentElement, merge.DocumentElement);
        }

        private void MergeNodes(XmlNode orig, XmlNode merge)
        {
            // This can happen in case of the root nodes only
            if (comparer.Compare(orig.Name, merge.Name) != 0)
            {
                throw new Exception("Incompatible nodes.");
            }

            MergeAttributes(orig, merge);
            MergeChildNodes(orig, merge);
        }

        private void MergeAttributes(XmlNode orig, XmlNode merge)
        {
            foreach (XmlAttribute ma in merge.Attributes)
            {
                // Include or overwrite
                XmlAttribute oa = orig.Attributes[ma.Name];
                if (oa == null)
                {
                    // Include
                    XmlAttribute na = orig.OwnerDocument.CreateAttribute(ma.Name);
                    na.Value = ma.Value;

                    orig.Attributes.Append(na);
                }
                else
                {
                    // Overwrite value
                    oa.Value = ma.Value;
                }
            }
        }

        private void MergeChildNodes(XmlNode orig, XmlNode merge)
        {
            foreach (XmlNode m in merge.ChildNodes)
            {
                // Find possible matching nodes in original
                HashSet<XmlNode> all = new HashSet<XmlNode>();
                HashSet<XmlNode> matching = new HashSet<XmlNode>();

                all.UnionWith(orig.SelectNodes(String.Format("./{0}", m.Name)).Cast<XmlNode>());

                if (all.Count == 0)
                {
                    // Simple case, merge in the whole branch
                    MergeInWholeBranch(orig, m);
                }
                else
                {
                    // If all specified attributes match
                    foreach (XmlNode on in all)
                    {
                        bool goodmatch = true;
                        foreach (XmlAttribute ma in m.Attributes)
                        {
                            XmlAttribute oa = on.Attributes[ma.Name];

                            if (oa == null)
                            {
                                continue;
                            }
                            else if (comparer.Compare(oa.Value, ma.Value) == 0)
                            {
                                continue;
                            }
                            else
                            {
                                goodmatch = false;
                                break;
                            }
                        }

                        if (goodmatch)
                        {
                            matching.Add(on);
                        }
                    }

                    // Now check the number of matching nodes
                    if (matching.Count == 1)
                    {
                        // Merge the two nodes
                        MergeNodes(matching.First(), m);
                    }
                    else
                    {
                        // Multiple nodes but no matching
                        // Merge in the whole branch
                        MergeInWholeBranch(orig, m);
                    }
                }
            }
        }

        private void MergeInWholeBranch(XmlNode orig, XmlNode merge)
        {
            XmlNode nn = orig.OwnerDocument.ImportNode(merge, true);

            if (comparer.Compare(merge.Name, "configSections") == 0 && orig.HasChildNodes)
            {
                // config sections must go to the beginning
                orig.InsertBefore(nn, orig.FirstChild);
            }
            else
            {
                orig.AppendChild(nn);
            }
        }

    }
}
