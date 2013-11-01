using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Format
{
    public class DataFileColumn : IColumn
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
                    DataType = DataType.SqlBigInt
                };
            }
        }

        private int id;
        private string name;
        private DataType dataType;
        private DataFileColumnMetadata metadata;
        private bool isNullable;
        private bool isIdentity;
        private bool isKey;
        private bool isHidden;

        /// <summary>
        /// Ordinal ID of the objects (column, parameter, etc).
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Name of the variable
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Data type
        /// </summary>
        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public IVariableMetadata Metadata
        {
            get { return metadata; }
            set { metadata = (DataFileColumnMetadata)value; }
        }

        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }

        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
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
            this.id = 0;
            this.name = null;
            this.dataType = null;
            this.metadata = new DataFileColumnMetadata();
            this.isNullable = false;
            this.isIdentity = false;
            this.isKey = false;
            this.isHidden = false;
        }

        private void CopyMembers(DataFileColumn old)
        {
            this.id = old.id;
            this.name = old.name;
            this.dataType = old.dataType;
            this.metadata = new DataFileColumnMetadata(old.metadata);
            this.isNullable = old.isNullable;
            this.isIdentity = old.isIdentity;
            this.isKey = old.isKey;
            this.isHidden = old.isHidden;
        }
    }
}
