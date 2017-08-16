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
        public EntityFactory(RegistryContext context)
            : base(context)
        {

        }

        #endregion

        #region Entity Search Functions

        private string GetTableHint()
        {
            if (RegistryContext.TransactionMode.HasFlag(TransactionMode.ReadWrite))
            {
                return "WITH(ROWLOCK, UPDLOCK)";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Loads all entities of a given type.
        /// </summary>
        /// <typeparam name="T">Type of the entities to load.</typeparam>
        /// <returns>An IEnumerable interface to the loaded objects.</returns>
        public IEnumerable<T> FindAll<T>()
            where T : Entity, new()
        {
            var type = Constants.EntityTypeMap[typeof(T)];

            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*, ROW_NUMBER () OVER ( ORDER BY Entity.Number ) AS rn
	FROM Entity {1}
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
	WHERE
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT q.* FROM q
WHERE rn BETWEEN @From + 1 AND @From + @Max OR @From IS NULL OR @Max IS NULL
ORDER BY rn
";

            sql = String.Format(sql, type, GetTableHint());

            var cmd = RegistryContext.CreateTextCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
            cmd.Parameters.Add("@From", SqlDbType.Int).Value = DBNull.Value;
            cmd.Parameters.Add("@Max", SqlDbType.Int).Value = DBNull.Value;

            return new EntityCommandEnumerator<T>(RegistryContext, cmd, true);
        }

        public IEnumerable<T> FindChildren<T>(Entity parent)
            where T : Entity, new()
        {
            var childrentype = Constants.EntityTypeMap[typeof(T)];

            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*
	FROM Entity {1}
	INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @Guid AND
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
)
SELECT q.* FROM q
ORDER BY Number
";

            sql = String.Format(sql, childrentype, GetTableHint());

            var cmd = RegistryContext.CreateTextCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = parent.Guid;

            return new EntityCommandEnumerator<T>(RegistryContext, cmd, true);
        }

        public IEnumerable<Entity> FindReferencing(Entity e)
        {
            // TODO: update lock hint

            var sql = @"spFindReferencingEntity";

            var cmd = RegistryContext.CreateStoredProcedureCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = e.Guid;

            return new EntityCommandEnumerator<Entity>(RegistryContext, cmd, false);
        }

        public IEnumerable<T> FindConnection<T>(Entity parent, Entity to, int? referenceType)
            where T : Entity, new()
        {
            var sql = @"
WITH q AS
(
	SELECT Entity.*, [{0}].*
	FROM Entity {1}
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

            var childrentype = Constants.EntityTypeMap[typeof(T)];
            sql = String.Format(sql, childrentype, GetTableHint());

            var cmd = RegistryContext.CreateTextCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@ShowHidden", SqlDbType.Bit).Value = RegistryContext.ShowHidden;
            cmd.Parameters.Add("@ShowDeleted", SqlDbType.Bit).Value = RegistryContext.ShowDeleted;
            cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = parent.Guid;
            cmd.Parameters.Add("@EntityGuidTo", SqlDbType.UniqueIdentifier).Value = to.Guid;
            cmd.Parameters.Add("@ReferenceType", SqlDbType.Int).Value = referenceType.HasValue ? (object)referenceType.Value : DBNull.Value;

            return new EntityCommandEnumerator<T>(RegistryContext, cmd, true);
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
            var entityType = Constants.EntityTypeMap[typeof(T)];

            foreach (var guid in guids)
            {
                yield return (T)LoadEntity(entityType, guid);
            }
        }

        public Entity LoadEntity(Guid guid)
        {
            EntityType entityType;

            var sql = @"
SELECT Entity.EntityType
FROM Entity {0}
WHERE Entity.Guid = @Guid
";

            sql = String.Format(sql, GetTableHint());

            using (var cmd = RegistryContext.CreateTextCommand(sql))
            {
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = guid;

                entityType = (EntityType)(Int32)cmd.ExecuteScalar();
            }

            return LoadEntity(entityType, guid);
        }

        private SqlCommand GetLoadEntityCommand(EntityType entityType, Guid guid)
        {
            var sql = @"
SELECT Entity.*, [{0}].*, ROW_NUMBER () OVER ( ORDER BY Entity.Number ) AS rn
FROM Entity {1}
INNER JOIN [{0}] ON [{0}].EntityGuid = Entity.Guid
WHERE Entity.Guid = @Guid
";

            sql = String.Format(sql, entityType, GetTableHint());

            var cmd = RegistryContext.CreateTextCommand(sql);
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = guid;

            return cmd;
        }

        public T LoadEntity<T>(Guid guid)
            where T : Entity, new()
        {
            if (typeof(T) == typeof(Entity))
            {
                return (T)LoadEntity(guid);
            }
            else
            {
                var entityType = Constants.EntityTypeMap[typeof(T)];
                return (T)LoadEntity(entityType, guid);
            }
        }

        private Entity LoadEntity(EntityType entityType, Guid guid)
        {
            // This is somewhat slower due to reflection

            Entity e;

            if (RegistryContext.EntityCache.TryGet(guid, out e))
            {
                return e;
            }
            else
            {
                var classtype = Constants.EntityTypeMap[entityType];
                using (var cmd = GetLoadEntityCommand(entityType, guid))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw Error.EntityNotFound(guid);
                        }

                        var entity = (Entity)Activator.CreateInstance(classtype);
                        entity.RegistryContext = RegistryContext;
                        entity.LoadFromDataReader(reader);
                        RegistryContext.EntityCache.Add(entity);
                        return entity;
                    }
                }
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

        private Entity LoadEntityByNameParts(EntityType entityType, string[] nameParts)
        {
            // TODO: add update lock hint when necessary

            Guid guid;
            Entity entity;
            var fqn = CombineName(entityType, nameParts);

            if (!RegistryContext.EntityCache.TryGet(fqn, out entity))
            {
                var sql = @"spGetEntityGuid_byName";

                var npdt = new DataTable();
                npdt.Columns.Add("ID", typeof(int));
                npdt.Columns.Add("Name", typeof(string));

                for (int i = 0; i < nameParts.Length; i++)
                {
                    npdt.Rows.Add(i, nameParts[i]);
                }

                using (var cmd = RegistryContext.CreateStoredProcedureCommand(sql))
                {
                    cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
                    cmd.Parameters.Add("@EntityType", SqlDbType.Int).Value = entityType == EntityType.Unknown ? (object)DBNull.Value : entityType;
                    cmd.Parameters.Add("@NameParts", SqlDbType.Structured).Value = npdt;

                    var res = cmd.ExecuteScalar();

                    if (res == DBNull.Value)
                    {
                        throw new EntityNotFoundException(String.Format(ExceptionMessages.EntityNotFound, fqn));
                    }
                    else
                    {
                        guid = (Guid)res;
                    }
                }

                if (!RegistryContext.EntityCache.TryGet(guid, out entity))
                {
                    entity = LoadEntity(entityType, guid);
                }

                entity.SetFullyQualifiedName(fqn);
            }

            return entity;
        }

        public bool CheckEntityDuplicate(EntityType entityType, Guid entityGuid, Guid parentEntityGuid, string name)
        {
            var sql = @"spCheckEntityDuplicate";

            using (var cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
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
