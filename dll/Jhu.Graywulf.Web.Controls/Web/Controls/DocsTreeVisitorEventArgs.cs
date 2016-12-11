using System.Web;

namespace Jhu.Graywulf.Web.Controls
{
    public class DocsTreeVisitorEventArgs
    {
        private string path;
        private int level;
        private string title;
        private bool skip;
        private bool expand;
        private bool isSelected;
        private bool isCurrent;
        private bool isDirectory;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public int Level
        {
            get { return level; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public bool Skip
        {
            get { return skip; }
            set { skip = value; }
        }

        public bool Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public bool IsCurrent
        {
            get { return isCurrent; }
        }

        public bool IsDirectory
        {
            get { return isDirectory; }
        }

        internal DocsTreeVisitorEventArgs(string path, int level, bool isCurrent, bool isDirectory)
        {
            InitializeMembers();

            this.path = path;
            this.level = level;
            this.isCurrent = isCurrent;
            this.isDirectory = isDirectory;
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.level = 0;
            this.title = null;
            this.skip = true;
            this.expand = false;
            this.isSelected = false;
        }
    }
}
