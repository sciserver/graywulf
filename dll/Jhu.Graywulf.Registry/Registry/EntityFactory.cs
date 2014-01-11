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
            public Entity[] Entities;
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

        public IEnumerable<ItemType> FindChildren<ItemType>(Entity parent)
            where ItemType : Entity, new()
        {
            var childrentype = new ItemType().EntityType;

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

        #endregion
        #region Entity Load Functions

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

        private Entity LoadEntityByNameParts(string[] nameParts)
        {
            var sql = @"spGetEntity_byNameParts";

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
            where T : Entity
        {
            Entity e = new Entity(Context);
            e.Guid = guid;
            e.Load();

            return (T)LoadStronglyTypedEntity(e.EntityType, guid);
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
            return LoadEntityByNameParts(name.Split('.'));
        }

        public Entity LoadEntity(params string[] nameParts)
        {
            var parts = new List<string>();

            for (int i = 0; i < nameParts.Length; i++)
            {
                parts.AddRange(nameParts[i].Split('.'));
            }

            return LoadEntityByNameParts(parts.ToArray());
        }

        public T LoadEntity<T>(string name)
            where T : Entity
        {
            return (T)LoadEntity(name);
        }

        public T LoadEntity<T>(params string[] nameParts)
            where T : Entity
        {
            return (T)LoadEntity(nameParts);
        }

        public bool CheckEntityDuplicate(Entity parentEntity, string name)
        {
            var sql = @"spCheckEntityDuplicate";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = Context.UserGuid;
                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = parentEntity == null ? Guid.Empty : parentEntity.Guid;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128).Value = name;
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
        public void Serialize(Entity entity, TextWriter output, HashSet<EntityType> mask)
        {
            //var entities = new List<Entity>(entity.EnumerateChildrenForSerialize(mask));

            var registry = new Registry();
            registry.Entities = entity.EnumerateChildrenForSerialize(mask).ToArray();

            var ser = new XmlSerializer(registry.GetType());
            ser.Serialize(output, registry);
        }

        /// <summary>
        /// Deserializes entities from an XML stream.
        /// </summary>
        /// <param name="input">The TextReader object used for reading from the XML stream.</param>
        /// <returns>An IEnumerable interface to the deserialized objects.</returns>
        public List<Entity> Deserialize(TextReader input)
        {
            List<Entity> entities;
            XmlSerializer ser = new XmlSerializer(typeof(List<Entity>));

            // Deserialize object into memory, they don't have GUIDs now
            entities = (List<Entity>)ser.Deserialize(input);

            int depth = 0;
            int count = 0;

            while (count < entities.Count)
            {
                foreach (var entity in entities)
                {
                    if (entity.ParentReference.IsEmpty && depth == 0 ||
                        !entity.ParentReference.IsEmpty && depth == entity.ParentReference.Name.Count(c => c == '.') + 1)
                    {
                        entity.IsDeserializing = true;
                        entity.Context = this.Context;

                        if (!entity.ParentReference.IsEmpty)
                        {
                            entity.ResolveParentReference();
                        }

                        entity.Save();

                        Console.Error.WriteLine("Created {0}", entity.Name);

                        count++;
                    }
                }

                depth++;
            }

            foreach (var entity in entities)
            {
                entity.IsDeserializing = false;     // Allows saving entity references
                entity.ResolveNameReferences();
                entity.Save();
            }

            return entities;
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
