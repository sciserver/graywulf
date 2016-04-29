using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Entities.Mapping
{
    public sealed class DbColumn
    {
        #region Delegates

        private delegate object PropertyValueGetterDelegate(object obj);
        private delegate void PropertyValueSetterDelegate(object obj, object value);

        #endregion
        #region Private member variables

        private string name;
        private DbColumnBinding binding;
        private int? order;
        private Type propertyType;
        private SqlDbType dbType;
        private int? size;
        private bool? isNullable;
        private bool isRange;
        private PropertyValueGetterDelegate getPropertyValue;
        private PropertyValueSetterDelegate setPropertyValue;
        private ValidationAttribute[] validators;

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

        public bool? IsNullable
        {
            get { return isNullable; }
        }

        public bool IsRange
        {
            get { return isRange; }
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
                    name = Constants.AclColumnName,
                    binding = DbColumnBinding.Acl,
                    propertyType = null,
                    dbType = SqlDbType.Binary,
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
            this.isNullable = null;
            this.isRange = false;
            this.getPropertyValue = null;
            this.setPropertyValue = null;
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
            // TODO: add IsNullable to attribute and use value if provided

            if (!attr.TypeNullable.HasValue)
            {
                Type type = p.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    this.isNullable = true;
                    type = type.GetGenericArguments()[0];
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Range<>))
                {
                    this.isRange = true;
                    type = type.GetGenericArguments()[0];
                }

                if (type.IsEnum)
                {
                    type = type.GetEnumUnderlyingType();
                }

                if (!Constants.TypeToSqlDbType.ContainsKey(type))
                {
                    throw DbError.InvalidColumnType(p.Name, p.ReflectedType);
                }

                this.dbType = Constants.TypeToSqlDbType[type];
            }
            else
            {
                this.dbType = attr.TypeNullable.Value;
            }

            validators = (ValidationAttribute[])p.GetCustomAttributes(typeof(ValidationAttribute), true);
        }

        #endregion
        #region Entity property accessor logic

        private void CreateValueGetterDelegate(PropertyInfo p)
        {
            var exps = new List<Expression>();
            var vars = new List<ParameterExpression>();

            // Function parameters
            var obj = Expression.Parameter(typeof(object), "obj");

            // Cast entity to strongly typed version
            var ce = Expression.Variable(p.ReflectedType, "ce");
            vars.Add(ce);
            exps.Add(Expression.Assign(ce, Expression.ConvertChecked(obj, p.ReflectedType)));

            // Get value of property
            var pp = Expression.Variable(typeof(object), "pp");
            vars.Add(pp);
            exps.Add(Expression.Assign(pp, Expression.Convert(Expression.Property(ce, p), typeof(object))));

            var blk = Expression.Block(vars, exps);

            var exp = Expression.Lambda<PropertyValueGetterDelegate>(blk, obj);

            this.getPropertyValue = exp.Compile();
        }

        private void CreateValueSetterDelegate(PropertyInfo p)
        {
            var exps = new List<Expression>();
            var vars = new List<ParameterExpression>();

            // Function parameters
            var obj = Expression.Parameter(typeof(object), "obj");
            var value = Expression.Parameter(typeof(object), "value");

            // Cast entity to strongly typed version
            var ce = Expression.Variable(p.ReflectedType, "ce");
            vars.Add(ce);
            exps.Add(Expression.Assign(ce, Expression.ConvertChecked(obj, p.ReflectedType)));

            // Cast value to strongly typed version
            var cv = Expression.Variable(p.PropertyType, "cv");
            vars.Add(cv);

            // Handle nullable types
            Type basetype = p.PropertyType;
            Type sqltype = Constants.SqlDbTypeToType[dbType];
            bool nullable = false;

            if (basetype.IsGenericType && basetype.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                basetype = basetype.GetGenericArguments()[0];
                nullable = true;
            }

            if (basetype.IsEnum)
            {
                basetype = basetype.GetEnumUnderlyingType();
            }

            if (sqltype != basetype)
            {
                if (nullable)
                {
                    sqltype = typeof(Nullable<>).MakeGenericType(sqltype);
                }

                var tv = Expression.Variable(sqltype, "tv");
                vars.Add(tv);
                exps.Add(Expression.Assign(tv, Expression.ConvertChecked(value, sqltype)));
                exps.Add(Expression.Assign(cv, Expression.ConvertChecked(cv, p.PropertyType)));
            }
            else
            {
                exps.Add(Expression.Assign(cv, Expression.ConvertChecked(value, p.PropertyType)));
            }

            // Set value of property
            exps.Add(Expression.Assign(Expression.Property(ce, p), cv));

            var blk = Expression.Block(vars, exps);

            var exp = Expression.Lambda<PropertyValueSetterDelegate>(blk, obj, value);

            this.setPropertyValue = exp.Compile();
        }

        public object GetPropertyValue(object obj)
        {
            return getPropertyValue(obj);
        }

        public void SetPropertyValue(object obj, object value)
        {
            if ((binding & DbColumnBinding.Acl) != 0)
            {
                ((SecurableEntity)obj).Permissions = AccessControlList.FromBinary((byte[])value);
            }
            else if (dbType == SqlDbType.Xml)
            {
                var xml = new XmlDocument();
                xml.LoadXml((string)value);
                setPropertyValue(obj, xml.DocumentElement);
            }
            else
            {
                setPropertyValue(obj, value == DBNull.Value ? null : value);
            }
        }

        public SqlParameter GetParameter(object obj)
        {
            SqlParameter par;
            object value;

            if ((binding & DbColumnBinding.Acl) != 0)
            {
                par = new SqlParameter("@" + name, dbType);
                value = ((SecurableEntity)obj).Permissions.ToBinary();
            }
            else if (dbType == SqlDbType.Xml)
            {
                par = new SqlParameter("@" + name, dbType);
                value = ((XmlElement)GetPropertyValue(obj)).OuterXml;
            }
            else
            {
                if (size.HasValue)
                {
                    par = new SqlParameter("@" + name, dbType, size.Value);
                }
                else
                {
                    par = new SqlParameter("@" + name, dbType);
                }

                value = GetPropertyValue(obj);
            }

            par.Value = value == null ? DBNull.Value : value;

            return par;
        }

        public SqlParameter[] GetRangeParameters(object obj)
        {
            var pars = new SqlParameter[2];
            var val = (IRange)GetPropertyValue(obj);

            for (int i = 0; i < 2; i++)
            {
                if (i == 0 && val.From != null ||
                    i == 1 && val.To != null)
                {
                    var name = "@" + this.name + (i == 0 ? "_from" : "_to");

                    if (size.HasValue)
                    {
                        pars[i] = new SqlParameter(name, dbType, size.Value);
                    }
                    else
                    {
                        pars[i] = new SqlParameter(name, dbType);
                    }

                    pars[i].Value = i == 0 ? val.From : val.To;
                }
            }

            return pars;
        }

        public void LoadFromDataReader(Entity entity, SqlDataReader reader)
        {
            int i;

            // Auxiliary columns don't always exist in the resultset
            if ((binding & DbColumnBinding.Acl) != 0)
            {
                i = reader.GetOrdinal(name);
                var bytes = reader.GetSqlBinary(i).Value;
                ((SecurableEntity)entity).Permissions = AccessControlList.FromBinary(bytes);

                return;
            }
            else if ((binding | DbColumnBinding.Auxiliary) != 0)
            {
                try
                {
                    i = reader.GetOrdinal(name);
                }
                catch (Exception)
                {
                    i = -1;
                }
            }
            else
            {
                i = reader.GetOrdinal(name);
            }

            if (i >= 0)
            {
                SetPropertyValue(entity, reader.GetValue(i));
            }
        }

        internal bool GetSearchCriterion(object obj, out string criterion, out SqlParameter[] parameters)
        {
            criterion = null;
            parameters = null;

            var val = GetPropertyValue(obj);

            if (val == null || val.Equals(DefaultValue))
            {
                return false;
            }

            if (propertyType == typeof(string))
            {
                var str = (string)GetPropertyValue(obj);

                if (str.IndexOf('%') >= 0)
                {
                    criterion = String.Format("[{0}] LIKE @{0}", name);
                }
                else
                {
                    criterion = String.Format("[{0}] = @{0}", name);
                }

                parameters = new[] { GetParameter(obj) };
            }
            else if (isRange)
            {
                var from = ((IRange)val).From;
                var to = ((IRange)val).To;

                if (from != null && to != null)
                {
                    criterion = String.Format("[{0}] BETWEEN @{0}_from AND @{0}_to", name);
                }
                else if (to == null)
                {
                    criterion = String.Format("[{0}] >= @{0}_from", name);
                }
                else if (from == null)
                {
                    criterion = String.Format("[{0}] <= @{0}_to", name);
                }

                parameters = GetRangeParameters(obj);
            }
            else
            {
                criterion = String.Format("[{0}] = @{0}", name);
                parameters = new [] { GetParameter(obj) };
            }

            return true;
        }

        public void Validate(object obj)
        {
            if (validators != null)
            {
                for (int i = 0; i < validators.Length; i++)
                {
                    validators[i].Validate(GetPropertyValue(obj), name);
                }
            }
        }

        #endregion
    }
}
