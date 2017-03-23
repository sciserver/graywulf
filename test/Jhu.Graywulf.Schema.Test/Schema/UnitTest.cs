using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    [TestClass]
    public class UnitTest : Jhu.Graywulf.Test.TestClassBase
    {
        [TestMethod]
        public void ParseUnitEntityTest1()
        {
            var part = new UnitEntity("arcsec");

            Assert.AreEqual(part.UnitBase, "arcsec");
        }

        [TestMethod]
        public void ParseUnitEntityTest2()
        {
            var part = new UnitEntity("km-2");
            var tPart = new UnitEntity() { UnitBase = "m", Exponent = "-2", Prefix = "k"};

            Assert.AreEqual(part.ToString(), tPart.ToString());
        }
        

        [TestMethod]
        public void ParseUnitTest1()
        {
            var unitString = "1E-27 erg s-1 cm-2 AA-1";
            var unit = Unit.Parse(unitString);

            Assert.AreEqual(unitString, unit.ToString());
        }

        [TestMethod]
        public void ParseUnitTest2()
        {
            var unitString = "%";
            var unit = Unit.Parse(unitString);
            

            Assert.AreEqual(unitString, unit.ToString());
        }

        [TestMethod]
        public void ParseUnitGroupTest()
        {
            var groupString = "log(s z+2)-3";
            var group = new UnitGroup(groupString);

            Assert.AreEqual(group.ToString(),groupString);
        }

        [TestMethod]
        public void ParseUnitTest3()
        {

            var unitString = " ";
            var unit = Unit.Parse(unitString);

            Assert.AreEqual("", unit.ToString());
        }

        [TestMethod]
        public void UnitToHtmlTest()
        {
            var unit = Unit.Parse("1E-27 erg log(s-1 cm-2) AA-1");

            Assert.AreEqual(unit.ToHtml(), @"1E-27 erg log(s<sup>-1</sup> cm<sup>-2</sup>) AA<sup>-1</sup>");

        }

        
        [TestMethod]
        public void UnitToLatexTest()
        {
            var unit = Unit.Parse("1E-27 erg log(s-1 cm-2) AA-1");

            Assert.AreEqual(unit.ToLatex(), @"${\rm 1E-27 \times erg~log(s^{-1}~cm^{-2})~AA^{-1}}$");

        }

    }
}
