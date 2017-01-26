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
        public void ParseUnitPartTest1()
        {
            var part = new UnitPart("arcsec");

            Assert.AreEqual(part.UnitBase, "arcsec");
        }

        [TestMethod]
        public void ParseUnitPartTest2()
        {
            var part = new UnitPart("km-2");
            var tPart = new UnitPart() { UnitBase = "m", Exponent = "-2", Prefix = "k"};

            Assert.AreEqual(part.ToString(), tPart.ToString());
        }


        [TestMethod]
        public void ParseUnitPartTest3()
        {
            var part = new UnitPart("log(umag)-1");
            var tPart = new UnitPart() { UnitBase = "mag", Exponent = "-1", Prefix = "u", Function="log" };

            Assert.AreEqual(part.ToString(), tPart.ToString());
        }

        [TestMethod]
        public void ParseUnitTest()
        {
            var unitString = "1E-27 erg s-1 cm-2 AA-1";
            var unit = new Unit(unitString);

            Assert.AreEqual(unitString, unit.ToString());
        }
    }
}
