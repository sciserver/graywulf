using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Implements basic functionality to handle database schemas.
    /// </summary>
    /// <remarks>
    /// The class provides cache functionality to prevent frequent
    /// schema read requests slow down the system.
    /// 
    /// Two levels of caching is implemented to support queries that
    /// are executed in a user context accessing only rarely used 
    /// datasets and a global cache for datasets accessed frequently.
    /// Later are, e.g., the datasets configured in the graywulf registry.
    /// 
    /// As parallel query processing accesses the local cache concurrently,
    /// thread safety must be guaranteed for instance functions as well.
    /// </remarks>
    public partial class SchemaManager
    {
        #region Static members

        /// <summary>
        /// This comparer can be changed to provide case sensitivity
        /// </summary>
        public static StringComparer Comparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }

        /// <summary>
        /// Used for caching schema description between SchemaManager instances
        /// </summary>
        /// <remarks>
        /// This collection is handled by the DatasetCollection class
        /// </remarks>
        protected static readonly Cache<string, DatasetBase> datasetCache;

        /// <summary>
        /// Static constructor that initializes the global cache
        /// </summary>
        static SchemaManager()
        {
            datasetCache = new Cache<string, DatasetBase>(Comparer);
        }

        /// <summary>
        /// Clears the schema cache.
        /// </summary>
        public static void ClearCache()
        {
            datasetCache.Clear();
        }

        #endregion


        /// <summary>
        /// Provides access to the datasets managable by the schema manager.
        /// </summary>
        /// <remarks>
        /// Caches are managed through this specialized collection class.
        /// </remarks>
        protected DatasetCollection datasets;

        /// <summary>
        /// Gets the collection of datasets
        /// </summary>
        /// <remarks>
        /// Through this property, the schema manager provides lazy-loading
        /// and caching of dataset schemas.
        /// </remarks>
        public DatasetCollection Datasets
        {
            get { return datasets; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SchemaManager()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializes local variables
        /// </summary>
        private void InitializeMembers()
        {
            this.datasets = new DatasetCollection(this, datasetCache);

            this.datasets.ItemAdded += OnDatasetAdded;
            this.datasets.ItemLoading += OnDatasetLoading;
            this.datasets.AllObjectsLoading += OnAllDatasetsLoading;
        }

        /// <summary>
        /// This function is called when a new dataset is automatically loaded
        /// or added to the collection by hand
        /// </summary>
        /// <param name="ds"></param>
        private void OnDatasetAdded(object sender, LazyItemAddedEventArgs<string, DatasetBase> e)
        {
            OnDatasetAdded(e.Key, e.Value);
        }

        protected virtual void OnDatasetAdded(string datasetName, DatasetBase ds)
        {
        }

        /// <summary>
        /// When implemented in a derived class, loads a dataset from
        /// a store specific to that schema manager.
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        /// <remarks>
        /// The default schema manager does not support loading datasets
        /// on the fly, only using datasets that have been added manually.
        /// </remarks>
        private void OnDatasetLoading(object sender, LazyItemLoadingEventArgs<string, DatasetBase> e)
        {
            try
            {
                e.Value = LoadDataset(e.Key);
                e.IsFound = true;
            }
            catch (Exception)
            {
                e.IsFound = false;
            }
        }

        protected virtual DatasetBase LoadDataset(string datasetName)
        {
            throw new NotImplementedException();
        }

        private void OnAllDatasetsLoading(object sender, AllObjectsLoadingEventArgs<string, DatasetBase> e)
        {
            e.Collection = LoadAllDatasets();
            e.Cancel = false;
        }

        /// <summary>
        /// Loads all dataset descriptions at once
        /// </summary>
        /// <remarks>
        /// Used by the schema browser.
        /// </remarks>
        protected virtual IEnumerable<KeyValuePair<string, DatasetBase>> LoadAllDatasets()
        {
            yield break;
        }

        /// <summary>
        /// Returns a database object identified by its composite string id.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DatabaseObject GetDatabaseObjectByKey(string key)
        {
            string[] parts = key.Split('|');

            DatabaseObjectType type = GetDatabaseObjectTypeFromKey(key);

            switch (type)
            {
                case DatabaseObjectType.Table:
                    return this.Datasets[parts[1]].Tables[parts[2], parts[3], parts[4]];
                case DatabaseObjectType.View:
                    return this.Datasets[parts[1]].Views[parts[2], parts[3], parts[4]];
                case DatabaseObjectType.TableValuedFunction:
                case DatabaseObjectType.SqlTableValuedFunction:
                case DatabaseObjectType.ClrTableValuedFunction:
                    return this.Datasets[parts[1]].TableValuedFunctions[parts[2], parts[3], parts[4]];
                case DatabaseObjectType.ScalarFunction:
                case DatabaseObjectType.SqlScalarFunction:
                case DatabaseObjectType.ClrScalarFunction:
                    return this.Datasets[parts[1]].ScalarFunctions[parts[2], parts[3], parts[4]];
                case DatabaseObjectType.StoredProcedure:
                case DatabaseObjectType.SqlStoredProcedure:
                case DatabaseObjectType.ClrStoredProcedure:
                    return this.Datasets[parts[1]].StoredProcedures[parts[2], parts[3], parts[4]];
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the type of a schema object identified by its composite
        /// string id.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DatabaseObjectType GetDatabaseObjectTypeFromKey(string key)
        {
            int i = key.IndexOf('|');

            if (i < 0)
            {
                i = key.Length;
            }

            DatabaseObjectType type;

            if (Enum.TryParse(key.Substring(0, i), out type))
            {
                return type;
            }

            throw new NotImplementedException();
        }

    }
}
