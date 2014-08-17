using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Web.Controls
{
    public class SiteMap : WebControl
    {
        private static readonly Regex titleRegex = new Regex(@"<%\s*@\s*page\s*[^>]*title\s*=\s*""([^""]*)""", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

        [Themeable(true)]
        public string CssClassSelected
        {
            get { return (string)ViewState["CssClassSelected"] ?? ""; }
            set { ViewState["CssClassSelected"] = value; }
        }

        public bool ExpandRootLevel
        {
            get { return (bool)(ViewState["ExpandRootLevel"] ?? true); }
            set { ViewState["ExpandRootLevel"] = value; }
        }

        public string RootPath
        {
            get { return (string)ViewState["RootPath"] ?? "/~"; }
            set { ViewState["RootPath"] = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var vpp = HostingEnvironment.VirtualPathProvider;
            var vd = vpp.GetDirectory(RootPath);

            RenderDirectory(writer, vpp, vd);
        }

        private void RenderDirectory(HtmlTextWriter writer, VirtualPathProvider vpp, VirtualDirectory dir)
        {
            RenderCssAttribute(writer, false);
            writer.RenderBeginTag("ul");

            var dirpath = VirtualPathUtility.ToAppRelative(dir.VirtualPath);
            var pagedir = VirtualPathUtility.GetDirectory(Page.AppRelativeVirtualPath);
            var rootpath = VirtualPathUtility.AppendTrailingSlash(RootPath);

            // Render root directory
            foreach (var vf in dir.Children.Cast<VirtualFileBase>().OrderBy(f => f.Name))
            {
                var filepath = VirtualPathUtility.ToAppRelative(vf.VirtualPath);
                var filedir = VirtualPathUtility.GetDirectory(filepath);
                var filename = VirtualPathUtility.GetFileName(filepath);
                
                var isinpath = pagedir.StartsWith(filepath);

                if (vf.IsDirectory)
                {
                    filepath = VirtualPathUtility.Combine(filepath, "00_index.aspx");

                    if (!vpp.FileExists(filepath))
                    {
                        // Skip directory, if there's no index file
                        continue;
                    }
                }
                else if (comparer.Compare(dirpath, rootpath) != 0 &&
                         comparer.Compare(filename, "00_index.aspx") == 0 ||
                         comparer.Compare(filename, "Default.aspx") == 0 ||
                         comparer.Compare(VirtualPathUtility.GetExtension(filepath), ".aspx") != 0)
                {
                    // Skip index file and default.aspx in root or non aspx files
                    continue;
                }

                var isselected = comparer.Compare(filepath, Page.AppRelativeVirtualPath) == 0;
                var isinroot = comparer.Compare(filedir, rootpath) == 0;
                var title = GetPageTitle(vpp.GetFile(filepath));

                // Apply css to <li>
                RenderCssAttribute(writer, isselected);
                writer.RenderBeginTag("li");

                // Apply css to <a>
                RenderCssAttribute(writer, isselected);
                writer.AddAttribute("href", VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, filepath));
                writer.RenderBeginTag("a");
                writer.Write(title);
                writer.RenderEndTag();

                writer.RenderEndTag();

                // Render children, if
                //      - it is the current page
                //      - the current page is under this directory
                if (vf.IsDirectory && (isselected || isinpath || (isinroot && ExpandRootLevel)))
                {
                    RenderDirectory(writer, vpp, (VirtualDirectory)vf);
                }
            }

            writer.RenderEndTag();
        }

        private void RenderCssAttribute(HtmlTextWriter writer, bool isSelected)
        {
            if (isSelected && !String.IsNullOrEmpty(CssClassSelected))
            {
                writer.AddAttribute("class", CssClassSelected);
            }
            else if (!String.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute("class", CssClass);
            }
        }

        private string GetPageTitle(VirtualFile file)
        {
            string aspx;
            using (var infile = file.Open())
            {
                using (var reader = new StreamReader(infile))
                {
                    aspx = reader.ReadToEnd();
                }
            }

            var m = titleRegex.Match(aspx);
            if (m.Success && m.Groups.Count > 1)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return VirtualPathUtility.GetFileName(file.VirtualPath);
            }
        }
    }
}