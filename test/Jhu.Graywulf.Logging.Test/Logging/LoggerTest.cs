using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Logging
{
    [TestClass]
    public class LoggerTest : Jhu.Graywulf.Test.LoggingTestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            StopLogger();
        }

        [TestMethod]
        public void LogEventTest()
        {
            LoggingContext.Current.LogStatus(EventSource.Test, "Test message");
        }
    }
}
