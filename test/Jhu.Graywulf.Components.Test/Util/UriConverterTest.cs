using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Util
{
    [TestClass]
    public class UriConverterTest
    {
        [TestMethod]
        public void FromFilePathTest()
        {
            Assert.AreEqual("alma.txt", UriConverter.FromFilePath(@"alma.txt").ToString());
            Assert.AreEqual("relative/path/alma.txt", UriConverter.FromFilePath(@"relative\path\alma.txt").ToString());
            Assert.AreEqual("file:///absolute/path/alma.txt", UriConverter.FromFilePath(@"\absolute\path\alma.txt").ToString());
            Assert.AreEqual("file:///c:/absolute/path/alma.txt", UriConverter.FromFilePath(@"c:\absolute\path\alma.txt").ToString());
            Assert.AreEqual("file://server/alma.txt", UriConverter.FromFilePath(@"\\server\alma.txt").ToString());
        }

        [TestMethod]
        public void ToFilePathTest()
        {
            Assert.AreEqual("alma.txt", UriConverter.ToFilePath(new Uri("alma.txt", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"relative\path\alma.txt", UriConverter.ToFilePath(new Uri("relative/path/alma.txt", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"\absolute\path\alma.txt", UriConverter.ToFilePath(new Uri("file:///absolute/path/alma.txt")));
            Assert.AreEqual(@"c:\absolute\path\alma.txt", UriConverter.ToFilePath(new Uri("file:///c:/absolute/path/alma.txt")));
            Assert.AreEqual(@"\\server\alma.txt", UriConverter.ToFilePath(new Uri("file://server/alma.txt")));
        }

        [TestMethod]
        public void GetPathTest()
        {
            Assert.AreEqual("alma.txt", UriConverter.GetPath(new Uri("alma.txt", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"relative/path/alma.txt", UriConverter.GetPath(new Uri("relative/path/alma.txt", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"/absolute/path/alma.txt", UriConverter.GetPath(new Uri("file:///absolute/path/alma.txt")));
            Assert.AreEqual(@"c:\absolute\path\alma.txt", UriConverter.GetPath(new Uri("file:///c:/absolute/path/alma.txt")));
            Assert.AreEqual(@"\\server\alma.txt", UriConverter.GetPath(new Uri("file://server/alma.txt")));

            Assert.AreEqual(@"/dir/file.txt", UriConverter.GetPath(new Uri("http://server/dir/file.txt")));
            Assert.AreEqual(@"/dir/file.txt", UriConverter.GetPath(new Uri("http://server/dir/file.txt?alma=1")));
            Assert.AreEqual(@"/dir/file.txt", UriConverter.GetPath(new Uri("http://server/dir/file.txt#fragment")));
            Assert.AreEqual(@"/dir/file.txt", UriConverter.GetPath(new Uri("http://server/dir/file.txt?alma=1#fragment")));

            Assert.AreEqual(@"dir/file.txt", UriConverter.GetPath(new Uri("dir/file.txt", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"dir/file.txt", UriConverter.GetPath(new Uri("dir/file.txt?alma=1", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"dir/file.txt", UriConverter.GetPath(new Uri("dir/file.txt#fragment", UriKind.RelativeOrAbsolute)));
            Assert.AreEqual(@"dir/file.txt", UriConverter.GetPath(new Uri("dir/file.txt?alma=1#fragment", UriKind.RelativeOrAbsolute)));
        }
    }
}
