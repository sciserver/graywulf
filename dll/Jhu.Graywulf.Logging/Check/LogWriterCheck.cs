using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Check
{
    public class LogWriterCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Plugin; }
        }

        public Logging.LogWriterBase LogWriter { get; set; }

        public LogWriterCheck(Logging.LogWriterBase writer)
        {
            LogWriter = writer;
        }

        public override void Execute(TextWriter output)
        {
            var message = String.Format("Testing log writer {0}.", LogWriter.GetType().Name);
            output.WriteLine(message);

            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                Logging.EventSource.WebUI,
                message,
                GetType().FullName,
                null,
                null);

            LogWriter.WriteEvent(e);

            output.WriteLine("Log event written successfully.");
        }
    }
}
