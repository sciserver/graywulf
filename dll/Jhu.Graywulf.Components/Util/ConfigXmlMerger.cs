using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Jhu.Graywulf.Util
{
    /// <summary>
    /// Implements function to merge two config files.
    /// </summary>
    public static class ConfigXmlMerger
    {
        private static StringComparer comparer = StringComparer.InvariantCulture;

        private static HashSet<string> nonOverWrite;

        static ConfigXmlMerger()
        {
            nonOverWrite = new HashSet<string>(comparer)
            {
                "sectionGroup",
                "section",
                "add",
                "remove",
                "delete"
            };
        }

        /// <summary>
        /// Merges two XML documents containing confif files.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="merge"></param>
        public static void Merge(XmlDocument orig, XmlDocument merge)
        {
            MergeNodes(orig.DocumentElement, merge.DocumentElement);
        }

        /// <summary>
        /// Merges two nodes at the same location in and XML tree
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="merge"></param>
        private static void MergeNodes(XmlElement orig, XmlElement merge)
        {
            // This can happen in case of the root nodes only
            if (comparer.Compare(orig.Name, merge.Name) != 0)
            {
                throw new Exception("Incompatible nodes.");
            }

            MergeAttributes(orig, merge);
            MergeChildElements(orig, merge);
        }

        /// <summary>
        /// Merges the attributes of two, supposedly similar xml nodes.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="merge"></param>
        private static void MergeAttributes(XmlElement orig, XmlElement merge)
        {
            foreach (XmlAttribute mergeAttribute in merge.Attributes)
            {
                // Check if original element contains the same attribute
                XmlAttribute origAttribute = orig.Attributes[mergeAttribute.Name];

                // Include or overwrite
                if (origAttribute == null)
                {
                    // Merge
                    orig.SetAttribute(mergeAttribute.Name, mergeAttribute.Value);
                }
                else
                {
                    // Overwrite value
                    origAttribute.Value = mergeAttribute.Value;
                }
            }
        }

        /// <summary>
        /// Merges two xml branches
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="merge"></param>
        private static void MergeChildElements(XmlElement orig, XmlElement merge)
        {
            foreach (XmlNode mergeNode in merge.ChildNodes)
            {
                if (!(mergeNode is XmlElement))
                {
                    continue;
                }

                var mergeElement = (XmlElement)mergeNode;

                var all = new HashSet<XmlElement>();

                // Try merging in nodes
                try
                {
                    // Find possible matching nodes in original
                    var xpath = String.Format("./{0}", mergeElement.Name);
                    var nodes = orig.SelectNodes(xpath).Cast<XmlElement>();
                    all.UnionWith(nodes);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Cannot merge in node: {0}.", mergeElement.Name), e);
                }

                // There are two ways of merging xml elements and branches. If the original
                // document doesn't contain a tag, merging the new branch is trivial, we
                // just add everything. If the original document contains the element, there
                // are again two ways to go. It the element is listed in the merged list,
                // then mergin happens whenever the full attribute set is the same. If
                // attributes differ, then a new branch is created. If an element is not listed
                // in the merged set, but a match is found, attributes are overwritten by
                // the merge operation.

                if (all.Count == 0)
                {
                    // No matching node in original xml
                    // Simple case, merge in the whole branch
                    MergeInWholeBranch(orig, mergeElement);
                }
                else if (!nonOverWrite.Contains(mergeElement.Name))
                {
                    // This is a match that requires overwrite. Make sure match
                    // is unique, as usually required by config files
                    if (all.Count > 1)
                    {
                        throw new Exception(String.Format("Multiple matching element found: {0}.", orig.Name));
                    }

                    // The attributes of this element need to be overwritten
                    MergeNodes(all.First(), mergeElement);
                }
                else
                {
                    // Check if all specified attributes match
                    var matching = new HashSet<XmlElement>();

                    foreach (var on in all)
                    {
                        bool goodmatch = true;
                        foreach (XmlAttribute matchinAttribute in mergeElement.Attributes)
                        {
                            XmlAttribute oa = on.Attributes[matchinAttribute.Name];

                            if (oa == null)
                            {
                                continue;
                            }
                            else if (comparer.Compare(oa.Value, matchinAttribute.Value) == 0)
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
                        MergeNodes(matching.First(), mergeElement);
                    }
                    else
                    {
                        // Multiple nodes but none of them matching
                        // Merge in the whole branch
                        MergeInWholeBranch(orig, mergeElement);
                    }
                }
            }
        }

        private static void MergeInWholeBranch(XmlElement orig, XmlElement merge)
        {
            XmlNode nn = orig.OwnerDocument.ImportNode(merge, true);

            // TODO: make this a bit more generic, but it's otherwise OK
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
