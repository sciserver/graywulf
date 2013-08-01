using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.RemoteService
{
    public class PluginDataContractResolver : DataContractResolver
    {

        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                XmlDictionary dictionary = new XmlDictionary();

                //typeName = dictionary.Add(dataContractType.Name);
                //typeNamespace = dictionary.Add(String.Format("http://graywulf.jhu/{0}", dataContractType.Namespace));

                typeName = dictionary.Add(dataContractType.AssemblyQualifiedName);
                typeNamespace = dictionary.Add(dataContractType.Namespace);
            }

            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            var type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            if (type != null)
            {
                return type;
            }
            else
            {
                string fulltypename;

                if (typeName.IndexOf(',') >= 0)
                {
                    fulltypename = typeName;
                }
                else
                {
                    var i = typeNamespace.LastIndexOf('/');
                    if (i >= 0)
                    {
                        typeNamespace = typeNamespace.Substring(i + 1);
                    }

                    if (!String.IsNullOrWhiteSpace(typeNamespace))
                    {
                        fulltypename = String.Format("{0}.{1}", typeNamespace, typeName);
                    }
                    else
                    {
                        fulltypename = typeName;
                    }
                }

                var t = Type.GetType(fulltypename);
                return t;
            }
        }

#if msdnsample
        private Dictionary<string, int> serializationDictionary;
        private Dictionary<int, string> deserializationDictionary;
        private int serializationIndex = 0;
        private XmlDictionary dic;

        public PluginDataContractResolver()
        {
            serializationDictionary = new Dictionary<string, int>();
            deserializationDictionary = new Dictionary<int, string>();
            dic = new XmlDictionary();
        }

        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                return false;
            }

            int index;
            if (serializationDictionary.TryGetValue(typeNamespace.Value, out index))
            {
                typeNamespace = dic.Add(index.ToString());
            }
            else
            {
                serializationDictionary.Add(typeNamespace.Value, serializationIndex);
                typeNamespace = dic.Add(serializationIndex++ + "#" + typeNamespace);
            }

            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Type type;

            int deserializationIndex;
            int poundIndex = typeNamespace.IndexOf("#");

            if (poundIndex < 0)
            {
                if (Int32.TryParse(typeNamespace, out deserializationIndex))
                {
                    deserializationDictionary.TryGetValue(deserializationIndex, out typeNamespace);
                }

                type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);

            }
            else
            {
                if (Int32.TryParse(typeNamespace.Substring(0, poundIndex), out deserializationIndex))
                {
                    typeNamespace = typeNamespace.Substring(poundIndex + 1, typeNamespace.Length - poundIndex - 1);
                    deserializationDictionary.Add(deserializationIndex, typeNamespace);
                }

                type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }

            return type;
        }
#endif

#if false
        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                XmlDictionary dictionary = new XmlDictionary();

                typeName = dictionary.Add(dataContractType.FullName);
                typeNamespace = dictionary.Add(dataContractType.Assembly.FullName);
            }

            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            // TODO: test case with no namespace... possible?
            /*var i = typeNamespace.LastIndexOf('/');
            if (i >= 0)
            {
                typeNamespace = typeNamespace.Substring(i + 1);
            }

            string fulltypename;
            if (!String.IsNullOrWhiteSpace(typeNamespace))
            {
                fulltypename = String.Format("{0}.{1}", typeNamespace, typeName);
            }
            else
            {
                fulltypename = typeName;
            }*/

            XmlDictionaryString name;
            XmlDictionaryString namespace;

            name.
            
            if (dictionary.TryGetValue(typeName, out tName) && dictionary.TryGetValue(typeNamespace, out tNamespace))
            {
                return this.assembly.GetType(tNamespace.Value + "." + tName.Value);
            }
            else
            {
                return null;
            }


            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null) ?? Type.GetType(fulltypename);
        }
#endif
    }

}
