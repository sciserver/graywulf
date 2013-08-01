using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;


namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Contains information about a generic database object
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class DatabaseObject : ICacheable, ICloneable, IDatabaseObjectName
    {
        private long cachedVersion;

        private DatabaseObjectType objectType;
        private DatasetBase dataset;

        private string databaseName;
        private string schemaName;
        private string objectName;

        private DatabaseObjectMetadata metadata;

        #region ICacheable implementation

        /// <summary>
        /// Reserved for caching logic
        /// </summary>
        [IgnoreDataMember]
        public long CachedVersion
        {
            get { return cachedVersion; }
        }

        /// <summary>
        /// Gets if the object is to be cached or discarded after use
        /// </summary>
        [IgnoreDataMember]
        public bool IsCacheable
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Database object type
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectType ObjectType
        {
            get { return objectType; }
            set { objectType = value; }
        }

        /// <summary>
        /// Gets the dataset this object belongs to.
        /// </summary>
        [DataMember]
        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        /// <summary>
        /// Gets the name of the dataset this object belongs to.
        /// </summary>
        [IgnoreDataMember]
        public string DatasetName
        {
            get { return dataset.Name; }
        }

        /// <summary>
        /// Gets or sets the name of the database this object belongs to.
        /// </summary>
        /// <remarks>
        /// This is not used in most cases, as in SkyQuery dataset is
        /// the primary entity and not databases.
        /// </remarks>
        [DataMember]
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the schema name of the object.
        /// </summary>
        [DataMember]
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the object
        /// </summary>
        [DataMember]
        public string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        public DatabaseObjectMetadata Metadata
        {
            get
            {
                if (metadata == null)
                {
                    metadata = dataset.LoadDatabaseObjectMetadata(this);
                }

                return metadata;
            }
        }

        /// <summary>
        /// Gets the name of the objects as it is diplayed by the schema browser
        /// </summary>
        [IgnoreDataMember]
        public virtual string DisplayName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(schemaName))
                {
                    return String.Format("{0}.{1}", schemaName, objectName);
                }
                else
                {
                    return String.Format("{0}", objectName);
                }
            }
        }

        /// <summary>
        /// Gets the internal object key.
        /// </summary>
        /// <remarks>
        /// This unique id is used in dictionaries and on the web page to
        /// identify various types of database objects
        /// </remarks>
        [IgnoreDataMember]
        public virtual string ObjectKey
        {
            get { return dataset.GetObjectKeyFromParts(this); }
            set { dataset.GetPartsFromObjectKey(this, value); }
        }

        /// <summary>
        /// Creates a new object with variables set to default values.
        /// </summary>
        public DatabaseObject()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a new database object with dataset set.
        /// </summary>
        /// <param name="dataset"></param>
        public DatabaseObject(DatasetBase dataset)
        {
            InitializeMembers();

            this.dataset = dataset;
        }

        /// <summary>
        /// Creates a new database object with its full name initialized
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="objectName"></param>
        protected DatabaseObject(DatasetBase dataset, string databaseName, string schemaName, string objectName)
        {
            InitializeMembers();

            this.dataset = dataset;
            this.databaseName = databaseName;
            this.schemaName = schemaName;
            this.objectName = objectName;

            this.metadata = null;
        }

        /// <summary>
        /// Creates a copy of the database object
        /// </summary>
        /// <param name="old"></param>
        protected DatabaseObject(DatabaseObject old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variable to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.cachedVersion = DateTime.Now.Ticks;
            this.objectType = DatabaseObjectType.Unknown;
            this.dataset = null;
            this.databaseName = null;
            this.schemaName = null;
            this.objectName = null;
        }

        /// <summary>
        /// Copies member variable from an existing object
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(DatabaseObject old)
        {
            this.cachedVersion = DateTime.Now.Ticks;
            this.objectType = old.objectType;
            this.dataset = old.dataset;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.objectName = old.objectName;
        }

        public virtual string GetFullyResolvedName()
        {
            return dataset.GetObjectFullyResolvedName(this);
        }

        /// <summary>
        /// Touches the object so it won't get dropped from the cache.
        /// </summary>
        public void Touch()
        {
            cachedVersion = DateTime.Now.Ticks;
        }

        public void Rename(string objectName)
        {
            dataset.RenameObject(this, objectName);

            this.objectName = objectName;
        }

        public void Drop()
        {
            dataset.DropObject(this);
        }

        public override string ToString()
        {
            return GetFullyResolvedName();
        }

        #region ICloneable Members

        /// <summary>
        /// When overloaded in derived classes, returns a copy of the object
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Loads all columns belonging to a table or view
        /// </summary>
        /// <returns></returns>
        protected ConcurrentDictionary<string, Column> LoadColumns()
        {
            if (Dataset != null)
            {
                return Dataset.LoadColumns(this);
            }
            else
            {
                return new ConcurrentDictionary<string, Column>(SchemaManager.Comparer);
            }
        }

        /// <summary>
        /// Loads indexes of this table or view from the dataset
        /// </summary>
        /// <returns></returns>
        protected ConcurrentDictionary<string, Index> LoadIndexes()
        {
            if (Dataset != null)
            {
                return Dataset.LoadIndexes(this);
            }
            else
            {
                return new ConcurrentDictionary<string, Index>(SchemaManager.Comparer);
            }
        }

        /// <summary>
        /// Loads all parameters belonging to a function or stored procedure
        /// </summary>
        /// <returns></returns>
        protected ConcurrentDictionary<string, Parameter> LoadParameters()
        {
            if (Dataset != null)
            {
                return Dataset.LoadParameters(this);
            }
            else
            {
                return new ConcurrentDictionary<string, Parameter>(SchemaManager.Comparer);
            }
        }
    }
}
