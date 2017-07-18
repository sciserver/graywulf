using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class FileLogWriter : StreamLogWriter
    {
        public static FileLogWriterConfiguration Configuration
        {
            get
            {
                return (FileLogWriterConfiguration)ConfigurationManager.GetSection("jhu.graywulf/logging/file");
            }
        }

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

        private void OpenFile()
        {
            stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        private void CloseFile()
        {
            Close();
        }

        protected override void OnStart()
        {
        }

        protected override void OnBatchStart()
        {
            if (!IsOpen)
            {
                OpenFile();
                Open(stream);
            }
        }

        protected override void OnBatchEnd()
        {
            // do nothing here, don't close file too frequently
        }

        protected override void OnStop()
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

        protected override void OnUnhandledExpcetion(Exception ex)
        {
            Close();
        }

        public override IEnumerable<Check.CheckRoutineBase> GetCheckRoutines()
        {
            yield return new Check.FileAccessCheck(Configuration.Path);
        }
    }
}
