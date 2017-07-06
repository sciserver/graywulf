using System;
using System.IO;

namespace Jhu.Graywulf.Logging
{
    public class FileLogWriter : StreamLogWriter
    {
        private string path;
        private FileStream stream;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public FileLogWriter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.stream = null;
        }

        public override void Start()
        {
            stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            Open(stream);
        }

        public override void Stop()
        {
            Close();
        }
    }
}
