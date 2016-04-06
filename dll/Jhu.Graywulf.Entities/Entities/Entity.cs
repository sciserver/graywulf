using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    public abstract class Entity : ContextObject, IDatabaseTableObject
    {
        #region Private member variables

        private DbTable dbTable;
        private bool isLoaded;
        private bool isDirty;

        #endregion
        #region Properties

        private DbTable DbTable
        {
            get
            {
                if (dbTable == null)
                {
                    dbTable = DbTable.GetDbTable(this.GetType());
                }

                return dbTable;
            }
        }

        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        public bool IsDirty
        {
            get { return isDirty; }
        }

        public bool IsExisting
        {
            get
            {
                return GetKey() != DbTable.Key.DefaultValue;
            }
        }

        #endregion
        #region Constructors and initializers

        protected Entity()
        {
            InitializeMembers();
        }

        protected Entity(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        protected Entity(Entity old)
            : base(old.Context)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.isLoaded = false;
            this.isDirty = false;
        }

        private void CopyMembers(Entity old)
        {
            this.isLoaded = old.isLoaded;
            this.isDirty = old.isDirty;
        }

        #endregion
        #region Key and property access functions

        public virtual void SetDirty()
        {
            this.isDirty = true;
        }

        public object GetKey()
        {
            return DbTable.Key.GetValue(this);
        }

        public void SetKey(object value)
        {
            DbTable.Key.SetValue(this, value);
            SetDirty();
        }

        #endregion
        #region Database column functions

        protected virtual IEnumerable<DbColumn> GetColumnList(DbColumnBinding binding)
        {
            IEnumerable<DbColumn> columns;

            if ((binding & DbColumnBinding.Key) != 0)
            {
                columns = new[] { DbTable.Key }.Concat(DbTable.Columns.Values);
            }
            else
            {
                columns = DbTable.Columns.Values;
            }

            return columns.Where(ci => (ci.Binding & binding) != 0).OrderBy(ci => ci.Order);
        }

        private string GetColumnListString(IEnumerable<DbColumn> columns, DbColumnListType type)
        {
            var sb = new StringBuilder();
            string format = null;

            switch (type)
            {
                case DbColumnListType.Select:
                    format = "{0}";
                    break;
                case DbColumnListType.Insert:
                    format = "{0}";
                    break;
                case DbColumnListType.InsertValues:
                    format = "@{0}";
                    break;
                case DbColumnListType.Update:
                    format = "{0} = @{0}";
                    break;
                case DbColumnListType.Where:
                    format = "{0} = @{0}";
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var c in columns)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine(",");
                }

                sb.AppendFormat(format, c.Name);
            }

            return sb.ToString();
        }

        private void AppendColumnParameter(SqlCommand cmd, DbColumn c)
        {
            if (c.Size.HasValue)
            {
                cmd.Parameters.Add("@" + c.Name, c.Type, c.Size.Value).Value = c.GetValue(this);
            }
            else
            {
                cmd.Parameters.Add("@" + c.Name, c.Type).Value = c.GetValue(this);
            }
        }

        private void AppendColumnListParameters(SqlCommand cmd, IEnumerable<DbColumn> columns)
        {
            foreach (var c in columns)
            {
                AppendColumnParameter(cmd, c);   
            }
        }

        #endregion
        #region CRUD functions

        protected virtual string GetTableQuery()
        {
            var sql = @"
SELECT {0}
FROM {1}
";

            var columns = GetColumnList(DbColumnBinding.Any);

            return String.Format(
                sql,
                GetColumnListString(columns, DbColumnListType.Select),
                DbTable.Name);
        }

        public virtual void LoadFromDataReader(SqlDataReader reader)
        {
            var columns = GetColumnList(DbColumnBinding.Any);

            foreach (var c in columns)
            {
                c.SetValue(this, reader.GetValue(reader.GetOrdinal(c.Name)));
            }

            isLoaded = true;
            isDirty = false;
        }

        protected virtual SqlCommand GetInsertCommand()
        {
            if ((DbTable.Key.Binding & DbColumnBinding.Identity) != 0)
            {
                var sql = @"
SET NOCOUNT ON;

INSERT {0}
    ({1})
VALUES
    ({2});

SET NOCOUNT OFF;

RETURN @@IDENTITY;
";

                var columns = GetColumnList(DbColumnBinding.Column);

                var cmd = new SqlCommand(
                    String.Format(
                        sql,
                        DbTable.Name,
                        GetColumnListString(columns, DbColumnListType.Insert)));

                AppendColumnListParameters(cmd, columns);

                return cmd;
            }
            else
            {
                var sql = @"
SET NOCOUNT ON;

INSERT {0}
    ({1})
VALUES
    ({2});

SET NOCOUNT OFF;

RETURN @{3}
";

                var columns = GetColumnList(DbColumnBinding.Column | DbColumnBinding.Key);

                var cmd = new SqlCommand(
                    String.Format(
                        sql,
                        DbTable.Name,
                        GetColumnListString(columns, DbColumnListType.Insert),
                        DbTable.Key.Name));

                AppendColumnListParameters(cmd, columns);

                return cmd;
            }
        }

        protected virtual SqlCommand GetSelectCommand()
        {
            var sql = @"
WITH __e AS
(
{0}
)
SELECT * 
FROM __e
WHERE {1} = @{1};
";

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    GetTableQuery(),
                    DbTable.Name,
                    DbTable.Key.Name));

            AppendColumnParameter(cmd, DbTable.Key);

            return cmd;
        }

        protected virtual SqlCommand GetUpdateCommand()
        {
            var sql = @"
SET NOCOUNT ON;

UPDATE {0}
SET {1}
WHERE {2} = @{2};

SET NOCOUNT OFF;

RETURN @@ROWCOUNT;
";

            var columns = GetColumnList(DbColumnBinding.Column);

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    DbTable.Name,
                    GetColumnListString(columns, DbColumnListType.Insert),
                    DbTable.Key.Name));

            AppendColumnParameter(cmd, DbTable.Key);
            AppendColumnListParameters(cmd, columns);

            return cmd;
        }

        protected virtual SqlCommand GetDeleteCommand()
        {
            var sql = @"
SET NOCOUNT ON;

DELETE {0}
WHERE {1} = @{1};

SET NOCOUNT OFF;

RETURN @@ROWCOUNT;
";

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    DbTable.Name,
                    DbTable.Key.Name));

            AppendColumnParameter(cmd, DbTable.Key);

            return cmd;
        }

        public virtual void Save()
        {
            if (IsExisting)
            {
                Create();
            }
            else
            {
                Modify();
            }
        }

        private void Create()
        {
            using (var cmd = GetInsertCommand())
            {
                var retval = Context.ExecuteCommandNonQuery(cmd);   
                
                if (retval == DbTable.Key.DefaultValue)
                {
                    throw Error.ErrorCreateEntity();
                }
                else
                {
                    SetKey(retval);
                }
            }
        }

        public void Load()
        {
            using (var cmd = GetSelectCommand())
            {
                Context.ExecuteCommandAsSingleObject(cmd, this);
            }
        }

        private void Modify()
        {
            using (var cmd = GetUpdateCommand())
            {
                long retval = (long)Context.ExecuteCommandNonQuery(cmd); 

                if (retval == 0)
                {
                    throw Error.ErrorModifyEntity();
                }
            }
        }

        public virtual void Delete()
        {
            using (var cmd = GetDeleteCommand())
            {
                long retval = (long)Context.ExecuteCommandNonQuery(cmd);

                if (retval == 0)
                {
                    throw Error.ErrorDeleteEntity();
                }
            }
        }

        #endregion
    }
}
