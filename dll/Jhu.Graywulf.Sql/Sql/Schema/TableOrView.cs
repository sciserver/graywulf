﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Contains information about a database table
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class TableOrView : DatabaseObject, IColumns, IIndexes, ICloneable
    {
        #region Property storage members and private variables

        [NonSerialized]
        private LazyProperty<ConcurrentDictionary<string, Column>> columns;

        [NonSerialized]
        private LazyProperty<QuantityIndex> quantities;

        [NonSerialized]
        private LazyProperty<ConcurrentDictionary<string, Index>> indexes;

        // TODO: consider moving this from here
        [NonSerialized]
        private LazyProperty<TableStatistics> statistics;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the collection of columns
        /// </summary>
        // TODO: how to serialize columns?
        [IgnoreDataMember]
        public ConcurrentDictionary<string, Column> Columns
        {
            get { return columns.Value; }
            protected set { columns.Value = value; }
        }

        /// <summary>
        /// Gets or sets the quantity indexes
        /// </summary>
        [IgnoreDataMember]
        public QuantityIndex Quantities
        {
            get { return quantities.Value; }
            set { quantities.Value = value; }
        }

        /// <summary>
        /// Gets the collection of indexes
        /// </summary>
        [IgnoreDataMember]
        public ConcurrentDictionary<string, Index> Indexes
        {
            get { return indexes.Value; }
            protected set { indexes.Value = value; }
        }

        /// <summary>
        /// Gets the primary key of the table or related table of a view
        /// </summary>
        [IgnoreDataMember]
        public Index PrimaryKey
        {
            get { return Indexes.Values.FirstOrDefault(i => i.IsPrimaryKey); }
        }

        [IgnoreDataMember]
        public TableStatistics Statistics
        {
            get { return statistics.Value; }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public TableOrView()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Creates a table or view and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public TableOrView(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public TableOrView(TableOrView old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.ObjectType = DatabaseObjectType.Unknown;

            this.columns = new LazyProperty<ConcurrentDictionary<string, Column>>(LoadColumns);
            this.quantities = new LazyProperty<QuantityIndex>(LoadQuantities);
            this.indexes = new LazyProperty<ConcurrentDictionary<string, Index>>(LoadIndexes);
            this.statistics = new LazyProperty<TableStatistics>(LoadStatistics);
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(TableOrView old)
        {
            this.ObjectType = old.ObjectType;

            this.columns = new LazyProperty<ConcurrentDictionary<string, Column>>(LoadColumns);
            this.quantities = new LazyProperty<QuantityIndex>(LoadQuantities);
            this.indexes = new LazyProperty<ConcurrentDictionary<string, Index>>(LoadIndexes);
            this.statistics = new LazyProperty<TableStatistics>(LoadStatistics);
        }

        public void CopyColumns(IEnumerable<Column> columns)
        {
            this.Columns = new ConcurrentDictionary<string, Column>(SchemaManager.Comparer);

            int q = 0;
            foreach (var c in columns)
            {
                var nc = new Column(this, c)
                {
                    ID = q++
                };
                this.Columns.TryAdd(nc.Name, nc);
            }
        }

        public void CopyIndexes(IEnumerable<Index> indexes)
        {
            this.Indexes = new ConcurrentDictionary<string, Index>(SchemaManager.Comparer);

            foreach (var ix in indexes)
            {
                var ni = new Index(this, ix);
                this.Indexes.TryAdd(ix.IndexName, ix);
            }
        }

        protected QuantityIndex LoadQuantities()
        {
            return new QuantityIndex(Columns.Values);
        }


        private TableStatistics LoadStatistics()
        {
            if (Dataset != null)
            {
                return Dataset.LoadTableStatistics(this);
            }
            else
            {
                return new TableStatistics();
            }
        }

        public Index FindIndexWithFirstKey(string columnName)
        {
            foreach (var idx in Indexes.Values)
            {
                // TODO: modify this once columns are also stored by ordinal index and not just by name
                var col = idx.Columns.Values.Where(c => !c.IsIncluded).OrderBy(c => c.KeyOrdinal).FirstOrDefault();

                if (SchemaManager.Comparer.Compare(columnName, col.Name) == 0)
                {
                    return idx;
                }
            }

            return null;
        }
    }
}
