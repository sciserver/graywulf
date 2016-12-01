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

        public XmlReader(XmlDocument xml)
        {
            this.xml = xml;

            // Add the z namespace to support xpath queries
            this.nsmgr = new XmlNamespaceManager(xml.NameTable);
            this.nsmgr.AddNamespace("z", "http://schemas.microsoft.com/2003/10/Serialization/");
        }

        public bool GetXmlBoolean(string path)
        {
            var text = GetXmlInnerText(path);
            return Boolean.Parse(text);
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
                        // See if the current node contains a value or references another one
                        var zref = n.Attributes["z:Ref"];

                        if (zref != null)
                        {
                            // This is a reference, find referenced node
                            var xpath = String.Format("//*[@z:Id=\"{0}\"]", zref.Value);
                            n = xml.SelectNodes(xpath, nsmgr)[0];
                        }
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
    }
}
