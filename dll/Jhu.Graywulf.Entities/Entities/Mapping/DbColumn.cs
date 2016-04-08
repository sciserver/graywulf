using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Jhu.Graywulf.Entities.AccessControl;

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
        private int? order;
        private Type propertyType;
        private SqlDbType dbType;
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

        public int? Order
        {
            get { return order; }
        }

        public Type PropertyType
        {
            get { return propertyType; }
        }

        public SqlDbType DbType
        {
            get { return dbType; }
        }

        public int? Size
        {
            get { return size; }
        }

        public object DefaultValue
        {
            get
            {
                if (propertyType.IsValueType)
                {
                    return Activator.CreateInstance(this.propertyType);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
        #region Constructors and initializers

        public static DbColumn AclColumn
        {
            get
            {
                return new DbColumn()
                {
                    name = "Acl",
                    binding = DbColumnBinding.Acl,
                    propertyType = typeof(String),
                    dbType = SqlDbType.Xml,
                };                
            }
        }

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
            this.order = null;
            this.propertyType = typeof(Int32);
            this.dbType = SqlDbType.Int;
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
            this.order = attr.OrderNullable;
            this.size = attr.SizeNullable;
            this.propertyType = p.PropertyType;

            if (!attr.TypeNullable.HasValue)
            {
                if (!Constants.TypeToSqlDbType.ContainsKey(p.PropertyType))
                {
                    throw DbError.InvalidColumnType(p.Name, p.ReflectedType);
                }

                this.dbType = Constants.TypeToSqlDbType[p.PropertyType];
            }
            else
            {
                this.dbType = attr.TypeNullable.Value;
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

            if (Constants.SqlDbTypeToType[dbType] != p.PropertyType)
            {
                var tv = Expression.Variable(Constants.SqlDbTypeToType[dbType], "tv");
                vars.Add(tv);
                exps.Add(Expression.Assign(tv, Expression.ConvertChecked(value, Constants.SqlDbTypeToType[dbType])));
                exps.Add(Expression.Assign(cv, Expression.ConvertChecked(cv, p.PropertyType)));
            }
            else
            {
                exps.Add(Expression.Assign(cv, Expression.ConvertChecked(value, p.PropertyType)));
            }

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
            if ((binding & DbColumnBinding.Acl) != 0)
            {
                ((SecurableEntity)entity).Permissions = EntityAcl.FromXml((string)value);
            }
            else if (dbType == SqlDbType.Xml)
            {
                var xml = new XmlDocument();
                xml.LoadXml((string)value);
                setValue(entity, xml.DocumentElement);
            }
            else
            {
                setValue(entity, value);
            }
        }

        public SqlParameter GetParameter(Entity entity)
        {
            SqlParameter par;

            if ((binding & DbColumnBinding.Acl) != 0)
            {
                par = new SqlParameter("@" + name, dbType);
                par.Value = ((SecurableEntity)entity).Permissions.ToXml();
            }
            else if (dbType == SqlDbType.Xml)
            {
                par = new SqlParameter("@" + name, dbType);
                par.Value = ((XmlElement)GetValue(entity)).OuterXml;
            }
            else
            {
                if (size.HasValue)
                {
                    par = new SqlParameter("@" + name, dbType, size.Value);
                    par.Value = GetValue(entity);
                }
                else
                {
                    par = new SqlParameter("@" + name, dbType);
                    par.Value = GetValue(entity);
                }
            }

            return par;
        }

        #endregion
    }
}
