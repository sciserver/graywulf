using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Class for loading and searching entities.
    /// </summary>
    /// <remarks>
    /// Entity search functions do not contain stored procedure names
    /// explicitly
    /// </remarks>
    public class EntityFactory : ContextObject
    {
        public static string CombineName(EntityType entityType, params string[] nameParts)
        {
            var name = entityType.ToString() + Constants.EntityTypeSeparator;

            for (int i = 0; i < nameParts.Length; i++)
            {
                if (i > 0)
                {
                    name += Constants.EntityNameSeparator;
                }

                var idx = nameParts[i].IndexOf(Constants.EntityTypeSeparator);

                if (idx < 0)
                {
                    name += nameParts[i];
                }
                else
                {
                    name += nameParts[i].Substring(idx + 1);
                }
            }

            return name;
        }

        public static string CombineName(EntityType entityType, string parentName, string name)
        {
            var idx = parentName.IndexOf(Constants.EntityTypeSeparator);

            return entityType.ToString() + Constants.EntityTypeSeparator + parentName.Substring(idx + 1) + Constants.EntityNameSeparator + name;
        }

        public static string GetName(string name)
        {
            var idx = name.LastIndexOf(Constants.EntityNameSeparator);

            if (idx > -1)
            {
                return name.Substring(idx + 1);
            }
            else
            {
                idx = name.IndexOf(Constants.EntityTypeSeparator);
                if (idx > -1)
                {
                    return name.Substring(idx + 1);
                }
                else
                {
                    return name;
                }
            }
        }

        #region Constructors

        /// <summary>
        /// Creates an object with a valid context.
        /// </summary>
        /// <param name="context">A valid context object.</param>
        public EntityFactory(Context context)
            : base(context)
        {
            
        }

        #endregion
        
        #region Entity Search Functions

        /// <summary>
        /// Loads all entities of a given type.
        /// </summary>
        /// <typeparam name="ItemType">Type of the entities to load.</typeparam>
        /// <returns>An IEnumerable interface to the loaded objects.</returns>
        public IEnumerable<ItemType> FindAll<ItemType>()
            where ItemType : Entity, new()
        {
            var type = new ItemType().EntityType;

            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*, ROW_NUMBER () OVER ( ORDER BY Entity.Number ) AS rn
	FROM Entity
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
	WHERE
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT q.* FROM q
WHERE rn BETWEEN @From + 1 AND @From + @Max OR @From IS NULL OR @Max IS NULL
ORDER BY rn
";

            sql = String.Format(sql, type);

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@From", SqlDbType.Int).Value = DBNull.Value;
                cmd.Parameters.Add("@Max", SqlDbType.Int).Value = DBNull.Value;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ItemType item = new ItemType();
                        item.Context = Context;
                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }

        public IEnumerable<T> FindChildren<T>(Entity parent)
            where T : Entity, new()
        {
            var childrentype = Constants.EntityTypeMap[typeof(T)];

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
SELECT q.* FROM q
ORDER BY Number
";

            sql = String.Format(sql, childrentype);

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = parent.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        T item = new T();
                        item.Context = Context;
                        item.Parent = parent;
                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }

        public IEnumerable<Entity> FindReferencing(Entity e)
        {
            var sql = @"spFindReferencingEntity";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = e.Guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Entity item = new Entity();
                        item.Context = Context;
                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }

        public IEnumerable<ItemType> FindConnection<ItemType>(Entity parent, Entity to, int? referenceType)
            where ItemType : Entity, new()
        {
            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*
	FROM Entity
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
    INNER JOIN EntityReference ON EntityReference.EntityGuid = Entity.Guid
	WHERE 
        Entity.ParentGuid = @ParentGuid AND
		EntityReference.ReferencedEntityGuid = @EntityGuidTo AND
		(@ReferenceType IS NULL OR EntityReference.ReferenceType = @ReferenceType) AND
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT q.* FROM q
ORDER BY Number";

            var childrentype = new ItemType().EntityType;
            sql = String.Format(sql, childrentype);

            using (var cmd = Context.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = Context.ShowHidden;
                cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = Context.ShowDeleted;
                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = parent.Guid;
                cmd.Parameters.Add("@EntityGuidTo", SqlDbType.UniqueIdentifier).Value = to.Guid;
                cmd.Parameters.Add("@ReferenceType", SqlDbType.Int).Value = referenceType.HasValue ? (object)referenceType.Value : DBNull.Value;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ItemType item = new ItemType();
                        item.Context = Context;
                        item.Parent = parent;
                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }

        #endregion
        #region Entity Load Functions

        public IEnumerable<Entity> LoadEntities(IEnumerable<Guid> guids)
        {
            foreach (var guid in guids)
            {
                yield return LoadEntity(guid);
            }
        }

        public IEnumerable<T> LoadEntities<T>(IEnumerable<Guid> guids)
            where T : Entity, new()
        {
            foreach (var guid in guids)
            {
                yield return LoadEntity<T>(guid);
            }
        }

        /// <summary>
        /// Loads a strongly typed entity by Guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Entity LoadEntity(Guid guid)
        {
            Entity e = new Entity(Context);
            e.Guid = guid;
            e.Load();

            return LoadStronglyTypedEntity(e.EntityType, guid);
        }

        /// <summary>
        /// Loads an entity identified by its GUID.
        /// </summary>
        /// <param name="guid">GUID of the entity to be loaded.</param>
        /// <returns>The strongly typed entity.</returns>
        /// <remarks>
        /// The returned object is
        /// strongly type to the entity type and not simply an <see cref="Entity"/> class.
        /// </remarks>
        public T LoadEntity<T>(Guid guid)
            where T : Entity, new()
        {
            if (typeof(T) == typeof(Entity))
            {
                var e = new Entity(Context);
                e.Guid = guid;
                e.Load();

                return (T)LoadStronglyTypedEntity(e.EntityType, guid);
            }
            else
            {
                var e = new T();
                e.Context = Context;
                e.Guid = guid;
                e.Load();

                return e;
            }
        }

        /// <summary>
        /// Loads an entity identified by its fully qualified name.
        /// </summary>
        /// <param name="name">Fully qualified name of the entity to be loaded.</param>
        /// <returns>An entity object.</returns>
        /// <remarks>
        /// The loaded object will always have the generic type <see cref="Entity"/>.
        /// Use <b>LoadStronglyTypeEntity</b> to load into specific classes.
        /// </remarks>
        public Entity LoadEntity(string name)
        {
            var i = name.IndexOf(Constants.EntityTypeSeparator);

            if (i <= 0)
            {
                throw new ArgumentException("Name must start with entity type.");   // *** TODO
            }

            EntityType entityType;
            if (!Enum.TryParse<EntityType>(name.Substring(0, i), out entityType))
            {
                throw new ArgumentException("Invalid entity type in string");       // *** TODO
            }

            return LoadEntityByNameParts(entityType, name.Substring(i + 1).Split(Constants.EntityNameSeparator));
        }

        public T LoadEntity<T>(string name)
            where T : Entity
        {
            return (T)LoadEntity(name);
        }

        public Entity LoadEntity(EntityType entityType, params string[] nameParts)
        {
            return LoadEntity(CombineName(entityType, nameParts));
        }

        public T LoadEntity<T>(params string[] nameParts)
            where T : Entity
        {
            return (T)LoadEntity(Constants.EntityTypeMap[typeof(T)], nameParts);
        }

        private Entity LoadStronglyTypedEntity(EntityType entityType, Guid guid)
        {
            // Figure out class name from the entity type
            var classname = "Jhu.Graywulf.Registry." + entityType.ToString();

            // Create the strongly typed class and load the entity again
            var classtype = Type.GetType(classname);

            var e = (Entity)classtype.GetConstructor(new Type[] { typeof(Context) }).Invoke(new object[] { Context });
            e.Guid = guid;
            e.Load();

            return e;
        }

        private Entity LoadEntityByNameParts(EntityType entityType, string[] nameParts)
        {
            var sql = @"spFindEntity_byName";

            var npdt = new DataTable();
            npdt.Columns.Add("ID", typeof(int));
            npdt.Columns.Add("Name", typeof(string));

            for (int i = 0; i < nameParts.Length; i++)
            {
                npdt.Rows.Add(i, nameParts[i]);
            }

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@EntityType", SqlDbType.Int).Value = entityType == EntityType.Unknown ? (object)DBNull.Value : entityType;
                cmd.Parameters.Add("@NameParts", SqlDbType.Structured).Value = npdt;

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        throw Error.EntityNotFound(String.Join(Constants.EntityNameSeparator.ToString(), nameParts));
                    }

                    var e = new Entity(Context);
                    e.LoadFromDataReader(dr);

                    return LoadStronglyTypedEntity(e.EntityType, e.Guid);
                }
            }
        }

        public bool CheckEntityDuplicate(EntityType entityType, Guid entityGuid, Guid parentEntityGuid, string name)
        {
            var sql = @"spCheckEntityDuplicate";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = entityGuid == Guid.Empty ? (object)DBNull.Value : entityGuid;
                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = parentEntityGuid;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128).Value = name.Trim();
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                return (int)cmd.Parameters["RETVAL"].Value > 0;
            }
        }

        #endregion

        public void ApplyChanges(IEnumerable<Entity> update, IEnumerable<Entity> delete, IEnumerable<Entity> create)
        {
            // Write changes to the registry
            foreach (var entity in delete.OrderByDescending(e => e.Number))
            {
                entity.Delete();
            }
            foreach (var entity in create)
            {
                entity.Save();
            }
            foreach (var entity in update)
            {
                entity.Save();
            }
        }

        #region Utility function

        protected DataTable CreateGuidListTable(HashSet<Guid> guids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Guid", typeof(Guid));

            if (guids != null)
            {
                foreach (Guid guid in guids)
                {
                    dt.Rows.Add(guid);
                }
            }

            return dt;
        }

        #endregion
    }
}
