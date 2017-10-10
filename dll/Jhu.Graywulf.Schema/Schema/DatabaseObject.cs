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
    public class DatabaseObject : ICloneable, IMetadata
    {
        #region Private members

        [NonSerialized]
        private DatabaseObjectType objectType;

        [NonSerialized]
        private DatasetBase dataset;

        [NonSerialized]
        private string databaseName;

        [NonSerialized]
        private string schemaName;

        [NonSerialized]
        private string objectName;

        [NonSerialized]
        private LazyProperty<DatabaseObjectMetadata> metadata;

        #endregion
        #region Properties

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

        [IgnoreDataMember]
        public string ObjectNameWithSchema
        {
            get
            {
                return 
                    (String.IsNullOrWhiteSpace(schemaName) ? "" : (dataset.QuoteIdentifier(schemaName) + ".")) +
                    dataset.QuoteIdentifier(objectName);
            }
        }

        // TODO: how to serialize metadata?
        [IgnoreDataMember]
        public DatabaseObjectMetadata Metadata
        {
            get { return metadata.Value; }
        }

        [IgnoreDataMember]
        Metadata IMetadata.Metadata
        {
            get { return metadata.Value; }
        }

        /// <summary>
        /// Gets the name of the objects as it is diplayed by the schema browser
        /// </summary>
        [IgnoreDataMember]
        public virtual string DisplayName
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(schemaName))
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
        /// identify various types of database objects. The actual format
        /// depends on the type of the dataset, but for SQL Server it
        /// is the format of objecttype|dataset|database|schema|object
        /// </remarks>
        [IgnoreDataMember]
        public string UniqueKey
        {
            get { return dataset.GetObjectUniqueKey(this); }
            set { dataset.GetNamePartsFromObjectUniqueKey(this, value); }
        }

        /// <summary>
        /// Gets the value indicating whether the object exists in the database
        /// </summary>
        [IgnoreDataMember]
        public bool IsExisting
        {
            get { return dataset.IsObjectExisting(this); }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Creates a new object with variables set to default values.
        /// </summary>
        public DatabaseObject()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Creates a new database object with dataset set.
        /// </summary>
        /// <param name="dataset"></param>
        public DatabaseObject(DatasetBase dataset)
        {
            InitializeMembers(new StreamingContext());

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
            InitializeMembers(new StreamingContext());

            this.dataset = dataset;
            this.databaseName = databaseName;
            this.schemaName = schemaName;
            this.objectName = objectName;
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
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.objectType = DatabaseObjectType.Unknown;
            this.dataset = null;
            this.databaseName = null;
            this.schemaName = null;
            this.objectName = null;

            this.metadata = new LazyProperty<DatabaseObjectMetadata>(LoadMetadata);
        }

        /// <summary>
        /// Copies member variable from an existing object
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(DatabaseObject old)
        {
            this.objectType = old.objectType;
            this.dataset = old.dataset;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.objectName = old.objectName;

            this.metadata = new LazyProperty<DatabaseObjectMetadata>(LoadMetadata);
        }

        /// <summary>
        /// When overloaded in derived classes, returns a copy of the object
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Rename(string schemaName, string objectName)
        {
            dataset.RenameObject(this, schemaName, objectName);

            this.schemaName = schemaName;
            this.objectName = objectName;
        }

        public virtual void Create()
        {
        }

        public virtual void Drop()
        {
            dataset.DropObject(this);
        }

        public override string ToString()
        {
            return dataset.GetObjectFullyResolvedName(this);
        }

        private DatabaseObjectMetadata LoadMetadata()
        {
            if (dataset != null)
            {
                return dataset.LoadDatabaseObjectMetadata(this);
            }
            else
            {
                return new DatabaseObjectMetadata();
            }
        }

        /// <summary>
        /// Loads all columns belonging to a table or view
        /// </summary>
        /// <returns></returns>
        protected ConcurrentDictionary<string, Column> LoadColumns()
        {
            if (Dataset != null)
            {
                return new ConcurrentDictionary<string, Column>(Dataset.LoadColumns(this), SchemaManager.Comparer);
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
                return new ConcurrentDictionary<string, Index>(Dataset.LoadIndexes(this), SchemaManager.Comparer);
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
                return new ConcurrentDictionary<string, Parameter>(Dataset.LoadParameters(this), SchemaManager.Comparer);
            }
            else
            {
                return new ConcurrentDictionary<string, Parameter>(SchemaManager.Comparer);
            }
        }


    }
}
