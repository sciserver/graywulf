using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Jhu.Graywulf.Web.Api.Util
{
    /// <summary>
    /// Implements functions to read values from XML documents
    /// serialized by NetDataContractSerializer, without deserializing the class
    /// </summary>
    public class XmlReader
    {
        private XmlDocument xml;
        private XmlNamespaceManager nsmgr;

        public XmlReader(string xml)
        {
            this.xml = new XmlDocument();
            this.xml.LoadXml(xml);

            InitializeMembers();
        }

        public XmlReader(XmlDocument xml)
        {
            this.xml = xml;

            InitializeMembers();
        }

        private void InitializeMembers()
        {
            // Add the z namespace to support xpath queries
            this.nsmgr = new XmlNamespaceManager(xml.NameTable);
            this.nsmgr.AddNamespace("z", "http://schemas.microsoft.com/2003/10/Serialization/");
        }

        public bool TryGetXmlBoolean(string path, out bool value)
        {
            var text = GetXmlInnerText(path);

            if (text == null)
            {
                value = false;
                return false;
            }
            else
            {
                value = Boolean.Parse(text);
                return true;
            }
        }

        public string GetXmlInnerText(string path)
        {
            return GetXmlInnerText(path, true);
        }

        public string GetXmlInnerText(string path, bool resolveReferences)
        {
            var node = GetNode(path, resolveReferences);
            return node == null ? null : node.InnerText;
        }

        public string GetAttribute(string path, string attribute)
        {
            return GetAttribute(path, attribute, true);
        }

        public string GetAttribute(string path, string attribute, bool resolveReferences)
        {
            var node = GetNode(path, resolveReferences);
            return node == null ? null : node.Attributes[attribute].Value;
        }

        public Dictionary<string, string> GetAsDictionary(string path)
        {
            return GetAsDictionary(path, true);
        }

        public Dictionary<string, string> GetAsDictionary(string path, bool resolveReferences)
        {
            var node = GetNode(path, resolveReferences);
            return GetAsDictionary(node, resolveReferences);
        }

        public IEnumerable<Dictionary<string, string>> EnumerateAsDictionary(string path)
        {
            return EnumerateAsDictionary(path, true);
        }

        public IEnumerable<Dictionary<string, string>> EnumerateAsDictionary(string path, bool resolveReferences)
        {
            foreach (var node in EnumerateNodes(path, resolveReferences))
            {
                yield return GetAsDictionary(node, resolveReferences);
            }
        }

        private XmlNode GetNode(string path, bool resolveReferences)
        {
            return GetNode(xml.ChildNodes, path.Split('/'), resolveReferences, 0);
        }

        private XmlNode GetNode(XmlNodeList nodes, string[] path, bool resolveReferences, int i)
        {
            for (int k = 0; k < nodes.Count; k++)
            {
                var n = nodes[k];
                if (StringComparer.InvariantCultureIgnoreCase.Compare(n.LocalName, path[i]) == 0)
                {
                    if (resolveReferences)
                    {
                        n = ResolveNode(n);
                    }

                    if (i == path.Length - 1)
                    {
                        if (n == null || n.InnerText == null)
                        {
                            return null;
                        }
                        else
                        {
                            return n;
                        }
                    }
                    else
                    {
                        return GetNode(n.ChildNodes, path, resolveReferences, i + 1);
                    }
                }
            }

            return null;
        }

        private XmlNode ResolveNode(XmlNode n)
        {
            // See if the current node contains a value or references another one
            var zref = n.Attributes["z:Ref"];

            if (zref != null)
            {
                // This is a reference, find referenced node
                var xpath = String.Format("//*[@z:Id=\"{0}\"]", zref.Value);
                n = xml.SelectNodes(xpath, nsmgr)[0];
            }

            return n;
        }

        private IEnumerable<XmlNode> EnumerateNodes(string path, bool resolveReference)
        {
            string lastpart;
            XmlNode node;
            var idx = path.LastIndexOf('/');

            if (idx >= 0)
            {
                lastpart = path.Substring(idx + 1);
                node = GetNode(path.Substring(0, idx), resolveReference);
            }
            else
            {
                lastpart = path;
                node = xml.DocumentElement;
            }

            var nodes = node.ChildNodes;

            for (int k = 0; k < nodes.Count; k++)
            {
                var n = nodes[k];
                if (StringComparer.InvariantCultureIgnoreCase.Compare(n.LocalName, lastpart) == 0)
                {
                    yield return n;
                }
            }
        }

        private Dictionary<string, string> GetAsDictionary(XmlNode xmlNode, bool resolveReference)
        {
            var res = new Dictionary<string, string>();

            foreach (XmlNode n in xmlNode.ChildNodes)
            {
                var nn = n;

                if (resolveReference)
                {
                    nn = ResolveNode(nn);
                }

                res.Add(n.Name, nn.InnerText);
            }

            return res;
        }
    }
}
