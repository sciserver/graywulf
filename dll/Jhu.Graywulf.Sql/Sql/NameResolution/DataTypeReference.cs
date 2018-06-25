using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class DataTypeReference : DatabaseObjectReference
    {
        #region Property storage variables

        private List<ColumnReference> columnReferences;

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

        public List<ColumnReference> ColumnReferences
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
            :base(dataType)
        {
            InitializeMembers();
        }

        public DataTypeReference(Parsing.DataTypeIdentifier dt)
            : this()
        {
            InitializeMembers();
            InterpretDataTypeIdentifier(dt);
        }

        private void InitializeMembers()
        {
            this.columnReferences = new List<ColumnReference>();
        }

        private void CopyMembers(DataTypeReference old)
        {
            // Deep copy of column references
            this.columnReferences = new List<ColumnReference>();

            foreach (var cr in old.columnReferences)
            {
                var ncr = new ColumnReference(this, cr);
                this.columnReferences.Add(ncr);
            }
        }

        public override object Clone()
        {
            return new DataTypeReference(this);
        }

        #endregion

        private void InterpretDataTypeIdentifier(Parsing.DataTypeIdentifier dataType)
        {
            var sys = dataType.SystemDataTypeIdentifier;
            var udt = dataType.UdtIdentifier;

            if (sys != null)
            {
                InterpretSystemDataType(sys);
            }
            else
            {
                InterpretUdtIdentifier(udt);
            }
        }

        private void InterpretSystemDataType(Parsing.SystemDataTypeIdentifier dataType)
        {
            var name = Util.RemoveIdentifierQuotes(dataType.TypeName);

            if (Schema.SqlServer.Constants.SqlDataTypes.ContainsKey(name))
            {
                // This is a system type
                var sqltype = Schema.SqlServer.Constants.SqlDataTypes[name];
                var dt = Schema.DataType.Create(sqltype, dataType.Length, dataType.Precision, dataType.Scale, false);

                DatabaseObject = dt;
                DatabaseObjectName = dt.TypeNameWithLength;
            }
            else
            {
                // TODO: implement UDTs and CLR UDTs
                throw new NotImplementedException();
            }

            IsUserDefined = false;
        }

        private void InterpretUdtIdentifier(Parsing.UdtIdentifier dataType)
        {
            var ds = dataType.FindDescendant<Parsing.DatasetName>();
            DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = dataType.FindDescendant<Parsing.DatabaseName>();
            DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = dataType.FindDescendant<Parsing.SchemaName>();
            SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = dataType.FindDescendant<Parsing.FunctionName>();
            DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

            IsUserDefined = true;
        }

        public void InterpretTableDefinition(TableDefinitionList tableDefinition)
        {
            foreach (var item in tableDefinition.EnumerateTableDefinitionItems())
            {
                var cd = item.ColumnDefinition;
                var tc = item.TableConstraint;

                if (cd != null)
                {
                    var cr = new ColumnReference(this, cd.ColumnReference);
                    this.ColumnReferences.Add(cr);
                }

                if (item.TableConstraint != null)
                {
                    // TODO: implement, if index name resolution is required
                }
            }
        }
    }
}
