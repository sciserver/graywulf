using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    public partial class SchemaManager
    {

        /// <summary>
        /// Implements a synchronized collection to cache database objects
        /// of a dataset.
        /// </summary>
        public class DatasetCollection : CachedLazyDictionary<string, DatasetBase>
        {
            /// <summary>
            /// Parent schema manager
            /// </summary>
            private SchemaManager schemaManager;

            public event EventHandler<AllObjectsLoadingEventArgs<string, DatasetBase>> AllObjectsLoading;

            /// <summary>
            /// Creates a new collection for dataset objects
            /// </summary>
            /// <param name="dataset"></param>
            public DatasetCollection(SchemaManager schemaManager, Cache<string, DatasetBase> datasetCache)
                :base(datasetCache)
            {
                InitializeMembers();

                this.schemaManager = schemaManager;
            }

            /// <summary>
            /// Initializes member variable to their default values.
            /// </summary>
            private void InitializeMembers()
            {
                this.schemaManager = null;
            }

            public void LoadAll()
            {
                if (AllObjectsLoading != null)
                {
                    AllObjectsLoadingEventArgs<string, DatasetBase> e = new AllObjectsLoadingEventArgs<string, DatasetBase>();

                    AllObjectsLoading(this, e);

                    if (!e.Cancel)
                    {
                        foreach (var ds in e.Collection)
                        {
                            this[ds.Key] = ds.Value;
                        }
                    }
                }
            }
        }
    }
}
