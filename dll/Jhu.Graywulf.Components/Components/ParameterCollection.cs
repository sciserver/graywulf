using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Components
{
    [Serializable]
    public class ParameterCollection : Dictionary<string, Parameter>
    {
        #region Constructors and initializers

        public ParameterCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        protected ParameterCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ParameterCollection(Parameter[] items)
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            AddRange(items);
        }

        public ParameterCollection(ParameterCollection old)
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            foreach (var item in Util.DeepCloner.CloneDictionary(old))
            {
                this.Add(item.Key, item.Value);
            }
        }

        public bool CompareItems(ParameterCollection other)
        {
            // Compare keys
            foreach (var k in this.Keys)
            {
                if (!other.ContainsKey(k))
                {
                    return false;
                }
            }

            foreach (var k in other.Keys)
            {
                if (!this.ContainsKey(k))
                {
                    return false;
                }
            }

            bool eq = true;

            foreach (var k in this.Keys)
            {
                eq &= this[k].CompareMembers(other[k]); 
            }

            return eq;
        }

        #endregion
        #region Collection implementation

        public T GetValue<T>(string key)
        {
            return (T)this[key].Value;
        }

        public void SetValue<T>(string key, T value)
        {
            if (!this.ContainsKey(key))
            {
                this[key] = new Parameter()
                {
                    Name = key,
                    Value = value
                };
            }
            else
            {
                this[key].Value = value;
            }
        }


        public void Add(Parameter item)
        {
            Add(item.Name, item);
        }

        public void AddRange(Parameter[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                this.Add(items[i]);
            }
        }

        public Parameter[] GetAsArray()
        {
            if (Count == 0)
            {
                return null;
            }
            else
            {
                return Values.ToArray();
            }
        }

        #endregion
        #region XML serialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlText"></param>
        /// <remarks>
        /// This function is used for loading the parameters from the database
        /// </remarks>
        public void LoadFromXml(string xmlText)
        {
            Clear();

            if (!String.IsNullOrEmpty(xmlText))
            {
                using (var sr = new StringReader(xmlText))
                {
                    var s = new XmlSerializer(typeof(Parameter[]));
                    AddRange((Parameter[])s.Deserialize(sr));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function is used for saving the parameters into the database
        /// </remarks>
        public string SaveToXml()
        {
            if (Count == 0)
            {
                return null;
            }
            else
            {
                var sb = new StringBuilder();

                using (var xw = XmlWriter.Create(sb))
                {
                    var s = new XmlSerializer(typeof(Parameter[]));
                    s.Serialize(xw, Values.ToArray());
                }

                return sb.ToString();
            }
        }

        #endregion
    }
}
