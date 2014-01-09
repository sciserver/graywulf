﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Jhu.Graywulf.Registry
{
    public class EntitySettings
    {
        private string xmltext;
        private Dictionary<string, string> items;
        private bool isDirty;

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
            this.xmltext = null;
            this.items = null;
            this.isDirty = false;
        }

        private void CopyMembers(EntitySettings old)
        {
            this.xmltext = old.XmlText;  // Make sure read via the property
            this.items = null;
            this.isDirty = false;
        }

        private void LoadItems()
        {
            items = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // TODO: what if xmltext's empty?

            var xml = new XmlDocument();
            xml.LoadXml(xmltext);

            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                items.Add(node.Name, node.Attributes["value"].Value);
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

                xmltext = xml.ToString();
                isDirty = false;
            }
        }

        protected string GetValue(string key)
        {
            if (items == null)
            {
                LoadItems();
            }

            return null;
        }

        protected void SetValue(string key, string value)
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
