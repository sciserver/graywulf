using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class DataTypeReference : DatabaseObjectReference
    {
        #region Properties

        public Schema.DataType DataType
        {
            get { return (Schema.DataType)DatabaseObject; }
            set { DatabaseObject = value; }
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

        public DataTypeReference(Parsing.DataType dt)
            : this()
        {
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

        private void InterpretDataType(Parsing.DataType dataType)
        {
            var name = Util.RemoveIdentifierQuotes(dataType.TypeName);

            if (Schema.SqlServer.Constants.SqlDataTypes.ContainsKey(name))
            {
                // This is a system type
                var sqltype = Schema.SqlServer.Constants.SqlDataTypes[name];
                var dt = Schema.DataType.Create(sqltype, dataType.Length, dataType.Precision, dataType.Scale, dataType.IsNullable);

                DatabaseObject = dt;
                DatabaseObjectName = dt.TypeNameWithLength;
            }
            else
            {
                // TODO: implement UDTs and CLR UDTs
                throw new NotImplementedException();
            }
        }
    }
}
