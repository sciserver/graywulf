using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    public abstract partial class DatasetBase
    {

        /// <summary>
        /// Implements a synchronized collection to cache database objects
        /// of a dataset.
        /// </summary>
        [Serializable]
        public class DatabaseObjectCollection<T> : LazyDictionary<string, T>
            where T : DatabaseObject, ICacheable, new()
        {
            /// <summary>
            /// Parent dataset
            /// </summary>
            private DatasetBase dataset;

            private bool isAllLoaded;

            public event EventHandler<AllObjectsLoadingEventArgs<string, T>> AllObjectsLoading;

            /// <summary>
            /// Gets a database objects by its three part name
            /// </summary>
            /// <param name="databaseName"></param>
            /// <param name="schemaName"></param>
            /// <param name="objectName"></param>
            /// <returns></returns>
            public T this[string databaseName, string schemaName, string objectName]
            {
                get
                {
                    string key = dataset.GetObjectKeyFromParts(
                        Constants.DatabaseObjectTypes[typeof(T)],
                        dataset.Name,
                        databaseName,
                        schemaName,
                        objectName);

                    return this[key];
                }
            }

            public bool ContainsKey(string databaseName, string schemaName, string objectName)
            {
                string key = dataset.GetObjectKeyFromParts(
                    Constants.DatabaseObjectTypes[typeof(T)],
                    dataset.Name,
                    databaseName,
                    schemaName,
                    objectName);

                return ContainsKey(key);
            }

            public bool IsAllLoaded
            {
                get { return isAllLoaded; }
            }

            /// <summary>
            /// Creates a new collection for database objects
            /// </summary>
            /// <param name="dataset"></param>
            public DatabaseObjectCollection(DatasetBase dataset)
                : base(SchemaManager.Comparer)
            {
                InitializeMembers();

                this.dataset = dataset;
            }

            /// <summary>
            /// Initializes member variable to their default values.
            /// </summary>
            private void InitializeMembers()
            {
                this.dataset = null;
                this.isAllLoaded = false;
            }

            public void LoadAll()
            {
                if (!isAllLoaded && AllObjectsLoading != null)
                {
                    AllObjectsLoadingEventArgs<string, T> e = new AllObjectsLoadingEventArgs<string, T>();

                    AllObjectsLoading(this, e);

                    if (!e.Cancel)
                    {
                        foreach (var ds in e.Collection)
                        {
                            this[ds.Key] = ds.Value;
                        }
                    }

                    this.isAllLoaded = true;
                }
            }
        }
    }
}
