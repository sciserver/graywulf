using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class DataTypeReference : DatabaseObjectReference, IColumnReferences
    {
        #region Property storage variables

        private IndexedDictionary<string, ColumnReference> columnReferences;

        #endregion
        #region Properties

        public Schema.DataType DataType
        {
            get { return (Schema.DataType)DatabaseObject; }
            set { DatabaseObject = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string UniqueName
        {
            get
            {
                if (!IsSystem)
                {
                    return base.UniqueName;
                }
                else
                {
                    return base.DatabaseObjectName;
                }
            }
        }

        public IndexedDictionary<string, ColumnReference> ColumnReferences
        {
            get { return columnReferences; }
        }

        #endregion
        #region Constructors and initializers

        public DataTypeReference()
        {
            InitializeMembers();
        }

        public DataTypeReference(DataTypeReference old)
        {
            CopyMembers(old);
        }

        public DataTypeReference(Schema.DataType dataType)
            : base(dataType)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columnReferences = new IndexedDictionary<string, ColumnReference>(SchemaManager.Comparer);
        }

        private void CopyMembers(DataTypeReference old)
        {
            // Deep copy of column references
            this.columnReferences = new IndexedDictionary<string, ColumnReference>(SchemaManager.Comparer);

            foreach (var key in old.columnReferences.Keys)
            {
                var ncr = new ColumnReference(this, old.columnReferences[key]);
                this.columnReferences.Add(key, ncr);
            }
        }

        public override object Clone()
        {
            return new DataTypeReference(this);
        }

        #endregion

        public static DataTypeReference Interpret(DataTypeIdentifier di)
        {
            var schema = di.FindDescendant<SchemaName>()?.Value;
            var datatype = di.FindDescendant<DataTypeName>().Value;

            var dr = new DataTypeReference()
            {
                SchemaName = RemoveIdentifierQuotes(schema),
                DatabaseObjectName = RemoveIdentifierQuotes(datatype),
                IsUserDefined = true
            };

            return dr;
        }

        public static DataTypeReference Interpret(DataTypeSpecification dtws)
        {
            var dr = dtws.DataTypeIdentifier.DataTypeReference;

            if (dr.SchemaName == null && Schema.SqlServer.Constants.SqlDataTypes.ContainsKey(dr.DatabaseObjectName))
            {
                // System type, this needs to be resolved here to have
                // access to precision, scale and length
                var sqltype = Schema.SqlServer.Constants.SqlDataTypes[dr.DatabaseObjectName];
                var dt = Schema.DataType.Create(sqltype, dtws.Length, dtws.Precision, dtws.Scale, false);

                dr.IsUserDefined = false;
                dr.DatabaseObject = dt;
                dr.DatabaseObjectName = dt.TypeNameWithLength;  // Needs update for system types
            }
            else
            {
                dr.IsUserDefined = true;
            }

            return dr;
        }

        public override void LoadDatabaseObject(DatasetBase dataset)
        {
            // Because this is the base type only, create a copy here since
            // properties like IsNullable will be overwritten later
            var dt = (DataType)dataset.GetObject(DatabaseName, SchemaName, DatabaseObjectName);
            DatabaseObject = new DataType(dt);

            // TODO: we could figure out here if it's a scalar function or else
        }
    }
}
