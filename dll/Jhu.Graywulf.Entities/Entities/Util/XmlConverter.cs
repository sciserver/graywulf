using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Jhu.Graywulf.Entities.Util
{
    public static class XmlConverter
    {
        public delegate void ToXmlDelegate(XmlWriter w);
        public delegate T FromXmlDelegate<T>(XmlReader r);

        private static XmlWriterSettings XmlWriterSettings
        {
            get
            {
                return new XmlWriterSettings()
                {
                    CloseOutput = true,
                    Encoding = Encoding.Unicode,
                };
            }
        }

        private static XmlReaderSettings XmlReaderSettings
        {
            get
            {
                return new XmlReaderSettings()
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                };
            }
        }

        public static string ToXml<T>(T obj, ToXmlDelegate action)
        {
            var sw = new StringWriter();
            var w = new XmlTextWriter(sw);

            action(w);

            return sw.ToString();
        }

        public static void ToXml<T>(T obj, Stream stream, ToXmlDelegate action)
        {
            var w = new XmlTextWriter(stream, Encoding.Unicode);
            action(w);
        }

        public static T FromXml<T>(Stream stream, FromXmlDelegate<T> action)
        {
            var r = (XmlReader)XmlTextReader.Create(stream, XmlReaderSettings);
            return action(r);
        }

        public static T FromXml<T>(string xml, FromXmlDelegate<T> action)
        {
            var sr = new StringReader(xml);
            var r = (XmlReader)XmlTextReader.Create(             sr, XmlReaderSettings);
            return action(r);
        }
    }
}
