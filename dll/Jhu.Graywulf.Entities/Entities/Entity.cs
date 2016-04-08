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
                return !GetKey().Equals(DbTable.Key.DefaultValue);
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
            return DbTable.Columns.Values.Where(ci => (ci.Binding & binding) != 0).OrderBy(ci => ci.Order);
        }

        private string GetColumnListString(IEnumerable<DbColumn> columns, DbColumnListType type)
        {
            var sb = new StringBuilder();
            string format = null;

            switch (type)
            {
                case DbColumnListType.Select:
                    format = "[{0}]";
                    break;
                case DbColumnListType.Insert:
                    format = "[{0}]";
                    break;
                case DbColumnListType.InsertValues:
                    format = "@{0}";
                    break;
                case DbColumnListType.Update:
                    format = "[{0}] = @{0}";
                    break;
                case DbColumnListType.Where:
                    format = "[{0}] = @{0}";
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

        private void AppendColumnListParameters(SqlCommand cmd, IEnumerable<DbColumn> columns)
        {
            foreach (var c in columns)
            {
                cmd.Parameters.Add(c.GetParameter(this));
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

            var columns = GetColumnList(DbColumnBinding.Key | DbColumnBinding.Column);

            return String.Format(
                sql,
                GetColumnListString(columns, DbColumnListType.Select),
                DbTable.Name);
        }

        public void LoadFromDataReader(SqlDataReader reader)
        {
            var columns = GetColumnList(DbColumnBinding.Any);

            foreach (var c in columns)
            {
                LoadColumnFromDataReader(reader, c);
            }

            isLoaded = true;
            isDirty = false;
        }

        protected virtual void LoadColumnFromDataReader(SqlDataReader reader, DbColumn c)
        {
            c.LoadFromDataReader(this, reader);
        }

        protected virtual SqlCommand GetInsertCommand()
        {
            var sql = @"
INSERT [{0}]
    ({1})
OUTPUT INSERTED.[{3}]
VALUES
    ({2});
";

            var columns = GetColumnList(DbColumnBinding.Column);

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    DbTable.Name,
                    GetColumnListString(columns, DbColumnListType.Insert),
                    GetColumnListString(columns, DbColumnListType.InsertValues),
                    DbTable.Key.Name));

            AppendColumnListParameters(cmd, columns);

            return cmd;
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
WHERE [{1}] = @{1};
";

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    GetTableQuery(),
                    DbTable.Key.Name));

            cmd.Parameters.Add(DbTable.Key.GetParameter(this));

            return cmd;
        }

        protected virtual SqlCommand GetUpdateCommand()
        {
            var sql = @"
UPDATE [{0}]
SET {1}
WHERE [{2}] = @{2};

SELECT @@ROWCOUNT;
";

            var columns = GetColumnList(DbColumnBinding.Column);

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    DbTable.Name,
                    GetColumnListString(columns, DbColumnListType.Update),
                    DbTable.Key.Name));

            cmd.Parameters.Add(DbTable.Key.GetParameter(this));
            AppendColumnListParameters(cmd, columns);

            return cmd;
        }

        protected virtual SqlCommand GetDeleteCommand()
        {
            var sql = @"
DELETE {0}
WHERE {1} = @{1};

SELECT @@ROWCOUNT;
";

            var cmd = new SqlCommand(
                String.Format(
                    sql,
                    DbTable.Name,
                    DbTable.Key.Name));

            cmd.Parameters.Add(DbTable.Key.GetParameter(this));

            return cmd;
        }

        public virtual void Save()
        {
            bool cancel = false;
            
            OnSaving(ref cancel);

            if (cancel)
            {
                return;
            }

            if (!IsExisting)
            {
                Create();
            }
            else
            {
                Modify();
            }

            isDirty = false;

            OnSaved();
        }

        protected virtual void OnSaving(ref bool cancel)
        {
        }

        protected virtual void OnSaved()
        {
        }

        private void Create()
        {
            bool cancel = false;

            OnCreating(ref cancel);

            if (cancel)
            {
                return;
            }

            using (var cmd = GetInsertCommand())
            {
                var id = Context.ExecuteCommandScalar(cmd);

                if (id == DbTable.Key.DefaultValue)
                {
                    throw Error.ErrorCreateEntity();
                }
                else
                {
                    SetKey(id);
                }
            }

            OnCreated();
        }

        protected virtual void OnCreating(ref bool cancel)
        {
        }

        protected virtual void OnCreated()
        {
        }

        public void Load()
        {
            bool cancel = false;

            OnLoading(ref cancel);

            if (cancel)
            {
                return;
            }

            using (var cmd = GetSelectCommand())
            {
                Context.ExecuteCommandAsSingleObject(cmd, this);
            }

            OnLoaded();
        }

        protected virtual void OnLoading(ref bool cancel)
        {
        }

        protected virtual void OnLoaded()
        {
        }

        private void Modify()
        {
            bool cancel = false;

            OnModifying(ref cancel);

            if (cancel)
            {
                return;
            }

            using (var cmd = GetUpdateCommand())
            {
                int count = (int)Context.ExecuteCommandScalar(cmd);

                if (count == 0)
                {
                    throw Error.ErrorModifyEntity();
                }
            }

            OnModified();
        }

        protected virtual void OnModifying(ref bool cancel)
        {
        }

        protected virtual void OnModified()
        {
        }

        public virtual void Delete()
        {
            bool cancel = false;

            OnDeleting(ref cancel);

            if (cancel)
            {
                return;
            }

            using (var cmd = GetDeleteCommand())
            {
                int count = (int)Context.ExecuteCommandScalar(cmd);

                if (count == 0)
                {
                    throw Error.ErrorDeleteEntity();
                }
            }

            OnDeleted();
        }

        protected virtual void OnDeleting(ref bool cancel)
        {
        }

        protected virtual void OnDeleted()
        {
        }

        #endregion
    }
}
