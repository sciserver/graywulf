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
                    string key = dataset.GetObjectUniqueKey(
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
                string key = dataset.GetObjectUniqueKey(
                    Constants.DatabaseObjectTypes[typeof(T)],
                    dataset.Name,
                    databaseName,
                    schemaName,
                    objectName);

                return ContainsKey(key);
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
            }
        }
    }
}
