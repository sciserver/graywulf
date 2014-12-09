using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Util
{
    [TestClass]
    public class TypeNameFormatterTest
    {
        [TestMethod]
        public void ToUnversionedAssemblyQualifiedNameTest()
        {
            var tn = TypeNameFormatter.ToUnversionedAssemblyQualifiedName(typeof(System.String));
            Assert.AreEqual("System.String, mscorlib", tn);
            
        }
    }
}
