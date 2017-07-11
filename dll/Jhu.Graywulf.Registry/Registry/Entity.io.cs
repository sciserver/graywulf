using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity : ContextObject
    {
        #region Generic entity IO helper

        private static readonly Components.LazyDictionary<Type, DBHelper> DBHelpers;

        static Entity()
        {
            DBHelpers = new Components.LazyDictionary<Type, DBHelper>();
            DBHelpers.ItemLoading += delegate (object sender, Components.LazyItemLoadingEventArgs<Type, DBHelper> e)
            {
                e.Value = new DBHelper(e.Key);
                e.IsFound = true;
            };
        }

        #endregion
        #region Database IO Functions

        private void DispatchException(SqlException ex)
        {
            if (ex.Number == 51000)
            {
                throw new DuplicateNameException();
            }
            else
            {
                throw ex;
            }
        }

        /// <summary>
        /// Loads the entity from a <b>SqlDataReader</b> object.
        /// </summary>
        /// <param name="dr">The data reader to read from.</param>
        /// <returns>Returns the number of columns read.</returns>
        /// <remarks>
        /// Always reads at the current cursor position, doesn't calls the <b>Read</b> function
        /// on the <b>SqlDataReader</b> object. Reads data columns by their ordinal position in
        /// the query and not by their names.
        /// </remarks>
        internal bool LoadFromDataReader(SqlDataReader dr)
        {
            int o = -1;

            this.guid = dr.GetGuid(++o);
            this.concurrencyVersion = BitConverter.ToInt64(dr.GetSqlBytes(++o).Value, 0);
            this.parentReference.Guid = dr.GetGuid(++o);
            this.EntityType = (EntityType)dr.GetInt32(++o);
            this.number = dr.GetInt32(++o);
            this.name = dr.GetString(++o);
            this.displayName = dr.GetString(++o);
            this.version = dr.GetString(++o);
            this.system = dr.GetBoolean(++o);
            this.hidden = dr.GetBoolean(++o);
            this.readOnly = dr.GetBoolean(++o);
            this.primary = dr.GetBoolean(++o);
            this.deleted = dr.GetBoolean(++o);
            this.lockOwner = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.runningState = (RunningState)dr.GetInt32(++o);
            this.alertState = dr.GetInt32(++o);
            this.deploymentState = (DeploymentState)dr.GetInt32(++o);
            this.userGuidOwner = dr.GetGuid(++o);
            this.dateCreated = dr.GetDateTime(++o);
            this.userGuidCreated = dr.GetGuid(++o);
            this.dateModified = dr.GetDateTime(++o);
            this.userGuidModified = dr.GetGuid(++o);
            this.dateDeleted = dr.IsDBNull(++o) ? DateTime.MinValue : dr.GetDateTime(o);
            this.userGuidDeleted = dr.IsDBNull(++o) ? Guid.Empty : dr.GetGuid(o);
            this.settings.LoadFromXml(dr.IsDBNull(++o) ? null : dr.GetString(o));
            this.comments = dr.GetString(++o);

            // In case of generic search, no more fields loaded
            if (dr.FieldCount > o + 2)
            {
                DBHelpers[this.GetType()].LoadFromDataReader(this, dr, ++o);
            }

            isExisting = true;

            OnLoaded();

            return true;
        }

        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Loads the entity from the database.
        /// </summary>
        /// <remarks>
        /// The value of the <see cref="Guid" /> property and the object context must be set.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        public void Load()
        {
            var sql = DBHelpers[this.GetType()].SelectQuery;

            using (var cmd = RegistryContext.CreateTextCommand(sql))
            {
                AppendBasicParameters(cmd);

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        throw Error.EntityNotFound(guid);
                    }

                    LoadFromDataReader(dr);
                }
            }
        }

        private void LoadEntityReferences()
        {
            if (Guid != Guid.Empty && EntityType != EntityType.Unknown && entityReferences.Count > 0)
            {
                var sql = "spFindEntityReference";

                using (var cmd = RegistryContext.CreateStoredProcedureCommand(sql))
                {
                    AppendBasicParameters(cmd);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var type = dr.GetByte(1);
                            var guid = dr.GetGuid(2);

                            entityReferences[type].Guid = guid;
                        }
                    }
                }

                isEntityReferencesLoaded = true;
            }
        }

        /// <summary>
        /// Saves the entity to the database.
        /// </summary>
        /// <remarks>
        /// This function attempts to save the entity but throws and exception if
        /// a concurrency issue is detected.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <exception cref="InvalidConcurrencyVersionException">
        /// Thrown when someone else modified the entity since it was loaded from the database.
        /// </exception>
        public void Save()
        {
            Save(false);
        }

        /// <summary>
        /// Saves the entity to the database.
        /// </summary>
        /// <remarks>
        /// This function attempts to save the entity but throws and exception if
        /// the value of the <b>forceOverwrite</b> parameter is false and a concurrency issue is detected.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <param name="forceOverwrite">Determines if concurrency issues are ignored.</param>
        /// <exception cref="InvalidConcurrencyVersionException">
        /// Thrown when someone else modified the entity since it was loaded from the database.
        /// </exception>
        public void Save(bool forceOverwrite)
        {
            // Check entity duplicate
            var ef = new EntityFactory(RegistryContext);

            var parentGuid = this.parentReference.IsEmpty ? Guid.Empty : this.parentReference.Guid;

            if (!IsExisting)
            {
                Create();
            }
            else
            {
                Modify(forceOverwrite);
            }
        }

        /// <summary>
        /// Saves the newly created entity to the database.
        /// </summary>
        /// <remarks>
        /// This function is called by the <see cref="Save()"/> function of the <see cref="Entity" /> class
        /// through the <see cref="Create"/> function in the derived classes.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        protected virtual void Create()
        {
            // Generate a new Guid for the entity
            guid = Guid.NewGuid();

            // --- Create record in the Entities table ---

            string sql = "spCreateEntity";

            using (var cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = parentReference.IsEmpty ? Guid.Empty : parentReference.Guid;
                cmd.Parameters.Add("@EntityType", SqlDbType.Int).Value = (int)EntityType;   // always use property
                cmd.Parameters.Add("@Number", SqlDbType.Int).Direction = ParameterDirection.Output;

                AppendEntityCreateModifyParameters(cmd);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    DispatchException(ex);
                }

                // Read return values
                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
                this.number = (int)cmd.Parameters["@Number"].Value;
            }

            using (var cmd = DBHelpers[this.GetType()].CreateInsertCommand(this))
            {
                AppendBasicParameters(cmd);
                cmd.ExecuteNonQuery();
            }

            isExisting = true;

            LogDebug();
        }

        /// <summary>
        /// Modifies the already existing entity in the database.
        /// </summary>
        /// <remarks>
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <param name="forceOverwrite">Determines if concurrency issues are ignored.</param>
        /// <exception cref="InvalidConcurrencyVersionException">
        /// Thrown when someone else modified the entity since it was loaded from the database.
        /// </exception>
        protected virtual void Modify(bool forceOverwrite)
        {
            if (!forceOverwrite) CheckConcurrency();

            // --- Modify record in the Entities table ---
            this.dateModified = DateTime.Now;

            string sql = "spModifyEntity";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                AppendEntityCreateModifyParameters(cmd);

                cmd.ExecuteNonQuery();

                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
            }

            // --- Modify record in the type specific table

            if (DBHelpers[this.GetType()].HasColumns)
            {
                using (var cmd = DBHelpers[this.GetType()].CreateUpdateCommand(this))
                {
                    AppendBasicParameters(cmd);
                    cmd.ExecuteNonQuery();
                }
            }

            LogDebug();
        }

        /// <summary>
        /// Appends basic parameters to stored procedure commands
        /// </summary>
        /// <param name="cmd">The <b>SqlCommand</b> to append the parameters to.</param>
        protected void AppendBasicParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = RegistryContext.UserReference.Guid;
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = guid;
        }

        /// <summary>
        /// Appends required parameters to create/modify stored procedure commands for entities (general).
        /// </summary>
        /// <param name="cmd">The <b>SqlCommand</b> to append the parameters to.</param>
        private void AppendEntityCreateModifyParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128).Value = name;
            cmd.Parameters.Add("@DisplayName", SqlDbType.NVarChar, 128).Value = displayName;
            cmd.Parameters.Add("@Version", SqlDbType.NVarChar, 25).Value = version;
            cmd.Parameters.Add("@RunningState", SqlDbType.Int).Value = runningState;
            cmd.Parameters.Add("@AlertState", SqlDbType.Int).Value = alertState;
            cmd.Parameters.Add("@DeploymentState", SqlDbType.Int).Value = deploymentState;
            cmd.Parameters.Add("@DateCreated", SqlDbType.DateTime).Value = dateCreated;
            cmd.Parameters.Add("@DateModified", SqlDbType.DateTime).Value = dateModified;
            cmd.Parameters.Add("@Settings", SqlDbType.NVarChar).Value = (object)settings.SaveToXml() ?? DBNull.Value;
            cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = comments ?? String.Empty;

            // Process entity feferences

            var refdt = new DataTable();
            refdt.Columns.Add("EntityGuid", typeof(Guid));
            refdt.Columns.Add("ReferenceType", typeof(int));
            refdt.Columns.Add("ReferencedEntityGuid", typeof(Guid));

            // We cannot save references when deserializing from xml because
            // the references might not be there yet
            if ((!IsExisting || isEntityReferencesLoaded) && !isDeserializing)
            {
                foreach (var er in entityReferences.Values)
                {
                    var referencedGuid = er.Guid;

                    refdt.Rows.Add(this.guid, er.ReferenceType, referencedGuid);
                }
            }

            cmd.Parameters.Add("@EntityReferences", SqlDbType.Structured).Value = refdt;

        }

        /// <summary>
        /// Flags an already existing entity in the database as deleted.
        /// </summary>
        /// <remarks>
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <exception cref="InvalidConcurrencyVersionException">
        /// Thrown when someone else modified the entity since it was loaded from the database.
        /// </exception>
        public void Delete()
        {
            Delete(false);
        }

        /// <summary>
        /// Flags an already existing entity in the database as deleted.
        /// </summary>
        /// <remarks>
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <param name="forceOverwrite">Determines if concurrency issues are ignored.</param>
        /// <exception cref="InvalidConcurrencyVersionException">
        /// Thrown when someone else modified the entity since it was loaded from the database.
        /// </exception>
        public void Delete(bool forceOverwrite)
        {
            try
            {
                DeleteRecursively(forceOverwrite);
            }
            catch (LockingCollisionException)
            {
                //Context.RollbackTransaction();
                throw;
            }
            catch (InvalidConcurrencyVersionException)
            {
                //Context.RollbackTransaction();
                throw;
            }
        }

        private void DeleteRecursively(bool forceOverwrite)
        {
            LoadAllChildren(true);

            // Delete should go in reverse order in order to prevent
            // concurrency version exceptions
            foreach (Entity e in EnumerateAllChildren().Reverse<Entity>()) e.DeleteRecursively(forceOverwrite);

            if (!forceOverwrite) CheckConcurrency();

            string sql = "spDeleteEntity";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
                this.deleted = true;
                this.dateDeleted = DateTime.Now;
                this.userGuidDeleted = RegistryContext.UserReference.Guid;
            }

            LogDebug();
        }

        /// <summary>
        /// Check whether the entity was modified in the database since it was loaded.
        /// </summary>
        /// <remarks>
        /// Refer to the Developer's Guide about the optimistic concurrency model.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <exception cref="InvalidConcurrencyVersionException">Thrown when a potential concurrency issue is detected.</exception>
        /// <exception cref="LockingCollisionException">TODO</exception>
        protected void CheckConcurrency()
        {
            // TODO: implement locking for move operations (don't allow move if any locked)
            string sql = "spCheckEntityConcurrency";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@LockOwner", SqlDbType.UniqueIdentifier).Value = RegistryContext.JobReference.Guid;
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Value = BitConverter.GetBytes(this.concurrencyVersion);
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                int retval = (int)cmd.Parameters["RETVAL"].Value;

                if (retval < 0)
                {
                    switch (retval)
                    {
                        case -1:
                            LogDebug(String.Format("Invalid concurrency version of {0}: {1}", Constants.EntityNames_Singular[this.EntityType], this.GetFullyQualifiedName()));
                            throw new InvalidConcurrencyVersionException();
                        case -2:
                            LogDebug(String.Format("Locking collision on {0}: {1}", Constants.EntityNames_Singular[this.EntityType], this.GetFullyQualifiedName()));
                            throw new LockingCollisionException();
                    }
                }
            }
        }

        public void Show()
        {
            string sql = "spShowEntity";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
                this.hidden = false;
            }
        }

        public void Hide()
        {
            string sql = "spHideEntity";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
                this.hidden = true;
            }
        }

        /// <summary>
        /// Moves the entity up among its siblings.
        /// </summary>
        /// <remarks>
        /// Moves the entity up among its siblings by decreasing its <see cref="Number" /> property.
        /// All siblings are reorganized to keep the values of <b>Numbers</b> contigous.
        /// </remarks>
        public void Move(EntityMoveDirection direction)
        {
            string sql;
            if (direction == EntityMoveDirection.Up)
            {
                sql = "spMoveUpEntity";
            }
            else
            {
                sql = "spMoveDownEntity";
            }

            CheckConcurrency();

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                var retval = (int)cmd.Parameters["RETVAL"].Value;

                if (retval >= 0)
                    this.number = retval;
            }

            LogDebug();
        }

        private string[] LoadAscendantNames()
        {
            var res = new List<string>();
            var sql = "spFindEntityAscendants";

            using (var cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = guid;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        res.Add(dr.GetString(2));
                    }
                }
            }

            return res.ToArray();
        }

        public virtual Entity Copy(Entity parent, bool prefixName)
        {
            return null;
        }

        protected virtual Entity CreateCopy(Entity parent, bool prefixName)
        {
            this.LoadEntityReferences();

            var e = (Entity)this.Clone();

            e.Guid = Guid.Empty;

            if (prefixName)
            {
                e.Name = string.Format("Copy_of_{0}", this.Name);  // TODO add resource for format string?
            }

            if (parent != null)
            {
                e.Parent = parent;
            }
            else
            {
                e.Parent = this.Parent;
            }

            return e;
        }

        /// <summary>
        /// Obtains a lock for the <b>Entity</b>.
        /// </summary>
        /// <remarks>
        /// The <see cref="RegistryContext.JobGuid" /> property of the context must be set.
        /// </remarks>
        /// <exception cref="InvalidConcurrencyVersionException"></exception>
        /// <exception cref="LockingCollisionException"></exception>
        public void ObtainLock()
        {
            try
            {
                ObtainLockRecursively();
            }
            catch (LockingCollisionException)
            {
                RegistryContext.RollbackTransaction();
                throw;
            }
            catch (InvalidConcurrencyVersionException)
            {
                RegistryContext.RollbackTransaction();
                throw;
            }
        }

        private void ObtainLockRecursively()
        {
            LoadAllChildren(false);
            foreach (Entity e in EnumerateAllChildren()) e.ObtainLockRecursively();

            CheckConcurrency();

            string sql = "spObtainEntityLock";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@LockOwner", SqlDbType.UniqueIdentifier).Value = RegistryContext.JobReference.Guid;
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                this.lockOwner = RegistryContext.ContextGuid;
                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
            }

            LogDebug();
        }

        /// <summary>
        /// Releases the lock on the <b>Entity</b>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Jhu.Graywulf.Registry.RegistryContext.JobGuid" /> property of the context must be set.
        /// </remarks>
        /// <exception cref="InvalidConcurrencyVersionException"></exception>
        /// <exception cref="LockingCollisionException"></exception>
        public void ReleaseLock(bool forceRelease)
        {
            try
            {
                ReleaseLockRecursively(forceRelease);
            }
            catch (LockingCollisionException)
            {
                RegistryContext.RollbackTransaction();
                throw;
            }
            catch (InvalidConcurrencyVersionException)
            {
                RegistryContext.RollbackTransaction();
                throw;
            }
        }

        private void ReleaseLockRecursively(bool forceRelease)
        {
            LoadAllChildren(false);
            foreach (Entity e in EnumerateAllChildren()) e.ReleaseLockRecursively(forceRelease);

            if (!forceRelease)
            {
                CheckConcurrency();
            }

            string sql = "spReleaseEntityLock";

            using (SqlCommand cmd = RegistryContext.CreateStoredProcedureCommand(sql))
            {
                AppendBasicParameters(cmd);
                cmd.Parameters.Add("@LockOwner", SqlDbType.UniqueIdentifier).Value = RegistryContext.ContextGuid;
                cmd.Parameters.Add("@ConcurrencyVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                int retval = (int)cmd.Parameters["RETVAL"].Value;

                this.lockOwner = Guid.Empty;
                this.concurrencyVersion = BitConverter.ToInt64((byte[])cmd.Parameters["@ConcurrencyVersion"].Value, 0);
            }

            LogDebug();
        }

        #endregion
        #region Navigation Functions

        public void LoadChildren(EntityType entityType, bool forceReload)
        {
            var gm = this.GetType().GetMethod("LoadChildren", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new[] { typeof(bool) }, null);
            gm = gm.MakeGenericMethod(Constants.EntityTypeMap[entityType]);
            gm.Invoke(this, new object[] { forceReload });
        }

        public void LoadChildren<T>(bool forceReload)
            where T : Entity, new()
        {
            var children = (Dictionary<string, T>)childEntities[Constants.EntityTypeMap[typeof(T)]];

            var reload = children == null || forceReload;

            if (children == null)
            {
                children = CreateDictionary<T>();
                childEntities[Constants.EntityTypeMap[typeof(T)]] = children;
            }

            if (forceReload)
            {
                children.Clear();
            }

            if (IsExisting && reload)
            {
                var f = new EntityFactory(RegistryContext);
                foreach (var entity in f.FindChildren<T>(this))
                {
                    children.Add(entity.name, entity);
                }
            }
        }

        private Dictionary<string, T> CreateDictionary<T>()
        {
            return new Dictionary<string, T>(Entity.StringComparer);
        }

        /// <summary>
        /// Loads child entities of an entity according to the entity hierarchy model.
        /// </summary>
        /// <remarks>
        /// Call this function on entities before accessing the <see cref="EnumerateAllChildren"/> function, since
        /// that property does not do lazy loading.
        /// The function is only implemented in derived classes.
        /// The <see cref="RegistryContext" /> property must have a value of a valid object context with open database connection.
        /// </remarks>
        /// <param name="forceReload">If true, reloads previously loaded entities too.</param>
        public void LoadAllChildren(bool forceReload)
        {
            foreach (EntityType t in childEntities.Keys.ToArray())
            {
                LoadChildren(t, forceReload);
            }
        }

        /// <summary>
        /// Loads child entities.
        /// </summary>
        public void LoadAllChildren()
        {
            LoadAllChildren(false);
        }

        #endregion
    }
}
