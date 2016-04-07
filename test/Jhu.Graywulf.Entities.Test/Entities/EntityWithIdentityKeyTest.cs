using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class EntityWithIdentityKeyTest : TestClassBase
    {
        [TestMethod]
        public void CreateTest()
        {
            using (var context = CreateContext())
            {
                var xml = new XmlDocument();
                var xn = xml.CreateElement("test");
                xml.AppendChild(xn);

                var e = new EntityWithIdentityKey(context)
                {
                    ID = -1,
                    Name = "test",
                    Rename = "test",
                    Four = "four",
                    AnsiText = "testtest",
                    VarCharText = "moretest",
                    SByte = 1,
                    Int16 = 2,
                    Int32 = 3,
                    Int64 = 4,
                    Byte = 5,
                    UInt16 = 6,
                    UInt32 = 7,
                    UInt64 = 8,
                    Single = 9,
                    Double = 10,
                    Decimal = 11,
                    String = "twelve",
                    ByteArray = new byte[13],
                    DateTime = DateTime.Now,
                    Guid = new Guid(),
                    XmlElement = xml.DocumentElement,
                };

                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.IsTrue(e.ID > 0);
            }
        }
    }
}
