using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlCodeGen
{
    public abstract class SqlColumnListGeneratorBase
    {
        private const string columnNull = " NULL";
        private const string columnNotNull = " NOT NULL";

        private TableReference table;
        private string tableAlias;
        private string joinedTableAlias;
        private ColumnContext columnContext;
        private ColumnListType listType;
        private ColumnListNullType nullType;
        private bool leadingComma;

        public TableReference Table
        {
            get { return table; }
            set { table = value; }
        }

        public string TableAlias
        {
            get { return tableAlias; }
            set { tableAlias = value; }
        }

        public string JoinedTableAlias
        {
            get { return joinedTableAlias; }
            set { joinedTableAlias = value; }
        }

        public ColumnContext ColumnContext
        {
            get { return columnContext; }
            set { columnContext = value; }
        }

        public ColumnListType ListType
        {
            get { return listType; }
            set { listType = value; }
        }

        public ColumnListNullType NullType
        {
            get { return nullType; }
            set { nullType = value; }
        }

        public bool LeadingComma
        {
            get { return leadingComma; }
            set { leadingComma = value; }
        }

        protected SqlColumnListGeneratorBase()
        {
            InitializeMembers();
        }

        protected SqlColumnListGeneratorBase(TableReference table, ColumnContext context)
        {
            InitializeMembers();

            this.table = table;
            this.columnContext = context;
        }

        protected SqlColumnListGeneratorBase(TableReference table, ColumnContext context, ColumnListType listType)
        {
            InitializeMembers();

            this.table = table;
            this.columnContext = context;
            this.listType = listType;
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.tableAlias = "";
            this.joinedTableAlias = "";
            this.columnContext = ColumnContext.Default;
            this.listType = ColumnListType.ForSelectWithOriginalNameNoAlias;
            this.nullType = ColumnListNullType.Defined;
            this.leadingComma = false;
        }

        #region Column name escaping

        /// <summary>
        /// Generates and escaped name for a column that should be
        /// propagated.
        /// </summary>
        /// <remarks>
        /// Will generate a name like DS_schema_table_column that
        /// is unique in a table.
        /// </remarks>
        /// <param name="table">Reference to the source table.</param>
        /// <param name="column">Reference to the column.</param>
        /// <returns>The excaped name of the temporary table column.</returns>
        public virtual string EscapeColumnName(TableReference table, string columnName)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}",
                                 table.DatasetName,
                                 table.SchemaName,
                                 table.DatabaseObjectName,
                                 table.Alias,
                                 columnName);
        }

        public virtual string EscapePropagatedColumnName(TableReference table, string columnName)
        {
            return String.Format("_{0}", EscapeColumnName(table, columnName));
        }

        #endregion

        protected virtual string GetNullString(ColumnListNullType nullType)
        {
            string nullstring;

            switch (nullType)
            {
                case ColumnListNullType.Nothing:
                case ColumnListNullType.Defined:
                    nullstring = String.Empty;
                    break;
                case ColumnListNullType.Null:
                    nullstring = columnNull;
                    break;
                case ColumnListNullType.NotNull:
                    nullstring = columnNotNull;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return nullstring;
        }

        protected virtual string GetFormatString(ColumnListType listType)
        {
            // 0: table alias
            // 1: escaped column name
            // 2: original column name
            // 3: column type
            // 4: null string

            string format = null;

            switch (listType)
            {
                case ColumnListType.ForCreateTableWithOriginalName:
                    format = "{2} {3}{4}";
                    break;
                case ColumnListType.ForCreateTableWithEscapedName:
                    format = "{1} {3}{4}";
                    break;
                case ColumnListType.ForCreateView:
                case ColumnListType.ForCreateIndex:
                case ColumnListType.ForInsert:
                    format = "{2}";
                    break;
                case ColumnListType.ForSelectWithOriginalName:
                    format = "{0}{2} AS {1}";
                    break;
                case ColumnListType.ForSelectWithEscapedName:
                    format = "{0}{1}";
                    break;
                case ColumnListType.ForSelectWithOriginalNameNoAlias:
                    format = "{0}{2}";
                    break;
                case ColumnListType.ForSelectWithEscapedNameNoAlias:
                    format = "{0}{1}";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return format;
        }

        protected abstract string GetQuotedIdentifier(string identifier);

        /// <summary>
        /// Returns a SQL snippet with the list of primary keys
        /// and propagated columns belonging to the table.
        /// </summary>
        /// <param name="table">Reference to the table.</param>
        /// <param name="type">Column list type.</param>
        /// <param name="nullType">Column nullable type.</param>
        /// <param name="tableAlias">Optional table alias prefix, specify null to omit.</param>
        /// <returns>A SQL snippet with the list of columns.</returns>
        public string GetColumnListString()
        {
            var nullstring = GetNullString(nullType);
            var format = GetFormatString(listType);

            var columnlist = new StringBuilder();
            var columns = table.GetColumnList(columnContext);

            foreach (var column in columns)
            {
                if (leadingComma || columnlist.Length != 0)
                {
                    columnlist.Append(", ");
                }

                // Alias
                string alias;

                if (tableAlias == String.Empty)
                {
                    alias = String.Empty;
                }
                else if (tableAlias != null)
                {
                    alias = GetQuotedIdentifier(tableAlias) + ".";
                }
                else if (String.IsNullOrWhiteSpace(table.Alias))
                {
                    alias = String.Empty;
                }
                else
                {
                    alias = GetQuotedIdentifier(table.Alias) + ".";
                }

                // Null
                string nullspec;

                if (nullType == ColumnListNullType.Defined)
                {
                    nullspec = column.DataType.IsNullable ? columnNull : columnNotNull;
                }
                else
                {
                    nullspec = nullstring;
                }

                columnlist.AppendFormat(
                    format,
                    alias,
                    GetQuotedIdentifier(EscapePropagatedColumnName(table, column.Name)),
                    GetQuotedIdentifier(column.Name),
                    column.DataType.NameWithLength,
                    nullspec);
            }

            return columnlist.ToString();
        }

        public string GetJoinString()
        {
            var columnlist = new StringBuilder();
            var columns = table.GetColumnList(columnContext);

            foreach (var column in columns)
            {
                if (columnlist.Length != 0)
                {
                    columnlist.Append(" AND ");
                }

                string alias1 = GetQuotedIdentifier(tableAlias);
                string alias2 = GetQuotedIdentifier(joinedTableAlias);
                string columnname = GetQuotedIdentifier(EscapePropagatedColumnName(table, column.Name));

                columnlist.AppendFormat(
                    "{0}.{2} = {1}.{2}",
                    alias1,
                    alias2,
                    columnname);
            }

            return columnlist.ToString();
        }
    }
}
