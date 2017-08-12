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

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            var message = String.Format("Testing log writer {0}.", LogWriter.GetType().Name);
            yield return ReportInfo(message);

            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                Logging.EventSource.WebUI,
                message,
                GetType().FullName,
                null,
                null);

            LogWriter.WriteEvent(e);

            yield return ReportSuccess("Log event written successfully.");
        }
    }
}
