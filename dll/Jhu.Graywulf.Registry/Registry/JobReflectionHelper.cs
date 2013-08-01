using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Activities;
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
        public Dictionary<string, JobParameter> GetParameters()
        {
            var excluded = new HashSet<string>() { "JobGuid", "UserGuid" };

            var t = Type.GetType(workflowTypeName);

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
        public List<string> GetCheckpoints()
        {
            Type t = Type.GetType(workflowTypeName);
            return GetCheckpoints((Activity)Activator.CreateInstance(t, true));
        }

        private List<string> GetCheckpoints(Activity root)
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

                res.AddRange(GetCheckpoints(activities.Current));
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
            Type t = Type.GetType(workflowTypeName);
            return t.GetInterface(interfaceName) != null;
        }
    }
}
