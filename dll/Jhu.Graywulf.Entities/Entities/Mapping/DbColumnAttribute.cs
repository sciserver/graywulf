using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbColumnAttribute : Attribute
    {
        #region Private member variables

        private string name;
        private DbColumnBinding binding;
        private int? order;
        private SqlDbType? type;
        private int? size;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DbColumnBinding Binding
        {
            get { return binding; }
            set { binding = value; }
        }

        public int Order
        {
            get { return order.Value; }
            set { order = value; }
        }

        internal int? OrderNullable
        {
            get { return order; }
            set { order = value; }
        }

        public SqlDbType Type
        {
            get { return type.Value; }
            set { type = value; }
        }

        internal SqlDbType? TypeNullable
        {
            get { return type; }
            set { type = value; }
        }

        public int Size
        {
            get { return size.Value; }
            set { size = value; }
        }

        internal int? SizeNullable
        {
            get { return size; }
            set { size = value; }
        }

        #endregion
        #region Constructors and initializers

        public DbColumnAttribute()
        {
            InitializeMembers();
        }

        public DbColumnAttribute(string name)
        {
            InitializeMembers();

            this.name = name;
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.binding = DbColumnBinding.Column;
            this.order = null;
            this.type = null;
            this.size = null;
        }

        #endregion
    }
}
