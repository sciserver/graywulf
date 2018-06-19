using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class DataTypeReference : DatabaseObjectReference
    {
        #region Property storage variables

        private string systemDataTypeName;
        private bool isUdt;

        #endregion
        #region Properties

        public Schema.DataType DataType
        {
            get { return (Schema.DataType)DatabaseObject; }
            set { DatabaseObject = value; }
        }

        public bool IsUdt
        {
            get { return isUdt; }
        }

        public bool IsSystem
        {
            get { return !isUdt; }
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
                    return systemDataTypeName;
                }
            }
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

        public DataTypeReference(Parsing.DataTypeIdentifier dt)
            : this()
        {
            InitializeMembers();
            InterpretDataType(dt);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(DataTypeReference old)
        {
        }

        public override object Clone()
        {
            return new DataTypeReference(this);
        }

        #endregion

        private void InterpretDataType(Parsing.DataTypeIdentifier dataType)
        {
            var sys = dataType.SystemDataTypeIdentifier;
            var udt = dataType.UdtIdentifier;
            var isNullable = dataType.IsNullable;

            if (sys != null)
            {
                InterpretSystemDataType(sys, isNullable);
            }
            else
            {
                InterpretUdtIdentifier(udt, isNullable);
            }
        }

        private void InterpretSystemDataType(Parsing.SystemDataTypeIdentifier dataType, bool isNullable)
        {
            var name = Util.RemoveIdentifierQuotes(dataType.TypeName);

            if (Schema.SqlServer.Constants.SqlDataTypes.ContainsKey(name))
            {
                // This is a system type
                var sqltype = Schema.SqlServer.Constants.SqlDataTypes[name];
                var dt = Schema.DataType.Create(sqltype, dataType.Length, dataType.Precision, dataType.Scale, isNullable);

                DatabaseObject = dt;
                DatabaseObjectName = dt.TypeNameWithLength;
            }
            else
            {
                // TODO: implement UDTs and CLR UDTs
                throw new NotImplementedException();
            }

            isUdt = false;
        }

        private void InterpretUdtIdentifier(Parsing.UdtIdentifier dataType, bool isNullable)
        {
            var ds = dataType.FindDescendant<Parsing.DatasetName>();
            DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = dataType.FindDescendant<Parsing.DatabaseName>();
            DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = dataType.FindDescendant<Parsing.SchemaName>();
            SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = dataType.FindDescendant<Parsing.FunctionName>();
            DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

            isUdt = true;
        }
    }
}
