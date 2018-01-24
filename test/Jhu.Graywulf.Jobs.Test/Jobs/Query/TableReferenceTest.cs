using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Jobs.Query
{
    [TestClass]
    public class TableReferenceTest : SqlQueryTestBase
    {
        // TODO: rewrite all these to use a TEST database instead of SkyQuery stuff

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
            InitializeJobTests();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            CleanupJobTests();
            StopLogger();
        }

    }
}
