using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    public class EntitySettings
    {
        private string xmltext;
        private Dictionary<string, string> items;
        private bool isDirty;

        [XmlText]
        [DefaultValue("")]
        public string XmlText
        {
            get
            {
                if (isDirty)
                {
                    SaveItems();
                }

                return xmltext;
            }
            set
            {
                xmltext = value;
                items = null;
                isDirty = false;
            }
        }

        [XmlIgnore]
        public string this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public EntitySettings()
        {
            InitializeMembers();
        }

        public EntitySettings(EntitySettings old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.xmltext = String.Empty;
            this.items = null;
            this.isDirty = false;
        }

        private void CopyMembers(EntitySettings old)
        {
            this.xmltext = old.XmlText;  // Make sure to read via the property
            this.items = null;
            this.isDirty = false;
        }

        private void LoadItems()
        {
            items = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (!String.IsNullOrEmpty(xmltext))
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmltext);

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    string name;
                    string value;

                    name = node.Name;

                    if (node.Attributes["value"] != null)
                    {
                        value = node.Attributes["value"].Value;
                    }
                    else
                    {
                        value = node.InnerXml;
                    }

                    items.Add(name, value);
                }
            }
        }

        private void SaveItems()
        {
            if (isDirty)
            {
                var xml = new XmlDocument();
                xml.AppendChild(xml.CreateElement("settings"));

                foreach (var key in items.Keys)
                {
                    var node = xml.CreateElement(key.ToString());
                    var attr = xml.CreateAttribute("value");
                    attr.Value = items[key];
                    node.Attributes.Append(attr);
                    xml.DocumentElement.AppendChild(node);
                }

                var sb = new StringBuilder();
                var xs = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    IndentChars = "    ",
                };
                var xw = XmlWriter.Create(sb, xs);

                xml.WriteTo(xw);

                xmltext = sb.ToString();
                isDirty = false;
            }
        }

        public string GetValue(string key)
        {
            if (items == null)
            {
                LoadItems();
            }

            return items[key];
        }

        public void SetValue(string key, string value)
        {
            if (items == null)
            {
                LoadItems();
            }

            items[key] = value;

            isDirty = true;
        }
    }
}
