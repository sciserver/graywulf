using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public class WebJobInstanceFactory : EntityFactory
    {
        private Guid userGuid;
        private Guid queueInstanceGuid;
        private HashSet<Guid> jobDefinitionGuids;
        private JobExecutionState jobExecutionStatus;

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public Guid QueueInstanceGuid
        {
            get { return queueInstanceGuid; }
            set { queueInstanceGuid = value; }
        }

        public HashSet<Guid> JobDefinitionGuids
        {
            get { return jobDefinitionGuids; }
            set { jobDefinitionGuids = value; }
        }

        public JobExecutionState JobExecutionStatus
        {
            get { return jobExecutionStatus; }
            set { jobExecutionStatus = value; }
        }

        public WebJobInstanceFactory(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.userGuid = Guid.Empty;
            this.queueInstanceGuid = Guid.Empty;
            this.jobDefinitionGuids = new HashSet<Guid>();
            this.jobExecutionStatus = Registry.JobExecutionState.All;
        }

        public int CountChildren()
        {
            string sql = "spFindJobInstance_byDetails";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                AppendCommandParameters(cmd, -1, -1);

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["@RowCount"].Value;
            }
        }

        public IEnumerable<Entity> SelectChildren(int from, int max)
        {
            string sql = "spFindJobInstance_byDetails";

            using (SqlCommand cmd = Context.CreateStoredProcedureCommand(sql))
            {
                AppendCommandParameters(cmd, from, max);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Entity item = new Entity(Context);
                        item.LoadFromDataReader(dr);

                        string classname = "Jhu.Graywulf.Registry." + item.EntityType.ToString();
                        Type classtype = global::System.Reflection.Assembly.GetExecutingAssembly().GetType(classname);

                        item = (Entity)classtype.GetConstructor(new Type[] { typeof(Context) }).Invoke(new object[] { Context });

                        item.LoadFromDataReader(dr);

                        yield return item;
                    }
                    dr.Close();
                }
            }
        }

        private void AppendCommandParameters(SqlCommand cmd, int from, int max)
        {
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
            cmd.Parameters.Add("@From", SqlDbType.Int).Value = from == -1 ? DBNull.Value : (object)from;
            cmd.Parameters.Add("@Max", SqlDbType.Int).Value = max == -1 ? DBNull.Value : (object)max;
            cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;

            cmd.Parameters.Add("@JobUserGuid", SqlDbType.UniqueIdentifier).Value = userGuid == Guid.Empty ? DBNull.Value : (object)userGuid;
            cmd.Parameters.Add("@QueueInstanceGuid", SqlDbType.UniqueIdentifier).Value = queueInstanceGuid == Guid.Empty ? DBNull.Value : (object)queueInstanceGuid;
            cmd.Parameters.Add("@JobDefinitionGuids", SqlDbType.Structured).Value = CreateGuidListTable(jobDefinitionGuids);
            cmd.Parameters.Add("@JobExecutionStatus", SqlDbType.Int).Value = jobExecutionStatus;
        }
    }
}
