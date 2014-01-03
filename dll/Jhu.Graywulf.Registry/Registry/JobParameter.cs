using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Registry
{
    [Serializable]
    public class JobParameter
    {
        private string name;
        private string typeName;
        private JobParameterDirection direction;
        private string xmlValue;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public JobParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public string XmlValue
        {
            get { return this.xmlValue; }
            set { this.xmlValue = value; }
        }

        public JobParameter()
        {
            InitializeMembers();
        }

        public JobParameter(JobParameter old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.typeName = null;
            this.direction = JobParameterDirection.Unknown;
            this.xmlValue = null;
        }

        private void CopyMembers(JobParameter old)
        {
            this.name = old.name;
            this.typeName = old.typeName;
            this.direction = old.direction;
            this.xmlValue = old.xmlValue;
        }

        public object GetValue()
        {
            return GetValue(Type.GetType(typeName));
        }

        public T GetValue<T>()
        {
            return (T)GetValue(typeof(T));
        }

        public object GetValue(Type type)
        {
            using (var m = new MemoryStream(Encoding.Unicode.GetBytes(xmlValue)))
            {
                var r = XmlDictionaryReader.CreateTextReader(m, Encoding.Unicode, XmlDictionaryReaderQuotas.Max, null);

                var s = GetSerializer();
                var res = s.ReadObject(r);

                return res;
            }
        }

        public void SetValue(object value)
        {
            using (var m = new MemoryStream())
            {
                var w = XmlTextWriter.Create(
                    m,
                    new XmlWriterSettings()
                        {
                            Indent = true,
                            Encoding = Encoding.Unicode,
                            NamespaceHandling = NamespaceHandling.OmitDuplicates,
                        });

                var s = GetSerializer();

                s.WriteObject(w, value);
                w.Flush();
                w.Close();

                // Don't forget to skip byte order mark
                var buffer = m.ToArray();
                var prelen = Encoding.Unicode.GetPreamble().Length;
                xmlValue = System.Text.Encoding.Unicode.GetString(buffer, prelen, buffer.Length - prelen);
            }
        }


        private XmlObjectSerializer GetSerializer()
        {
            var res = new NetDataContractSerializer(
                new StreamingContext(),
                int.MaxValue,
                false,
                System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                null);

            return res;
        }
    }
}
