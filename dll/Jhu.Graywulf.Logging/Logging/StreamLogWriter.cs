using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Jhu.Graywulf.Logging
{
    public class StreamLogWriter : LogWriter
    {
        Stream stream;
        TextWriter textWriter;

        public StreamLogWriter()
            : base()
        {
        }

        public StreamLogWriter(Stream stream)
        {
            this.stream = stream;
            this.textWriter = new StreamWriter(stream);
        }

        public StreamLogWriter(TextWriter textWriter)
        {
            this.stream = null;
            this.textWriter = textWriter;
        }

        public override void WriteEvent(Event e)
        {
            textWriter.WriteLine("{0} : {1} : {2}", e.EventDateTime, e.Operation, e.ExecutionStatus);
            if (e.Exception != null)
            {
                textWriter.WriteLine("   {0}", e.Exception.Message);
            }
        }
    }
}
