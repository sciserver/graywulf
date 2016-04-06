using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities.Mapping
{
    public sealed class DbColumn
    {
        #region Delegates

        private delegate object ValueGetterDelegate(Entity entity);
        private delegate void ValueSetterDelegate(Entity entity, object value);

        #endregion
        #region Private member variables

        private string name;
        private DbColumnBinding binding;
        private object defaultValue;
        private int? order;
        private SqlDbType type;
        private int? size;
        private ValueGetterDelegate getValue;
        private ValueSetterDelegate setValue;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
        }

        public DbColumnBinding Binding
        {
            get { return binding; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
        }

        public int? Order
        {
            get { return order; }
        }

        public SqlDbType Type
        {
            get { return type; }
        }

        public int? Size
        {
            get { return size; }
        }

        #endregion
        #region Constructors and initializers

        public static DbColumn Create(PropertyInfo p, DbColumnAttribute attr)
        {
            var column = new DbColumn();

            column.ReflectColumn(p, attr);
            column.CreateValueGetterDelegate(p);
            column.CreateValueSetterDelegate(p);

            return column;
        }

        private DbColumn()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.binding = DbColumnBinding.Column;
            this.defaultValue = null;
            this.order = null;
            this.type = SqlDbType.Int;
            this.size = null;
            this.getValue = null;
            this.setValue = null;
        }

        #endregion
        #region Reflection logic

        private void ReflectColumn(PropertyInfo p, DbColumnAttribute attr)
        {
            this.name = attr.Name ?? p.Name;
            this.binding = attr.Binding;
            this.defaultValue = attr.DefaultValue;
            this.order = attr.OrderNullable;
            this.size = attr.SizeNullable;

            if (!attr.TypeNullable.HasValue)
            {
                if (!Constants.DbTypeMappings.ContainsKey(p.PropertyType))
                {
                    throw DbError.InvalidColumnType(p.Name, p.ReflectedType);
                }

                this.type = Constants.DbTypeMappings[p.PropertyType];
            }
            else
            {
                this.type = attr.TypeNullable.Value;
            }
        }

        #endregion
        #region Entity property accessor logic

        private void CreateValueGetterDelegate(PropertyInfo p)
        {
            var exps = new List<Expression>();
            var vars = new List<ParameterExpression>();

            // Function parameters
            var entity = Expression.Parameter(typeof(Entity), "entity");

            // Cast entity to strongly typed version
            var ce = Expression.Variable(p.ReflectedType, "ce");
            vars.Add(ce);
            exps.Add(Expression.Assign(ce, Expression.ConvertChecked(entity, p.ReflectedType)));

            // Get value of property
            var pp = Expression.Variable(typeof(object), "pp");
            vars.Add(pp);
            exps.Add(Expression.Assign(pp, Expression.Convert(Expression.Property(ce, p), typeof(object))));

            var blk = Expression.Block(vars, exps);

            var exp = Expression.Lambda<ValueGetterDelegate>(blk, entity);

            this.getValue = exp.Compile();
        }

        private void CreateValueSetterDelegate(PropertyInfo p)
        {
            var exps = new List<Expression>();
            var vars = new List<ParameterExpression>();

            // Function parameters
            var entity = Expression.Parameter(typeof(Entity), "entity");
            var value = Expression.Parameter(typeof(object), "value");

            // Cast entity to strongly typed version
            var ce = Expression.Variable(p.ReflectedType, "ce");
            vars.Add(ce);
            exps.Add(Expression.Assign(ce, Expression.ConvertChecked(entity, p.ReflectedType)));

            // Cast value to strongly typed version
            var cv = Expression.Variable(p.PropertyType, "cv");
            vars.Add(cv);
            exps.Add(Expression.Assign(cv, Expression.ConvertChecked(value, p.PropertyType)));

            // Set value of property
            exps.Add(Expression.Assign(Expression.Property(ce, p), cv));

            var blk = Expression.Block(vars, exps);

            var exp = Expression.Lambda<ValueSetterDelegate>(blk, entity, value);

            this.setValue = exp.Compile();
        }

        public object GetValue(Entity entity)
        {
            return getValue(entity);
        }

        public void SetValue(Entity entity, object value)
        {
            setValue(entity, value);
        }

        #endregion
    }
}
