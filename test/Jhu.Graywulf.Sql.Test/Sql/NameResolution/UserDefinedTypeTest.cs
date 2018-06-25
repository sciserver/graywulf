using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class UserDefinedTypeTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void DeclareSingleVariableTest()
        {
            var query = Parse("DECLARE @var dbo.ClrUDT");
            var d = query.ParsingTree.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, query.VariableReferences.Count);
            Assert.IsTrue(query.VariableReferences.ContainsKey("@var"));
        }
    }        
}
