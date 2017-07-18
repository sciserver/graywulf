using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Jhu.Graywulf.Logging
{
    public abstract class StreamLogWriter : LogWriterBase
    {
        private Stream stream;
        private bool ownsStream;
        private TextWriter textWriter;
        private bool ownsTextWriter;

        protected bool IsOpen
        {
            get { return stream != null; }
        }

        public StreamLogWriter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.stream = null;
            this.ownsStream = false;
            this.textWriter = null;
            this.ownsTextWriter = false;
        }

        public override void Dispose()
        {
            Close();
        }

        public void Open(Stream stream)
        {
            this.stream = stream;
            this.ownsStream = false;
            this.textWriter = new StreamWriter(stream);
            this.ownsTextWriter = true;
        }

        public void Open(TextWriter textWriter)
        {
            this.stream = null;
            this.ownsStream = false;
            this.textWriter = textWriter;
            this.ownsTextWriter = false;
        }

        public void Close()
        {
            if (textWriter != null && ownsTextWriter)
            {
                textWriter.Close();
                textWriter.Dispose();
                textWriter = null;
            }

            if (stream != null && ownsStream)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        protected override void OnWriteEvent(Event e)
        {
            textWriter.WriteLine("{0} {1:s} {2} {3}", e.Order, e.DateTime, e.Operation, e.ExecutionStatus);

            if (e.ExceptionType != null)
            {
                textWriter.WriteLine("   {0}", e.ExceptionType);
                textWriter.WriteLine("   {0}", e.Message);
            }
        }
    }
}
