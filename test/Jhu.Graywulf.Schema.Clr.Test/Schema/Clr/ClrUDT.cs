using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

namespace Jhu.Graywulf.Schema.Clr
{
    [SqlUserDefinedType(Format.Native, Name = "ClrUDT")]
    public struct ClrUDT : INullable
    {
        private bool isNull;
        private int id;
        private double data;

        public bool IsNull
        {
            get
            {
                return (isNull);
            }
        }

        public int ID
        {
            [SqlMethod]
            get { return id; }
            [SqlMethod(IsMutator = true)]
            set { id = value; }
        }

        public double Data
        {
            [SqlMethod]
            get { return data; }
            [SqlMethod(IsMutator = true)]
            set { data = value; }
        }

        public static ClrUDT Null
        {
            get
            {
                var d = new ClrUDT();
                d.isNull = true;
                return d;
            }
        }

        [SqlMethod(OnNullCall = false)]
        public static ClrUDT Parse(SqlString s)
        {
            if (s.IsNull)
            {
                return Null;
            }

            string[] parts = s.Value.Split(",".ToCharArray());
            var d = new ClrUDT();
            d.id = int.Parse(parts[0]);
            d.data = double.Parse(parts[1]);
            return d;
        }

        public override string ToString()
        {
            if (this.IsNull)
            {
                return "NULL";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(id);
                sb.Append(",");
                sb.Append(data);
                return sb.ToString();
            }
        }

        [SqlMethod]
        public string Method()
        {
            return this.ToString();
        }

        [SqlMethod]
        public static string StaticMethod()
        {
            return "Hello from StaticMethod";
        }
    }
}
