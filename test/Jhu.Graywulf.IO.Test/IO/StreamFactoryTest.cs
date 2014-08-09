using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.IO
{
    [TestClass]
    public class StreamFactoryTest
    {
        [TestMethod]
        public void GetCompressionMethodTest()
        {
            var sf = StreamFactory.Create(null);

            Assert.AreEqual(DataFileCompression.None, sf.GetCompressionMethod(Util.UriConverter.FromFilePath("test.dat")));
            Assert.AreEqual(DataFileCompression.None, sf.GetCompressionMethod(Util.UriConverter.FromFilePath("test.tar")));
            Assert.AreEqual(DataFileCompression.GZip, sf.GetCompressionMethod(Util.UriConverter.FromFilePath("test.tar.gz")));
            Assert.AreEqual(DataFileCompression.BZip2, sf.GetCompressionMethod(Util.UriConverter.FromFilePath("test.tar.bz2")));
            Assert.AreEqual(DataFileCompression.Zip, sf.GetCompressionMethod(Util.UriConverter.FromFilePath("test.zip")));
        }

        [TestMethod]
        public void GetArchivalMethodTest()
        {
            var sf = StreamFactory.Create(null);

            Assert.AreEqual(DataFileArchival.None, sf.GetArchivalMethod(Util.UriConverter.FromFilePath("test.dat")));
            Assert.AreEqual(DataFileArchival.Tar, sf.GetArchivalMethod(Util.UriConverter.FromFilePath("test.tar")));
            Assert.AreEqual(DataFileArchival.Tar, sf.GetArchivalMethod(Util.UriConverter.FromFilePath("test.tar.gz")));
            Assert.AreEqual(DataFileArchival.Tar, sf.GetArchivalMethod(Util.UriConverter.FromFilePath("test.tar.bz2")));
            Assert.AreEqual(DataFileArchival.Zip, sf.GetArchivalMethod(Util.UriConverter.FromFilePath("test.zip")));
        }

        [TestMethod]
        public void ReadRelativePathFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/csv_numbers.csv", UriKind.Relative), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteRelativePathFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/writetest.csv", UriKind.Relative), null, DataFileMode.Write))
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadGzFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/csv_numbers.csv.gz", UriKind.Relative), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteGzFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/writetest.csv.gz", UriKind.Relative), null, DataFileMode.Write))
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadBz2FileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/csv_numbers.csv.bz2", UriKind.Relative), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteBz2FileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/writetest.csv.bz2", UriKind.Relative), null, DataFileMode.Write))
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadZipFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/csv_numbers.zip", UriKind.Relative), null, DataFileMode.Read))
            {
                Assert.IsTrue(s is IArchiveInputStream);

                var a = (IArchiveInputStream)s;
                var ae = a.ReadNextFileEntry();

                Assert.IsFalse(ae.IsDirectory);
                Assert.AreEqual("csv_numbers.csv", ae.Filename);

                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteZipFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/writetest.zip", UriKind.Relative), null, DataFileMode.Write))
            {
                Assert.IsTrue(s is IArchiveOutputStream);

                var a = (IArchiveOutputStream)s;

                var ae = a.CreateFileEntry("writetest.csv", 0);
                a.WriteNextEntry(ae);

                Assert.IsFalse(ae.IsDirectory);

                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadTarGzFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/csv_numbers.tar.gz", UriKind.Relative), null, DataFileMode.Read))
            {
                Assert.IsTrue(s is IArchiveInputStream);

                var a = (IArchiveInputStream)s;
                var ae = a.ReadNextFileEntry();

                Assert.IsFalse(ae.IsDirectory);
                Assert.AreEqual("csv_numbers.csv", ae.Filename);

                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteTarGzFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("../../../../graywulf/test/files/writetest.tar.gz", UriKind.Relative), null, DataFileMode.Write))
            {
                Assert.IsTrue(s is IArchiveOutputStream);

                var a = (IArchiveOutputStream)s;

                var ae = a.CreateFileEntry("writetest.csv", 1);
                a.WriteNextEntry(ae);

                Assert.IsFalse(ae.IsDirectory);

                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadFileTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("file:///C:/windows/win.ini"), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void ReadUncTest()
        {
            // TODO: requires a known UNC path, write test
        }

        [TestMethod]
        public void ReadHttpTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("http://www.bing.com"), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void ReadHttpsTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("https://www.google.com"), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void ReadFtpTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.Open(new Uri("ftp://ftp.debian.com/debian/README"), null, DataFileMode.Read))
            {
                s.ReadByte();
            }
        }

        // TODO: add FTP tests with credentials


    }
}
