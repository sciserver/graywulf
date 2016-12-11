using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace Jhu.Graywulf.Web.Controls
{
    public abstract class DocsTreeVisitor
    {
        private static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
        private static readonly StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
        private static readonly Regex titleRegex = new Regex(@"<%\s*@\s*page\s*[^>]*title\s*=\s*""([^""]*)""", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private string rootPath;
        private string currentPath;
        private string contentFileExtension;
        private string indexFileName;
        private bool skipRoot;
        private int expandLevel;

        public string RootPath
        {
            get { return rootPath; }
            set { rootPath = value; }
        }

        public string CurrentPath
        {
            get { return currentPath; }
            set { currentPath = value; }
        }

        public string ContentFileExtension
        {
            get { return contentFileExtension; }
            set { contentFileExtension = value; }
        }

        public string IndexFileName
        {
            get { return indexFileName; }
            set { indexFileName = value; }
        }

        public bool SkipRoot
        {
            get { return skipRoot; }
            set { skipRoot = value; }
        }

        public int ExpandLevel
        {
            get { return expandLevel; }
            set { expandLevel = value; }
        }


        public DocsTreeVisitor()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.rootPath = null;
            this.currentPath = null;
            this.contentFileExtension = ".aspx";
            this.indexFileName = "00_index";
            this.skipRoot = true;
            this.expandLevel = 1;
        }

        public void Execute()
        {
            OnExecuting();

            // Start from root path and explore the tree level by level
            // towards the current path

            var vpp = HostingEnvironment.VirtualPathProvider;
            var vroot = vpp.GetDirectory(rootPath);

            VisitDirectory(vpp, vroot, 0);

            OnComplete();
        }

        private void VisitDirectory(VirtualPathProvider vpp, VirtualDirectory vdir, int level)
        {
            var args = CreateEventArgs(vdir, level);
            OnVisitingDir(vpp, args);

            if (args.Expand)
            {
                // Descend to immediate children
                foreach (var vpath in vdir.Children.Cast<VirtualFileBase>().OrderBy(f => f.Name))
                {
                    if (vpath.IsDirectory)
                    {
                        VisitDirectory(vpp, (VirtualDirectory)vpath, level + 1);
                    }
                    else
                    {
                        var fargs = CreateEventArgs(vpath, level);
                        OnVisitingFile(vpp, fargs);
                    }
                }
            }

            OnVisitedDir(vpp, args);
        }

        protected virtual void OnExecuting()
        {
        }

        protected virtual void OnComplete()
        {
        }

        protected virtual void OnVisitingDir(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
        {
            var indexfile = VirtualPathUtility.Combine(args.Path, indexFileName + contentFileExtension);

            // Skip directory, if there's no index file
            if (!virtualPathProvider.FileExists(indexfile))
            {
                args.Skip = true;
            }
            else
            {
                args.Skip = skipRoot && args.Level == 0;
                args.IsSelected = currentPath.StartsWith(args.Path, comparison);
                args.Expand = args.IsSelected || (args.Level <= expandLevel);
                args.Path = VirtualPathUtility.Combine(args.Path, IndexFileName + contentFileExtension);
                args.Title = GetPageTitle(virtualPathProvider.GetFile(indexfile));
            }
        }

        protected virtual void OnVisitedDir(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
        {
        }

        protected virtual void OnVisitingFile(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
        {
            var filename = VirtualPathUtility.GetFileName(args.Path);

            // Skip index file and unknown extensions
            if (comparer.Compare(VirtualPathUtility.GetExtension(filename), contentFileExtension) != 0 ||
                comparer.Compare(filename, indexFileName + contentFileExtension) == 0)
            {
                args.Skip = true;
            }
            else
            {
                args.Skip = false;
                args.IsSelected = currentPath.StartsWith(args.Path, comparison);
                args.Title = GetPageTitle(virtualPathProvider.GetFile(args.Path));
            }
        }

        private DocsTreeVisitorEventArgs CreateEventArgs(VirtualFileBase vpath, int level)
        {
            bool current;
            var path = VirtualPathUtility.ToAppRelative(vpath.VirtualPath);

            if (vpath is VirtualDirectory)
            {
                current = comparer.Compare(VirtualPathUtility.Combine(path, indexFileName + contentFileExtension), CurrentPath) == 0;
            }
            else
            {
                current = comparer.Compare(path, CurrentPath) == 0;
            }

            var args = new DocsTreeVisitorEventArgs(path, level, current, vpath.IsDirectory);
            return args;
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

        protected string GetRelativePath(DocsTreeVisitorEventArgs args)
        {
            return VirtualPathUtility.MakeRelative(CurrentPath, args.Path);
        }
    }
}
