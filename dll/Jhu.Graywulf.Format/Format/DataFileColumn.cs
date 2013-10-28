using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Format
{
    public class DataFileColumn : Column
    {
        public static DataFileColumn Identity
        {
            get
            {
                return new DataFileColumn()
                {
                    Name = "__ID",
                    IsNullable = false,
                    IsIdentity = true,
                    IsKey = true,
                    DataType = DataType.BigInt
                };
            }
        }

        private string format;

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public DataFileColumn()
        {
            InitializeMembers();
        }

        public DataFileColumn(string name, DataType type)
        {
            InitializeMembers();

            this.Name = name;
            this.DataType = type;
        }

        public DataFileColumn(string name, Type type, short size)
        {
            InitializeMembers();

            this.Name = name;
            this.DataType = DataType.Create(type, size);
        }


        public DataFileColumn(DataFileColumn old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.format = "{0}";
        }

        private void CopyMembers(DataFileColumn old)
        {
            this.format = old.format;
        }
    }
}
