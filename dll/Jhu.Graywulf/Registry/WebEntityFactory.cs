using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public class WebEntityFactory : ContextObject
    {
        private Entity parentEntity;
        private EntityType childrenType;

        public Entity ParentEntity
        {
            get { return parentEntity; }
            set { parentEntity = value; }
        }

        public EntityType ChildrenType
        {
            get { return childrenType; }
            set { childrenType = value; }
        }

        public WebEntityFactory()
        {
        }

        public WebEntityFactory(Context context)
            : base(context)
        {
        }

        public int CountChildren()
        {
            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*
	FROM Entity
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @Guid AND
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT @RowCount = COUNT(*) FROM q
";

            sql = String.Format(sql, childrenType.ToString());

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@From", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@Max", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = parentEntity.Guid;

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["@RowCount"].Value;
            }
        }

        public IEnumerable<Entity> SelectChildren(int from, int max)
        {
            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*, ROW_NUMBER () OVER ( ORDER BY Entity.Number ) AS rn
	FROM Entity
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @Guid AND
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT q.* FROM q
WHERE rn BETWEEN @From + 1 AND @From + @Max OR @From IS NULL OR @Max IS NULL
ORDER BY rn

SET @RowCount = @@ROWCOUNT
";

            sql = String.Format(sql, childrenType.ToString());

            using (SqlCommand cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@From", SqlDbType.Int).Value = from;
                cmd.Parameters.Add("@Max", SqlDbType.Int).Value = max;
                cmd.Parameters.Add("@RowCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = parentEntity.Guid;

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
    }
}
