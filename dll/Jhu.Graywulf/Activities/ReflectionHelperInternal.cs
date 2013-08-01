using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Activities;
using System.Windows.Markup;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Activities
{
    /// <summary>
    /// This class provides functionality for reflecting workflows through app domains.
    /// </summary>
    public class ReflectionHelperInternal : MarshalByRefObject, IDisposable
    {
        private HashSet<string> loadedAssemblyNames;

        public ReflectionHelperInternal()
        {
            InitializeMembers();
            InitializeAppDomain();
        }

        private void InitializeMembers()
        {
            this.loadedAssemblyNames = new HashSet<string>();
        }

        /// <summary>
        /// Frees resources and detaches event handles.
        /// </summary>
        public void Dispose()
        {
            AppDomain ad = AppDomain.CurrentDomain;

            ad.AssemblyLoad -= ad_AssemblyLoad;
            ad.AssemblyResolve -= ad_AssemblyResolve;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void InitializeAppDomain()
        {
            AppDomain ad = AppDomain.CurrentDomain;

            ad.AssemblyLoad += ad_AssemblyLoad;
            ad.AssemblyResolve += ad_AssemblyResolve;
        }

        #region AppDomain event handlers

        /// <summary>
        /// Handles assembly load requests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function supports loading assemblies from a custom directory
        /// </remarks>
        Assembly ad_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // *** TODO: figure out how to load assemblies without locking them

            // Try to load assembly from the default locations
            AssemblyName an = new AssemblyName(args.Name);

            Assembly a = null;

            // Try to load assembly from default directory
            if (TryLoadAssembly(an.Name + ".dll", an, out a))
            {
                return a;
            }

            // Try to load from the workflow assembly path
            string[] files = Directory.GetFiles(AppSettings.WorkflowAssemblyPath, an.Name + ".dll", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (TryLoadAssembly(files[i], an, out a))
                {
                    return a;
                }
            }

            // Try to load from the same location where the 
            // requesting assembly is
            if (args.RequestingAssembly != null)
            {
                if (TryLoadAssembly(
                        Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), an.Name + ".dll"),
                        an,
                        out a))
                {
                    return a;
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Tries to load assembly from a given location.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="name"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private bool TryLoadAssembly(string filename, AssemblyName name, out Assembly a)
        {
            if (File.Exists(filename))
            {

                a = Assembly.ReflectionOnlyLoadFrom(filename);
                if (a.GetName().Name == name.Name)  // **** TODO: do some version testing here?
                {
                    // *** TODO: first read into memory to avoid locking the dll
                    // unfortunately this breaks workflows
                    //a = Assembly.Load(File.ReadAllBytes(filename));
                    a = Assembly.LoadFrom(filename);
                    return true;
                }
                else
                {
                    a = null;
                    return false;
                }
            }
            else
            {
                a = null;
                return false;
            }
        }

        /// <summary>
        /// Called when the runtime is trying to load an assembly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ad_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            loadedAssemblyNames.Add(args.LoadedAssembly.FullName);
        }

        /// <summary>
        /// Extracts the dependency properties of the workflow type that are flagged with
        /// the <see cref="WorkflowParameterAttribute"/> attribute.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, JobParameter> GetWorkflowParameters(string typeName)
        {
            var excluded = new HashSet<string>() { "JobGuid", "UserGuid" };

            var t = Type.GetType(typeName);

            var res = new Dictionary<string, JobParameter>();

            foreach (PropertyInfo pinfo in t.GetProperties())
            {
                if (pinfo.PropertyType.IsGenericType)
                {
                    Type gt = pinfo.PropertyType.GetGenericTypeDefinition();

                    if (!excluded.Contains(pinfo.Name))
                    {
                        JobParameterDirection dir;
                        
                        if (gt == typeof(System.Activities.InArgument<>))
                        {
                            dir = JobParameterDirection.In;
                        }
                        else if (gt == typeof(System.Activities.InOutArgument<>))
                        {
                            dir = JobParameterDirection.InOut;
                        }
                        else if (gt == typeof(System.Activities.OutArgument<>))
                        {
                            dir = JobParameterDirection.Out;
                        }
                        else
                        {
                            continue;
                        }

                        var par = new JobParameter()
                        {
                            Name = pinfo.Name,
                            TypeName = pinfo.PropertyType.GetGenericArguments()[0].AssemblyQualifiedName,
                            Direction = dir,
                            XmlValue = null
                        };

                        res.Add(pinfo.Name, par);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Extracts the checkpoints (<see cref="CheckpointActivity"/>) from the workflow.
        /// </summary>
        /// <returns>A list with the names of the checkpoints.</returns>
        /// <remarks>
        /// The function creates an instance of the workflow class and digs down
        /// in the activity hierarchy to get all activities with the type of <see cref="CheckpointActivity"/>.
        /// </remarks>
        public List<string> GetWorkflowCheckpoints(string typeName)
        {
            Type t = Type.GetType(typeName);

            return GetWorkflowCheckpoints((Activity)Activator.CreateInstance(t, true));
        }

        private List<string> GetWorkflowCheckpoints(Activity root)
        {
            List<string> res = new List<string>();

            // Inspect the activity tree using WorkflowInspectionServices.
            IEnumerator<Activity> activities = WorkflowInspectionServices.GetActivities(root).GetEnumerator();

            while (activities.MoveNext())
            {
                if (activities.Current is ICheckpoint)
                {
                    ICheckpoint cp = (ICheckpoint)activities.Current;
                    res.Add(cp.CheckpointName.Expression.ToString());
                }

                res.AddRange(GetWorkflowCheckpoints(activities.Current));
            }

            return res;
        }

        /// <summary>
        /// Checks if a type implements a given interface.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public bool IsTypeOfInterface(string typeName, string interfaceName)
        {
            Type t = Type.GetType(typeName);

            return t.GetInterface(interfaceName) != null;
        }

        /// <summary>
        /// Returns types that are inherited from types of parameters of a workflow.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public HashSet<string> GetWokflowParametersInheritedTypes(string typeName)
        {
            var wfpars = GetWorkflowParameters(typeName);

            var types = new HashSet<string>();

            foreach (string typename in wfpars.Values.Select(i => i.TypeName))
            {
                if (!types.Contains(typename))
                {
                    GetXmlSerializableTypes(Type.GetType(typename), types);
                }
            }

            return types;
        }

        private void GetXmlSerializableTypes(Type type, HashSet<string> types)
        {
            // If it's an array, find its base type
            while (type.IsArray)
            {
                type = type.BaseType;
            }

            string typename = type.AssemblyQualifiedName;

            if (!type.IsPrimitive && !type.IsSealed)
            {
                if (type.IsValueType)
                {
                    throw new NotImplementedException();
                }

                // Only classes are to be collected
                if (type.IsClass)
                {
                    if (!types.Contains(typename))
                    {
                        types.Add(typename);
                        GetXmlSerializableMemberTypes(type, types);
                    }
                }

                // Find types inherited from the current type
                // *** TODO: might need to load additional assemblies here
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(type))
                        {
                            GetXmlSerializableTypes(t, types);
                        }
                    }
                }
            }

            if (type.IsGenericType)
            {
                foreach (Type gpt in type.GetGenericArguments())
                {
                    GetXmlSerializableTypes(gpt, types);
                }
            }
        }

        private void GetXmlSerializableMemberTypes(Type type, HashSet<string> types)
        {
            MemberInfo[] members = type.FindMembers(
                MemberTypes.Field | MemberTypes.Property,
                BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty,
                null,
                null);

            foreach (MemberInfo m in members)
            {
                // Ignore fields that are marked with XmlIgnore
                if (!m.IsDefined(typeof(System.Xml.Serialization.XmlIgnoreAttribute), true))
                {
                    Type tt;

                    switch (m.MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo f = (FieldInfo)m;
                            tt = f.FieldType;
                            break;
                        case MemberTypes.Property:
                            PropertyInfo p = (PropertyInfo)m;
                            tt = p.PropertyType;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    GetXmlSerializableTypes(tt, types);
                }
            }
        }

        #region Derived type discovery

        /// <summary>
        /// Returns types that are inherited from a given type or implement a given interface.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public HashSet<Type> FindDerivedTypes(Type t)
        {
            return FindDerivedTypes(t, AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Returns types that are inherited from a given type or implement a given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public HashSet<Type> FindDerivedTypes(Type type, Assembly[] assemblies)
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
        public HashSet<Type> FindInterfaceImplementingTypes(Type intf, Assembly[] assemblies)
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
        public HashSet<Type> FindInheritedTypes(Type type, Assembly[] assemblies)
        {
            var res = new HashSet<Type>();

            foreach (var a in assemblies)
            {
                foreach (var t in a.GetTypes())
                {
                    if (!res.Contains(t) && t.IsSubclassOf(type))
                    {
                        res.Add(t);
                    }
                }
            }

            return res;
        }

        #endregion

        /// <summary>
        /// Returns the name of assemblies that were loaded while the
        /// app domain had this class instantiated
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetLoadedAssemblyNames()
        {
            return loadedAssemblyNames;
        }
    }
}
