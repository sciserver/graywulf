using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;
using System.Xml;

namespace Jhu.Graywulf.Web.Controls
{
    /// <summary>
    /// Summary description for Includer.
    /// </summary>
    public class Includer : System.Web.UI.Control
    {
        private static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

        private string content;
        private string title;

        public string Src
        {
            get { return (string)ViewState["Src"] ?? String.Empty; }
            set
            {
                ViewState["Src"] = value;
                content = null;
                title = String.Empty;
            }
        }

        public string QueryStringArgument
        {
            get { return (string)ViewState["QueryStringArgument"] ?? "page"; }
            set { ViewState["QueryStringArgument"] = value; }
        }

        public string ContentPath
        {
            get { return (string)ViewState["ContentPath"] ?? String.Empty; }
            set { ViewState["ContentPath"] = value; }
        }

        public string Title
        {
            get
            {
                EnsureFileLoaded();
                return this.title;
            }
        }

        public Includer()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.content = null;
            this.title = String.Empty;
        }

        private void LoadFile()
        {
            if (String.IsNullOrEmpty(Src))
            {
                throw new InvalidOperationException();
            }

            var path = VirtualPathUtility.Combine(Page.AppRelativeVirtualPath, Src);

            if (!VirtualPathUtility.IsAppRelative(path))
            {
                throw new InvalidOperationException();
            }

            this.content = File.ReadAllText(Page.Server.MapPath(path));

            ReadTitle();
        }

        private void EnsureFileLoaded()
        {
            if (content == null)
            {
                LoadFile();
            }
        }

        private XmlReader OpenXml(TextReader reader)
        {
            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            return XmlTextReader.Create(reader, settings);
        }

        private void ReadTitle()
        {
            this.title = String.Empty;

            using (var sr = new StringReader(content))
            {
                using (var xml = OpenXml(sr))
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element && comparer.Compare(xml.Name, "title") == 0)
                        {
                            this.title = xml.ReadInnerXml();
                            break;
                        }
                    }
                }
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            EnsureFileLoaded();

            
            var path = VirtualPathUtility.Combine(Page.AppRelativeVirtualPath, Src);
            var cpath = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.Combine(Page.AppRelativeVirtualPath, ContentPath));
            var page = VirtualPathUtility.GetFileName(Page.AppRelativeVirtualPath);

            var insidebody = false;

            using (var sr = new StringReader(content))
            {
                using (var xml = OpenXml(sr))
                {
                    while (xml.Read())
                    {
                        switch (xml.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (comparer.Compare(xml.Name, "body") == 0)
                                {
                                    insidebody = true;
                                }
                                else if (insidebody)
                                {
                                    var tagname = xml.Name;

                                    // set attributes
                                    while (xml.MoveToNextAttribute())
                                    {
                                        string value;

                                        if (comparer.Compare(xml.Name, "src") == 0 ||
                                            comparer.Compare(xml.Name, "background") == 0)
                                        {
                                            var src = VirtualPathUtility.Combine(path, xml.Value);
                                            value = VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, src);
                                        }
                                        else if (comparer.Compare(xml.Name, "href") == 0)
                                        {
                                            var href = VirtualPathUtility.Combine(path, xml.Value);
                                            href = VirtualPathUtility.MakeRelative(cpath, href);
                                            value = String.Format("{0}?{1}={2}", page, QueryStringArgument, href);
                                        }
                                        else
                                        {
                                            value = xml.Value;
                                        }

                                        writer.AddAttribute(xml.Name, value);
                                    }

                                    // render tag
                                    writer.RenderBeginTag(tagname);
                                }

                                break;
                            case XmlNodeType.EndElement:
                                if (comparer.Compare(xml.Name, "body") == 0)
                                {
                                    insidebody = false;
                                }
                                else if (insidebody)
                                {
                                    writer.RenderEndTag();
                                }

                                break;
                            case XmlNodeType.Text:
                                if (insidebody)
                                {
                                    writer.Write(xml.Value);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}