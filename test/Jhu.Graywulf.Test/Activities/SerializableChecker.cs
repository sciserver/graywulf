using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// Takes a class and verifies if it's serializable by
    /// taking all possible derived classes into account
    /// </summary>
    public class SerializableChecker
    {
        private Assembly[] assemblies;
        private HashSet<Type> visitedTypes;
        private HashSet<Type> serializableTypes;
        private Dictionary<Type, string> nonserializableTypes;

        public bool Execute(Type t)
        {
            return Execute(t, null);
        }

        public bool Execute(Type t, Assembly[] assemblies)
        {
            this.assemblies = assemblies;
            this.visitedTypes = new HashSet<Type>();
            this.serializableTypes = new HashSet<Type>();
            this.nonserializableTypes = new Dictionary<Type, string>();

            // Look for SerializableAttribute
            var res = CheckType(t, t.Name);

            // Try to serialize with NetDataContractSerializer

            res &= TryNetDataContractSerializer(t);

            return res;
        }

        private bool TryNetDataContractSerializer(Type type)
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

                var s = new NetDataContractSerializer(
                    new StreamingContext(),
                    int.MaxValue,
                    false,
                    System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                    null);

                var o = Activator.CreateInstance(type, true);

                s.WriteObject(w, o);
                w.Flush();
                w.Close();

                // Don't forget to skip byte order mark
                var buffer = m.ToArray();
                var prelen = Encoding.Unicode.GetPreamble().Length;
                var xml = System.Text.Encoding.Unicode.GetString(buffer, prelen, buffer.Length - prelen);
            }

            return true;
        }

        private bool CheckType(Type type, string fieldName)
        {
            IEnumerable<Type> types;
            if (assemblies == null)
            {
                types = FindDerivedTypes(type);
            }
            else
            {
                types = FindDerivedTypes(type, assemblies);
            }

            foreach (var t in types)
            {
                if (!CheckSingleType(t, fieldName))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckSingleType(Type t, string fieldName)
        {
            // TODO: add all known derived types

            if (serializableTypes.Contains(t))
            {
                return true;
            }

            if (nonserializableTypes.ContainsKey(t))
            {
                return false;
            }

            if (!t.IsSerializable)
            {
                nonserializableTypes.Add(t, fieldName);
                return false;
            }

            visitedTypes.Add(t);

            foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (!visitedTypes.Contains(f.FieldType) && !CheckField(f, fieldName))
                {
                    nonserializableTypes.Add(t, String.Format("{0}.{1}", fieldName, f.Name));
                    return false;
                }
            }

            serializableTypes.Add(t);
            return true;
        }

        private bool CheckField(FieldInfo f, string fieldName)
        {
            // Skip if field is marked as NonSerialized
            var attrs = f.GetCustomAttributes(typeof(NonSerializedAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                return true;
            }

            // Otherwise check field type
            return CheckType(f.FieldType, String.Format("{0}.{1}", fieldName, f.Name));
        }

        #region Derived type discovery

        /// <summary>
        /// Returns types that are inherited from a given type or implement a given interface.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private HashSet<Type> FindDerivedTypes(Type t)
        {
            return FindDerivedTypes(t, AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Returns types that are inherited from a given type or implement a given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private HashSet<Type> FindDerivedTypes(Type type, Assembly[] assemblies)
        {
            if (type.IsInterface)
            {
                return FindInterfaceImplementingTypes(type, assemblies);
            }
            else
            {
                return FindInheritedTypes(type, assemblies);
            }
        }

        /// <summary>
        /// Returns types that implement a given interface.
        /// </summary>
        /// <param name="intf"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private HashSet<Type> FindInterfaceImplementingTypes(Type intf, Assembly[] assemblies)
        {
            var name = intf.FullName;
            var res = new HashSet<Type>();

            foreach (var a in assemblies)
            {
                foreach (var t in a.GetTypes())
                {
                    if (!res.Contains(t) && t.GetInterface(name) != null)
                    {
                        res.Add(t);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns types that are inherited from a given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private HashSet<Type> FindInheritedTypes(Type type, Assembly[] assemblies)
        {
            var res = new HashSet<Type>();

            foreach (var a in assemblies)
            {
                try
                {
                    var types = a.GetTypes();

                    foreach (var t in a.GetTypes())
                    {
                        if (!res.Contains(t) && t.IsSubclassOf(type))
                        {
                            res.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return res;
        }

        #endregion
    }
}
