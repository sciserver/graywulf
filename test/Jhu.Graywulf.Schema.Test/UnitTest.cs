using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void ParseUnitPartStringTest()
        {
            var part = new UnitPart("arcsec");

        }

        [TestMethod]
        public void ParseUnitStringTest()
        {
            var unitString = "1e-27 erg s-1 cm-2 AA-1";
            var unit = new Unit(unitString);

        }
    }
}
