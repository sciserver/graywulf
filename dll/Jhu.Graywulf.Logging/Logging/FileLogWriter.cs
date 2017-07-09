using System;
using System.IO;
using System.Threading;

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
            stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            Open(stream);
        }

        public override void Stop()
        {
            Close();
        }

        protected override void OnWriteEvent(Event e)
        {
            using (var m = new Mutex(false, "Global\\Jhu.Graywulf.Logging.FileLogWriter"))
            {
                m.WaitOne();

                try
                {
                    base.OnWriteEvent(e);
                }
                finally
                {
                    m.ReleaseMutex();
                }
            }
        }
    }
}
