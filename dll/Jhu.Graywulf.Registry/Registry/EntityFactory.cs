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
        [XmlRoot(ElementName = "Registry", Namespace = "")]
        [XmlType(TypeName = "Registry", Namespace = "")]
        public class Registry
        {
            [XmlArrayItem(typeof(Cluster))]
            [XmlArrayItem(typeof(DatabaseDefinition))]
            [XmlArrayItem(typeof(DatabaseInstance))]
            [XmlArrayItem(typeof(DatabaseInstanceFile))]
            [XmlArrayItem(typeof(DatabaseInstanceFileGroup))]
            [XmlArrayItem(typeof(DatabaseVersion))]
            [XmlArrayItem(typeof(DeploymentPackage))]
            [XmlArrayItem(typeof(DiskVolume))]
            [XmlArrayItem(typeof(Domain))]
            [XmlArrayItem(typeof(Federation))]
            [XmlArrayItem(typeof(FileGroup))]
            [XmlArrayItem(typeof(JobDefinition))]
            [XmlArrayItem(typeof(JobInstance))]
            [XmlArrayItem(typeof(JobInstanceDependency))]
            [XmlArrayItem(typeof(Machine))]
            [XmlArrayItem(typeof(MachineRole))]
            [XmlArrayItem(typeof(Partition))]
            [XmlArrayItem(typeof(QueueDefinition))]
            [XmlArrayItem(typeof(QueueInstance))]
            [XmlArrayItem(typeof(ServerInstance))]
            [XmlArrayItem(typeof(ServerVersion))]
            [XmlArrayItem(typeof(Slice))]
            [XmlArrayItem(typeof(User))]
            [XmlArrayItem(typeof(UserDatabaseInstance))]
            [XmlArrayItem(typeof(UserGroup))]
            [XmlArrayItem(typeof(UserGroupMembership))]
            [XmlArrayItem(typeof(UserRole))]
            [XmlArrayItem(typeof(UserRoleMembership))]
            [XmlArrayItem(typeof(UserIdentity))]
            public Entity[] Entities;
        }

        public static string CombineName(EntityType entityType, params string[] nameParts)
        {
            var name = entityType.ToString() + ":";

            for (int i = 0; i < nameParts.Length; i++)
            {
                if (i > 0)
                {
                    name += ".";
                }

                var idx = nameParts[i].IndexOf(':');

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
            var idx = parentName.IndexOf(':');

            return entityType.ToString() + ":" + parentName.Substring(idx + 1) + "." + name;
        }

        public static string GetName(string name)
        {
            var idx = name.LastIndexOf('.');

            if (idx > -1)
            {
                return name.Substring(idx + 1);
            }
            else
            {
                idx = name.IndexOf(':');
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
            var i = name.IndexOf(':');

            if (i <= 0)
            {
                throw new ArgumentException("Name must start with entity type.");   // *** TODO
            }

            EntityType entityType;
            if (!Enum.TryParse<EntityType>(name.Substring(0, i), out entityType))
            {
                throw new ArgumentException("Invalid entity type in string");       // *** TODO
            }

            return LoadEntityByNameParts(entityType, name.Substring(i + 1).Split('.'));
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
                    dr.Read();
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
                cmd.Parameters.Add("@EntityType", SqlDbType.Int).Value = entityType;
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

        #region Serialization Functions

        /// <summary>
        /// Serializes an entity and all its child elements into XML.
        /// </summary>
        /// <param name="entity">The root entity of the serialization.</param>
        /// <param name="output">The TextWriter object used for writing the XML stream.</param>
        public void Serialize(Entity entity, TextWriter output, EntityGroup entityGroupMask, bool excludeUserCreated)
        {
            var registry = new Registry();
            registry.Entities = EnumerateChildrenForSerialize(entity, entityGroupMask, excludeUserCreated).ToArray();

            var ser = new XmlSerializer(registry.GetType());
            ser.Serialize(output, registry);
        }

        /// <summary>
        /// Recursively enumerate entities in the registry tree
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="excludeEntities"></param>
        /// <param name="excludeUserJobs"></param>
        /// <returns></returns>
        /// <remarks>
        /// The function enumerates nodes of a subtree of the registry,
        /// returning parent items first, then immediate children in their
        /// appropriate order by the field 'Number'.
        /// Certain entities are excluded but their children are still
        /// included in the search!
        /// </remarks>
        private IEnumerable<Entity> EnumerateChildrenForSerialize(Entity entity, EntityGroup entityGroupMask, bool excludeUserJobs)
        {
            /*
            // Make sure it's not a simple user job
            // TODO: exclude user jobs is an ad-hoc solution
            // maybe filtering on name prefixes?
            if (excludeUserJobs &&
                entity.EntityType == EntityType.JobInstance &&
                (((JobInstance)entity).ScheduleType != ScheduleType.Recurring ||
                 ((JobInstance)entity).JobExecutionStatus != JobExecutionState.Scheduled))
            {
                yield break;
            }
            */

            // See if this particular type of entity is included by the mask
            if ((entity.EntityGroup & entityGroupMask) != 0)
            {
                yield return entity;
            }

            // Even if it's excluded, return children.
            // Some exports (layout) might require exporting certain security
            // objects (user-mydb mapping)
            entity.LoadAllChildren(true);
            foreach (Entity e in entity.EnumerateAllChildren().OrderBy(ei => ei.Number))
            {
                foreach (Entity ee in EnumerateChildrenForSerialize(e, entityGroupMask, excludeUserJobs))
                {
                    yield return ee;
                }
            }
        }

        /// <summary>
        /// Deserializes entities from an XML stream.
        /// </summary>
        /// <param name="input">The TextReader object used for reading from the XML stream.</param>
        /// <returns>An IEnumerable interface to the deserialized objects.</returns>
        public IEnumerable<Entity> Deserialize(TextReader input, bool ignoreDuplicates)
        {
            Registry registry;
            var ser = new XmlSerializer(typeof(Registry));

            // Deserialize object into memory, they don't have GUIDs now
            registry = (Registry)ser.Deserialize(input);

            int depth = 0;
            int count = 0;

            while (count < registry.Entities.Length)
            {
                foreach (var entity in registry.Entities)
                {
                    if (entity.ParentReference.IsEmpty && depth == 0 ||
                        !entity.ParentReference.IsEmpty && depth == entity.ParentReference.Name.Count(c => c == '.') + 1)
                    {
                        Console.Error.Write("Deserializing {0}... ", entity.Name);

                        entity.IsDeserializing = true;
                        entity.Context = this.Context;

                        if (!entity.ParentReference.IsEmpty)
                        {
                            ResolveParentReference(entity);
                        }

                        try
                        {
                            entity.Save();
                            Console.Error.WriteLine("done.");
                        }
                        catch (DuplicateNameException)
                        {
                            if (!ignoreDuplicates)
                            {
                                throw;
                            }
                            Console.Error.WriteLine("ignored duplicate.");
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine("failed.");
                            throw;
                        }

                        count++;
                    }
                }

                depth++;
            }

            foreach (var entity in registry.Entities)
            {
                Console.Error.Write("Resolving references of {0}... ", entity.Name);

                try
                {
                    entity.IsDeserializing = false;     // Allows saving entity references
                    ResolveNameReferences(entity);
                    entity.Save();

                    Console.Error.WriteLine("done.");
                }
                catch (DuplicateNameException)
                {
                    if (!ignoreDuplicates)
                    {
                        throw;
                    }
                    Console.Error.WriteLine("ignored duplicate.");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("failed.");
                    throw;
                }
            }

            return registry.Entities;
        }

        private void ResolveParentReference(Entity entity)
        {
            entity.ParentReference.EnsureEntityLoaded();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This is used by the XML deserializer
        /// </remarks>
        private void ResolveNameReferences(Entity entity)
        {
            foreach (IEntityReference r in entity.EntityReferences.Values)
            {
                if (!r.IsEmpty)
                {
                    // Make sure entity reference is loaded by retrieving its value
                    var o = r.Value;
                }
            }

            entity.IsEntityReferencesLoaded = true;
        }

        #endregion
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
