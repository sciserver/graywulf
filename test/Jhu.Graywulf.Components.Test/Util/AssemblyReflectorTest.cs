using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Util
{
    [TestClass]
    public class AssemblyReflectorTest
    {
        [TestMethod]
        public void IsSystemTest()
        {
            Assert.IsFalse(AssemblyReflector.IsSystem(Assembly.GetAssembly(typeof(AssemblyReflectorTest))));
            Assert.IsTrue(AssemblyReflector.IsSystem(Assembly.GetAssembly(typeof(string))));
        }

        /*
         * logic moved to AssemblyCheck
        [TestMethod]
        public void GetReferencedAssembliesTest()
        {
            var res = AssemblyReflector.GetReferencedAssemblies(Assembly.GetAssembly(typeof(AssemblyReflectorTest)), true);
            Assert.AreEqual(1, res.Count);
        }
        */
    }
}
