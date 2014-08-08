using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Activities;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Registry
{
    public class JobReflectionHelper : MarshalByRefObject
    {
        public static JobReflectionHelper CreateInstance(string workflowTypeName)
        {
            var basedir = "";

            try
            {
                AppDomain ad;
                Components.AppDomainManager.Instance.GetAppDomainForType(workflowTypeName, true, out ad);

                basedir = ad.BaseDirectory;

                return (JobReflectionHelper)ad.CreateInstanceAndUnwrap(
                    typeof(JobReflectionHelper).Assembly.FullName,
                    typeof(JobReflectionHelper).FullName,
                    true,
                    BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { workflowTypeName },
                    null,
                    null);
            }
            catch (Exception ex)
            {
                // **** TODO
                throw new Exception(String.Format("Cannot create type {0} based in {1}", workflowTypeName, basedir), ex);
            }
        }

        private string workflowTypeName;

        protected JobReflectionHelper(string workflowTypeName)
        {
            this.workflowTypeName = workflowTypeName;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Extracts the dependency properties of the workflow type that are flagged with
        /// the <see cref="WorkflowParameterAttribute"/> attribute.
        /// </summary>
        /// <returns></returns>
        public ParameterCollection GetParameters()
        {
            var excluded = new HashSet<string>() { "JobGuid", "UserGuid" };

            var t = Type.GetType(workflowTypeName);

            var res = new ParameterCollection();

            foreach (PropertyInfo pinfo in t.GetProperties())
            {
                if (pinfo.PropertyType.IsGenericType)
                {
                    Type gt = pinfo.PropertyType.GetGenericTypeDefinition();

                    if (!excluded.Contains(pinfo.Name))
                    {
                        ParameterDirection dir;

                        if (gt == typeof(System.Activities.InArgument<>))
                        {
                            dir = ParameterDirection.In;
                        }
                        else if (gt == typeof(System.Activities.InOutArgument<>))
                        {
                            dir = ParameterDirection.InOut;
                        }
                        else if (gt == typeof(System.Activities.OutArgument<>))
                        {
                            dir = ParameterDirection.Out;
                        }
                        else
                        {
                            continue;
                        }

                        var par = new Parameter()
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
        /// Checks if a type implements a given interface.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public bool HasInterface(string interfaceName)
        {
            var t = Type.GetType(workflowTypeName);

            if (t == null)
            {
                return false;
            }
            else
            {
                return t.GetInterface(interfaceName) != null;
            }
        }
    }
}
