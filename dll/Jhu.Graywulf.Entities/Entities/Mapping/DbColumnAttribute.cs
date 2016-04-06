using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbColumnAttribute : DbObjectAttributeBase
    {
        #region Private member variables

        private DbColumnBinding binding;
        private object defaultValue;
        private int? order;
        private SqlDbType? type;
        private int? size;

        #endregion
        #region Properties

        public DbColumnBinding Binding
        {
            get { return binding; }
            set { binding = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
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
            : base(name)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.binding = DbColumnBinding.Column;
            this.defaultValue = null;
            this.order = null;
            this.type = null;
            this.size = null;
        }

        #endregion
    }
}
