using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Entities
{

    [TestClass]
    public class IdentityKeyEntitySearchTest : TestClassBase
    {
        protected static IdentityKeyEntity CreateEntity(Context context)
        {
            var xml = new XmlDocument();
            var xn = xml.CreateElement("test");
            xml.AppendChild(xn);

            var e = new IdentityKeyEntity(context)
            {
                ID = 0,
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

            return e;
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();
            }
        }

        [TestMethod]
        public void CountTest()
        {
            using (var context = CreateContext())
            {
                var s = new IdentityKeyEntitySearch(context);

                s.Name = "tes%";

                var cnt = s.Count();

                Assert.AreEqual(1, cnt);
            }
        }

        [TestMethod]
        public void FindTest()
        {
        }

        [TestMethod]
        public void RangeQueryTest()
        {
            using (var context = CreateContext())
            {
                var s = new IdentityKeyEntitySearch(context)
                {
                    Int16 = new Range<short>(-10, 20)
                };

                var cnt = s.Count();

                Assert.AreEqual(1, cnt);
            }
        }
    }
}
