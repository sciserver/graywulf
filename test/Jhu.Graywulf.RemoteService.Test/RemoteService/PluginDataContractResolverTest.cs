using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.RemoteService
{
    [TestClass]
    public class PluginDataContractResolverTest
    {
        private string Serialize(object value, Type type)
        {
            using (var m = new MemoryStream())
            {
                var w = XmlDictionaryWriter.CreateTextWriter(m, Encoding.Unicode);

                //var s = new DataContractSerializer(type, null, Int32.MaxValue, false, false, null, new PluginDataContractResolver());
                var s = new NetDataContractSerializer();

                s.WriteObject(w, value);
                w.Flush();
                w.Close();

                return System.Text.Encoding.Unicode.GetString(m.ToArray());
            }
        }

        private object Deserialize(string value, Type type)
        {

            using (var m = new MemoryStream(Encoding.Unicode.GetBytes(value)))
            {
                var r = XmlDictionaryReader.CreateTextReader(m, Encoding.Unicode, XmlDictionaryReaderQuotas.Max, null);

                //var s = new DataContractSerializer(type, null, Int32.MaxValue, false, false, null, new PluginDataContractResolver());
                var s = new NetDataContractSerializer();

                return s.ReadObject(r);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            DataFileBase csv = new CsvFile("", Format.DataFileMode.Write);
            csv = (DataFileBase)Deserialize(Serialize(csv, typeof(DataFileBase)), typeof(DataFileBase));

            Assert.IsInstanceOfType(csv, typeof(CsvFile));
        }
    }
}
