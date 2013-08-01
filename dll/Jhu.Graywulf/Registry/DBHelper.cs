using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;

namespace Jhu.Graywulf.Registry
{
    class DBHelper
    {
        private static readonly Dictionary<Type, SqlDbType> SqlTypes = new Dictionary<Type, SqlDbType>()
        {
            { typeof(Boolean), SqlDbType.Bit },
            { typeof(Byte), SqlDbType.TinyInt },
            { typeof(Int16), SqlDbType.SmallInt },
            { typeof(Int32), SqlDbType.Int },
            { typeof(Int64), SqlDbType.BigInt },
            { typeof(Single), SqlDbType.Real },
            { typeof(Double), SqlDbType.Float },
            { typeof(Decimal), SqlDbType.Money },
            { typeof(String), SqlDbType.NVarChar },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(Byte[]), SqlDbType.VarBinary },
        };

        private static readonly Dictionary<Type, bool> SqlTypeHasSize = new Dictionary<Type, bool>()
        {
            { typeof(Boolean), false },
            { typeof(Byte), false },
            { typeof(Int16), false },
            { typeof(Int32), false },
            { typeof(Int64), false },
            { typeof(Single), false },
            { typeof(Double), false },
            { typeof(Decimal), false },
            { typeof(String), true },
            { typeof(DateTime), false },
            { typeof(Byte[]), true },
        };

        private class ColumnDescription
        {
            public PropertyInfo PropertyInfo { get; set; }
            public DBColumnAttribute ColumnAttribute { get; set; }
        }

        public delegate int LoadFromDataReaderDelegate(Entity entity, SqlDataReader dr, int o);
        public delegate void AppendCreateModifyParametersDelegate(Entity entity, SqlCommand cmd);

        private Type type;
        private List<ColumnDescription> columns;

        public string SelectQuery { get; private set; }
        public string InsertQuery { get; private set; }
        public string UpdateQuery { get; private set; }

        public bool HasColumns
        {
            get { return columns.Count > 0; }
        }

        public LoadFromDataReaderDelegate LoadFromDataReader { get; private set; }
        public AppendCreateModifyParametersDelegate AppendCreateModifyParameters { get; private set; }

        public DBHelper(Type type)
        {
            this.type = type;

            FindColumns();

            SelectQuery = CreateSelectQuery();
            InsertQuery = CreateInsertQuery();
            UpdateQuery = CreateUpdateQuery();

            LoadFromDataReader = CreateLoadFromDataReaderDelegate();
            AppendCreateModifyParameters = CreateAppendCreateModifyParametersDelegate();
        }

        public SqlCommand CreateInsertCommand(Entity entity)
        {
            var cmd = entity.Context.CreateTextCommand(InsertQuery);

            cmd.CommandType = CommandType.Text;

            //AppendBasicParameters(entity, cmd);
            AppendCreateModifyParameters(entity, cmd);

            return cmd;
        }

        public SqlCommand CreateUpdateCommand(Entity entity)
        {
            var cmd = entity.Context.CreateTextCommand(UpdateQuery);

            cmd.CommandType = CommandType.Text;

            //AppendBasicParameters(entity, cmd);
            AppendCreateModifyParameters(entity, cmd);

            return cmd;
        }

        private void FindColumns()
        {
            columns = new List<ColumnDescription>();

            foreach (var prop in type.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attrs = prop.GetCustomAttributes(typeof(DBColumnAttribute), false);
                if (attrs != null && attrs.Length == 1)
                {
                    columns.Add(new ColumnDescription()
                    {
                        PropertyInfo = prop,
                        ColumnAttribute = (DBColumnAttribute)attrs[0],
                    });
                }
            }
        }

        private string CreateSelectQuery()
        {
            string sql;

            if (columns.Count == 0)
            {
                sql = @"
SELECT Entity.*
FROM Entity
WHERE Entity.Guid = @Guid
";
            }
            else
            {
                string cols = "EntityGuid";

                foreach (var column in columns)
                {
                    cols += String.Format(", [{0}].[{1}]", type.Name, column.PropertyInfo.Name);
                }

                sql = @"
SELECT Entity.*, {0}
FROM Entity
INNER JOIN [{1}] ON [{1}].EntityGuid = Entity.Guid
WHERE Entity.Guid = @Guid
";

                sql = String.Format(sql, cols, type.Name);

            }

            return sql;
        }

        private LoadFromDataReaderDelegate CreateLoadFromDataReaderDelegate()
        {
            // Create parameters
            var e = Expression.Parameter(typeof(Entity), "e");
            var dr = Expression.Parameter(typeof(SqlDataReader), "dr");
            var o = Expression.Parameter(typeof(int), "o");


            // This will collect expression within the block
            var exps = new List<Expression>();

            // Cast entity to strongly typed version
            var typede = Expression.Variable(type, "ce");
            exps.Add(Expression.Assign(typede, Expression.ConvertChecked(e, type)));

            // Collect fields that are serialized into the database

            // Create assignment statements
            foreach (var column in columns)
            {
                // Increment column counter
                exps.Add(Expression.PostIncrementAssign(o));

                // Get value from DataReader
                var getval = Expression.Call(dr, typeof(SqlDataReader).GetMethod("GetValue"), o);
                var prop = Expression.Property(typede, column.PropertyInfo);

                //exps.Add(Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), o));

                if (column.PropertyInfo.PropertyType == typeof(ExpressionProperty))
                {
                    var cast = Expression.ConvertChecked(getval, typeof(string));
                    var val = Expression.Property(prop, typeof(ExpressionProperty).GetProperty("Value"));

                    exps.Add(Expression.Assign(val, cast));
                }
                else if (column.PropertyInfo.PropertyType.IsEnum)
                {
                    var enumtype = Enum.GetUnderlyingType(column.PropertyInfo.PropertyType);

                    // Cast it to type of column
                    var cast1 = Expression.Unbox(getval, enumtype);
                    var cast2 = Expression.ConvertChecked(cast1, column.PropertyInfo.PropertyType);

                    exps.Add(Expression.Assign(prop, cast2));
                }
                else
                {
                    // Cast it to type of column
                    var cast = Expression.ConvertChecked(getval, column.PropertyInfo.PropertyType);

                    exps.Add(Expression.Assign(prop, cast));
                }
            }

            // return value
            exps.Add(o);

            var exp = Expression.Lambda<LoadFromDataReaderDelegate>(Expression.Block(new[] { typede }, exps), e, dr, o);

            return exp.Compile();
        }

        private string CreateInsertQuery()
        {
            string cols = "[EntityGuid]";
            string pars = "@Guid";

            foreach (var column in columns)
            {
                cols += String.Format(", [{0}]", column.PropertyInfo.Name);
                pars += String.Format(", @{0}", column.PropertyInfo.Name);
            }

            var sql = @"INSERT [{0}] ({1}) VALUES ({2})";

            sql = String.Format(sql, type.Name, cols, pars);

            return sql;
        }

        private string CreateUpdateQuery()
        {
            if (columns.Count > 0)
            {
                string cols = null;

                foreach (var column in columns)
                {
                    if (cols != null)
                    {
                        cols += ", ";
                    }

                    cols += String.Format("[{0}] = @{0}", column.PropertyInfo.Name);
                }

                var sql = @"UPDATE [{0}] SET {1} WHERE EntityGuid = @Guid";

                sql = String.Format(sql, type.Name, cols);

                return sql;
            }
            else
            {
                return null;
            }
        }

        private AppendCreateModifyParametersDelegate CreateAppendCreateModifyParametersDelegate()
        {
            var e = Expression.Parameter(typeof(Entity), "e");
            var cmd = Expression.Parameter(typeof(SqlCommand), "cmd");

            if (columns.Count > 0)
            {
                var parameters = typeof(SqlCommand).GetProperty("Parameters", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.DeclaredOnly);
                var value = typeof(SqlParameter).GetProperty("Value");

                // This will collect expression within the block
                var exps = new List<Expression>();

                // Cast entity to strongly typed version
                var typede = Expression.Variable(type, "ce");
                exps.Add(Expression.Assign(typede, Expression.ConvertChecked(e, type)));

                //
                foreach (var column in columns)
                {
                    var v1 = Expression.Property(cmd, parameters);

                    // Compose Add method parameters
                    var pname = Expression.Constant(String.Format("@{0}", column.PropertyInfo.Name));
                    Expression ptype;
                    Expression psize = null;
                    Expression pval = null;

                    if (column.PropertyInfo.PropertyType.IsEnum)
                    {
                        ptype = Expression.Constant(SqlTypes[Enum.GetUnderlyingType(column.PropertyInfo.PropertyType)]);
                        pval = Expression.Property(typede, column.PropertyInfo);
                    }
                    else if (column.PropertyInfo.PropertyType == typeof(ExpressionProperty))
                    {
                        ptype = Expression.Constant(SqlDbType.NVarChar);
                        psize = Expression.Constant(column.ColumnAttribute.Size);
                        pval = Expression.Property(Expression.Property(typede, column.PropertyInfo), typeof(ExpressionProperty).GetProperty("Value"));
                    }
                    else if (column.PropertyInfo.PropertyType == typeof(DateTime))
                    {
                        ptype = Expression.Constant(SqlDbType.DateTime);
                        pval = Expression.Condition(
                            Expression.Equal(Expression.Property(typede, column.PropertyInfo), Expression.Constant(DateTime.MinValue)),
                            Expression.Convert(Expression.Constant(DBNull.Value), typeof(object)),
                            Expression.Convert(Expression.Property(typede, column.PropertyInfo), typeof(object)));
                    }
                    else
                    {
                        ptype = Expression.Constant(SqlTypes[column.PropertyInfo.PropertyType]);

                        if (SqlTypeHasSize[column.PropertyInfo.PropertyType])
                        {
                            psize = Expression.Constant(column.ColumnAttribute.Size);
                        }

                        pval = Expression.Property(typede, column.PropertyInfo);
                    }

                    Expression v2;

                    if (psize != null)
                    {
                        var add = typeof(SqlParameterCollection).GetMethod("Add", new[] { typeof(string), typeof(SqlDbType), typeof(int) });
                        v2 = Expression.Call(v1, add, pname, ptype, psize);
                    }
                    else
                    {
                        var add = typeof(SqlParameterCollection).GetMethod("Add", new[] { typeof(string), typeof(SqlDbType) });
                        v2 = Expression.Call(v1, add, pname, ptype);
                    }

                    var v3 = Expression.Convert(pval, typeof(object));

                    var v4 = Expression.Assign(Expression.Property(v2, value), v3);

                    exps.Add(v4);
                }

                var exp = Expression.Lambda<AppendCreateModifyParametersDelegate>(Expression.Block(new[] { typede }, exps), e, cmd);

                return exp.Compile();
            }
            else
            {
                var exp = Expression.Lambda<AppendCreateModifyParametersDelegate>(Expression.Empty(), e, cmd);

                return exp.Compile();
            }
        }
    }
}
