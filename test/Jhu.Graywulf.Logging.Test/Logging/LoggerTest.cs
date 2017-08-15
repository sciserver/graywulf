using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Logging
{
    [TestClass]
    public class LoggerTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            LoggingContext.Current.StartLogger(EventSource.Test, true);
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            LoggingContext.Current.StopLogger();
        }

        [TestMethod]
        public void LogEventTest()
        {
            LoggingContext.Current.LogStatus(EventSource.Test, "Test message");
        }
    }
}
