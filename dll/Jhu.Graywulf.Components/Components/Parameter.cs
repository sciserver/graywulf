using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements a base class for any entity parameter that is
    /// stored as an xml.
    /// </summary>
    /// <remarks>
    /// The advantage of these parameters is
    /// that they are not intantiated by the framework itself,
    /// only by the plugins, so they may contain types that
    /// are not known by the framework.
    /// </remarks>
    [Serializable]
    public class Parameter : ICloneable
    {
        #region Private member variables

        private string name;
        private string typeName;
        private ParameterDirection direction;
        private object value;
        private string xmlValue;

        #endregion

        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlAttribute]
        [DefaultValue(null)]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        [XmlAttribute]
        [DefaultValue(ParameterDirection.Unknown)]
        public ParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [XmlIgnore]
        public object Value
        {
            get
            {
                if (value == null && xmlValue == null)
                {
                    return null;
                }
                else if (value == null && xmlValue != null)
                {
                    this.value = DeserializeValue(xmlValue);
                }

                return this.value;
            }
            set
            {
                this.value = value;
                this.xmlValue = null;
            }
        }

        [XmlIgnore]
        public string XmlValue
        {
            get
            {
                if (value == null && xmlValue == null)
                {
                    return null;
                }
                else if (value != null && xmlValue == null)
                {
                    xmlValue = SerializeValue(value);
                }

                return this.xmlValue;
            }
            set
            {
                this.value = null;
                this.xmlValue = value;
            }
        }

        [XmlElement("Value")]
        [DefaultValue(null)]
        public XmlDocument XmlValue_ForXml
        {
            get
            {
                if (String.IsNullOrEmpty(XmlValue))  // Always use property here for lazy loading
                {
                    return null;
                }
                else
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(XmlValue);          // Always use property here for lazy loading
                    return doc;
                }
            }
            set
            {
                XmlValue = value == null ? null : value.InnerXml;
            }
        }

        public Parameter()
        {
            InitializeMembers();
        }

        public Parameter(Parameter old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.typeName = null;
            this.direction = ParameterDirection.Unknown;
            this.value = null;
            this.xmlValue = null;
        }

        private void CopyMembers(Parameter old)
        {
            this.name = old.name;
            this.typeName = old.typeName;
            this.direction = old.direction;
            this.value = old.value;
            this.xmlValue = old.xmlValue;
        }

        public virtual object Clone()
        {
            return new Parameter(this);
        }

        private object DeserializeValue(string xmltext)
        {
            using (var m = new MemoryStream(Encoding.Unicode.GetBytes(xmltext)))
            {
                var r = XmlDictionaryReader.CreateTextReader(
                    m,
                    Encoding.Unicode,
                    XmlDictionaryReaderQuotas.Max,
                    null);

                var s = GetSerializer();
                var res = s.ReadObject(r);

                return res;
            }
        }

        private string SerializeValue(object value)
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
                return System.Text.Encoding.Unicode.GetString(buffer, prelen, buffer.Length - prelen);
            }
        }

        private XmlObjectSerializer GetSerializer()
        {
            var res = new NetDataContractSerializer(
                new StreamingContext(),
                int.MaxValue,
                false,
                System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
                null);

            return res;
        }
    }
}
