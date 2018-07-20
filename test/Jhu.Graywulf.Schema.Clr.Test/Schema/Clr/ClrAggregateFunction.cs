using System;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.IO;
using System.Text;

namespace Jhu.Graywulf.Schema.Clr
{
    [Serializable]
    [SqlUserDefinedAggregate(
        Format.UserDefined,
        IsInvariantToNulls = true,
        IsInvariantToDuplicates = false,
        IsInvariantToOrder = false,
        MaxByteSize = 8000,
        Name = "dbo.ClrAggregateFunction")
    ]
    public class ClrAggregateFunction : IBinarySerialize
    {
        private StringBuilder intermediateResult;

        public void Init()
        {
            this.intermediateResult = new StringBuilder();
        }

        public void Accumulate(SqlString value)
        {
            if (value.IsNull)
            {
                return;
            }

            this.intermediateResult.Append(value.Value).Append(',');
        }

        public void Merge(ClrAggregateFunction other)
        {
            this.intermediateResult.Append(other.intermediateResult);
        }

        public SqlString Terminate()
        {
            string output = string.Empty;
            //delete the trailing comma, if any  
            if (this.intermediateResult != null
                && this.intermediateResult.Length > 0)
            {
                output = this.intermediateResult.ToString(0, this.intermediateResult.Length - 1);
            }

            return new SqlString(output);
        }

        public void Read(BinaryReader r)
        {
            intermediateResult = new StringBuilder(r.ReadString());
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this.intermediateResult.ToString());
        }
    }
}