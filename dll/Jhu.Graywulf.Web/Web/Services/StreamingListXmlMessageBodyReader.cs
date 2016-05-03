using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services
{
    public class StreamingListXmlMessageBodyReader
    {
        private Type returnType;
        private Encoding encoding;

        public StreamingListXmlMessageBodyReader(Type returnType)
            :this(returnType, System.Text.Encoding.ASCII)
        {
            // overload
        }

        public StreamingListXmlMessageBodyReader(Type returnType, Encoding encoding)
        {
            this.returnType = returnType;
            this.encoding = encoding;
        }

        public object ReadBodyContents(System.Xml.XmlDictionaryReader reader)
        {
            // TODO: when to return null?

            object result = Activator.CreateInstance(returnType);

            // Figure out list name
            string className, classNamespace;
            Dictionary<string, PropertyInfo> properties;
            StreamingListFormatter.ReflectClass(returnType, out className, out classNamespace, out properties);

            reader.ReadStartElement(className);

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // Look up property and deserialize contents
                    var prop = properties[reader.Name];

                    if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        // Advance reader to the first item
                        // TODO: what if no elements in list?
                        reader.Read();

                        // Read values into a List<T>

                        var itemType = prop.PropertyType.GetGenericArguments()[0];
                        var listType = typeof(List<>).MakeGenericType(itemType);
                        var list = (IList)Activator.CreateInstance(listType);
                        
                        var s = new DataContractSerializer(itemType);
                        var tagname = reader.Name;

                        while (true)
                        {
                            if (reader.NodeType == XmlNodeType.Element &&
                                StringComparer.InvariantCultureIgnoreCase.Compare(reader.Name, tagname) == 0)
                            {
                                var value = s.ReadObject(reader);
                                list.Add(value);
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                // End of list
                                break;
                            }
                            else
                            {
                                // Possibly a whitespace, let reader advance
                                reader.Read();
                            }
                        }

                        prop.SetValue(result, list);
                    }
                    else
                    {
                        // fallback to DataContractSerializer
                        var s = new DataContractSerializer(prop.PropertyType);
                        var value = s.ReadObject(reader);
                        prop.SetValue(result, value);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else
                {
                    // Possibly a whitespace, let reader advance
                }
            }

            reader.ReadEndElement();

            return result;
        }


    }
}
