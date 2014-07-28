using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class FileFormatFactoryTest
    {
        [TestMethod]
        public void EnumerateFormatsTest()
        {
            var df = FileFormatFactory.Create(null).EnumerateFileFormatDescriptions();
        }
    }
}
