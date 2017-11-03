using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.IO
{
    [TestClass]
    public class StreamFactoryTest : Jhu.Graywulf.Test.TestClassBase
    {
        [TestMethod]
        public void GetFileExtensionsTest()
        {
            var sf = StreamFactory.Create(null);

            string path, filename, extension;
            DataFileArchival am;
            DataFileCompression cm;

            StreamFactory.GetFileExtensions(new Uri("file:///"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/", path);
            Assert.AreEqual("", filename);
            Assert.AreEqual("", extension);
            Assert.AreEqual(DataFileArchival.None, am);
            Assert.AreEqual(DataFileCompression.None, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("", filename);
            Assert.AreEqual("", extension);
            Assert.AreEqual(DataFileArchival.None, am);
            Assert.AreEqual(DataFileCompression.None, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/file"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("file", filename);
            Assert.AreEqual("", extension);
            Assert.AreEqual(DataFileArchival.None, am);
            Assert.AreEqual(DataFileCompression.None, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/file.txt"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("file", filename);
            Assert.AreEqual(".txt", extension);
            Assert.AreEqual(DataFileArchival.None, am);
            Assert.AreEqual(DataFileCompression.None, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/file.txt.gz"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("file", filename);
            Assert.AreEqual(".txt", extension);
            Assert.AreEqual(DataFileArchival.None, am);
            Assert.AreEqual(DataFileCompression.GZip, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/file.txt.tar.gz"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("file", filename);
            Assert.AreEqual(".txt", extension);
            Assert.AreEqual(DataFileArchival.Tar, am);
            Assert.AreEqual(DataFileCompression.GZip, cm);

            StreamFactory.GetFileExtensions(new Uri("file:///dir/file.txt.zip"), out path, out filename, out extension, out am, out cm);
            Assert.AreEqual("/dir", path);
            Assert.AreEqual("file", filename);
            Assert.AreEqual(".txt", extension);
            Assert.AreEqual(DataFileArchival.Zip, am);
            Assert.AreEqual(DataFileCompression.Zip, cm);
        }

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
        public void CombineFileExtensionsTest()
        {
            Assert.AreEqual("/", StreamFactory.CombineFileExtensions("/", "", "", DataFileArchival.None, DataFileCompression.None));
            Assert.AreEqual("/file", StreamFactory.CombineFileExtensions("/", "file", "", DataFileArchival.None, DataFileCompression.None));
            Assert.AreEqual("/file.txt", StreamFactory.CombineFileExtensions("/", "file", ".txt", DataFileArchival.None, DataFileCompression.None));
            Assert.AreEqual("/file.gz", StreamFactory.CombineFileExtensions("/", "file", "", DataFileArchival.None, DataFileCompression.GZip));
            Assert.AreEqual("/file.txt.gz", StreamFactory.CombineFileExtensions("/", "file", ".txt", DataFileArchival.None, DataFileCompression.GZip));
            Assert.AreEqual("/file.test.txt.gz", StreamFactory.CombineFileExtensions("/", "file.test", ".txt", DataFileArchival.None, DataFileCompression.GZip));
            Assert.AreEqual("/file.txt.tar.gz", StreamFactory.CombineFileExtensions("/", "file", ".txt", DataFileArchival.Tar, DataFileCompression.GZip));
            Assert.AreEqual("/file.txt.zip", StreamFactory.CombineFileExtensions("/", "file", ".txt", DataFileArchival.Zip, DataFileCompression.Zip));
        }

        [TestMethod]
        public void ReadRelativePathFileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/csv_numbers.csv");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteRelativePathFileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/writetest.csv");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Write).Result)
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadGzFileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/csv_numbers.csv.gz");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteGzFileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/writetest.csv.gz");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Write).Result)
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadBz2FileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/csv_numbers.csv.bz2");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void WriteBz2FileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/writetest.csv.bz2");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Write).Result)
            {
                s.WriteByte(0);
            }
        }

        [TestMethod]
        public void ReadZipFileTest()
        {
            var sf = StreamFactory.Create(null);
            var path = GetTestFilePath("modules/graywulf/test/files/csv_numbers.zip");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Read).Result)
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
            var path = GetTestFilePath("modules/graywulf/test/files/writetest.zip");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Write).Result)
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
            var path = GetTestFilePath("modules/graywulf/test/files/csv_numbers.tar.gz");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Read).Result)
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
            var path = GetTestFilePath("modules/graywulf/test/files/writetest.tar.gz");

            using (var s = sf.OpenAsync(new Uri(path, UriKind.Relative), null, DataFileMode.Write).Result)
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

            using (var s = sf.OpenAsync(new Uri("file:///C:/windows/win.ini"), null, DataFileMode.Read).Result)
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

            using (var s = sf.OpenAsync(new Uri("http://www.bing.com"), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void ReadHttpsTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.OpenAsync(new Uri("https://www.google.com"), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        [TestMethod]
        public void ReadFtpTest()
        {
            var sf = StreamFactory.Create(null);

            using (var s = sf.OpenAsync(new Uri("ftp://ftp.debian.com/debian/README"), null, DataFileMode.Read).Result)
            {
                s.ReadByte();
            }
        }

        // TODO: add FTP tests with credentials


    }
}
