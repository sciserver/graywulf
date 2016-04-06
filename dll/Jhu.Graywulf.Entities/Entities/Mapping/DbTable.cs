using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.Entities.Mapping
{
    public sealed class DbTable
    {
        #region Static cache

        private static ConcurrentDictionary<Type, DbTable> tables;

        static DbTable()
        {
            tables = new ConcurrentDictionary<Type, DbTable>();
        }

        public static DbTable GetDbTable(Type t)
        {
            DbTable table;

            if (tables.TryGetValue(t, out table))
            {
                return table;
            }
            else
            {
                table = DbTable.Create(t);
                tables.TryAdd(t, table);

                return table;
            }
        }

        #endregion
        #region Private member variables

        private string name;
        private Type type;
        private DbColumn key;
        private DbColumnCollection columns;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }

        public DbColumn Key
        {
            get { return key; }
        }

        public DbColumnCollection Columns
        {
            get { return columns; }
        }

        #endregion
        #region Constructors and inizializers

        public static DbTable Create(Type t)
        {
            var table = new DbTable();

            table.ReflectEntity(t);
            table.ReflectColumns(t);

            return table;
        }

        private DbTable()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.type = null;
            this.key = null;
            this.columns = null;
        }

        #endregion
        #region Reflection logic

        private void ReflectEntity(Type t)
        {
            type = t;

            var atts = t.GetCustomAttributes(typeof(DbTableAttribute), true);

            if (atts.Length != 1)
            {
                throw DbError.NoDbTableAttributeFound(t);
            }

            var dbtable = (DbTableAttribute)atts[0];

            // Default table name to class name
            this.name = dbtable.Name ?? t.Name;
        }

        private void ReflectColumns(Type t)
        {
            this.key = null;
            this.columns = new DbColumnCollection();

            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                var atts = p.GetCustomAttributes(typeof(DbColumnAttribute), true);

                if (atts.Length == 1)
                {
                    var attr = (DbColumnAttribute)atts[0];
                    var col = DbColumn.Create(p, attr);

                    if ((attr.Binding & DbColumnBinding.Key) != 0)
                    {
                        // Make sure DbKey is defined on one column only
                        if (this.key != null)
                        {
                            throw DbError.DuplicateKeyColumn(p.Name, t);
                        }

                        this.key = col;
                    }

                    columns.Add(p.Name, col);
                }
            }
        }

        #endregion
    }
}
