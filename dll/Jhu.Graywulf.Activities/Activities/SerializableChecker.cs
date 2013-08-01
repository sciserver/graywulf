using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// Takes a class and verifies if it's serializable by
    /// taking all possible derived classes into account
    /// </summary>
    public class SerializableChecker
    {
        private Assembly[] assemblies;
        private ReflectionHelperInternal rh;

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
            rh = new ReflectionHelperInternal();

            this.visitedTypes = new HashSet<Type>();
            this.serializableTypes = new HashSet<Type>();
            this.nonserializableTypes = new Dictionary<Type, string>();

            return CheckType(t, t.Name);
        }

        private bool CheckType(Type type, string fieldName)
        {
            IEnumerable<Type> types;
            if (assemblies == null)
            {
                types = rh.FindDerivedTypes(type);
            }
            else
            {
                types = rh.FindDerivedTypes(type, assemblies);
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
    }
}
