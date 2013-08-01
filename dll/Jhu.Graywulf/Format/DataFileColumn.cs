using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public class DataFileColumn
    {
        private string name;
        private Type type;
        private int size;
        private bool allowNull;
        private bool isIdentity;
        private string format;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public bool AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
        }

        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public static DataFileColumn Identity
        {
            get
            {
                return new DataFileColumn()
                {
                    Name = "__ID",
                    Type = typeof(long),
                    IsIdentity = true
                };
            }
        }

        public DataFileColumn()
        {
            InitializeMembers();
        }

        public DataFileColumn(string name, Type type)
        {
            InitializeMembers();

            this.name = name;
            this.type = type;
        }

        public DataFileColumn(DataFileColumn old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.type = null;
            this.size = -1;
            this.allowNull = false;
            this.isIdentity = false;
            this.format = null;
        }

        private void CopyMembers(DataFileColumn old)
        {
            this.name = old.name;
            this.type = old.type;
            this.size = old.size;
            this.allowNull = old.allowNull;
            this.isIdentity = old.isIdentity;
            this.format = old.format;
        }

        public void GetTypeInfo(out int size, out byte precision, out byte scale, out bool islong)
        {
            if (this.Type == typeof(sbyte))
            {
                size = 1;
                precision = 3;
                islong = false;
            }
            else if (this.Type == typeof(Int16))
            {
                size = 2;
                precision = 5;
                islong = false;
            }
            else if (this.Type == typeof(Int32))
            {
                size = 4;
                precision = 10;
                islong = false;
            }
            else if (this.Type == typeof(Int64))
            {
                size = 8;
                precision = 19;
                islong = false;
            }
            else if (this.Type == typeof(byte))
            {
                size = 1;
                precision = 3;
                islong = false;
            }
            else if (this.Type == typeof(UInt16))
            {
                size = 2;
                precision = 5;
                islong = false;
            }
            else if (this.Type == typeof(UInt32))
            {
                size = 4;
                precision = 10;
                islong = false;
            }
            else if (this.Type == typeof(UInt64))
            {
                size = 8;
                precision = 19;
                islong = false;
            }
            else if (this.Type == typeof(bool))
            {
                size = 1;
                precision = 255;
                islong = false;
            }
            else if (this.Type == typeof(float))
            {
                size = 4;
                precision = 7;
                islong = false;
            }
            else if (this.Type == typeof(double))
            {
                size = 8;
                precision = 15;
                islong = false;
            }
            else if (this.Type == typeof(decimal))
            {
                size = 8;
                precision = 19;
                islong = false;
            }
            else if (this.Type == typeof(char))
            {
                size = 1;
                precision = 255;
                islong = false;
            }
            else if (this.Type == typeof(string))
            {
                size = this.Size;
                precision = 255;
                islong = false;     // *** TODO
            }
            else if (this.Type == typeof(DateTime))
            {
                size = 8;
                precision = 23;
                islong = false;
            }
            else if (this.Type == typeof(Guid))
            {
                size = 16;
                precision = 255;
                islong = false;
            }
            else
            {
                throw new NotImplementedException();
            }

            scale = 255;
        }
    }
}
