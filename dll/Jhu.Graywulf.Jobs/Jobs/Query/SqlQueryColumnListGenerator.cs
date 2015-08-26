using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryColumnListGenerator
    {
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

        public SqlQueryColumnListGenerator()
        {
            InitializeMembers();
        }

        public SqlQueryColumnListGenerator(TableReference table)
        {
            InitializeMembers();

            this.table = table;
        }

        public SqlQueryColumnListGenerator(TableReference table, ColumnContext context)
        {
            InitializeMembers();

            this.table = table;
            this.columnContext = context;
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.tableAlias = "";
            this.joinedTableAlias = "";
            this.columnContext = ColumnContext.Default;
            this.listType = ColumnListType.ForSelectWithOriginalNameNoAlias;
            this.nullType = ColumnListNullType.Nothing;
            this.leadingComma = false;
        }

        public Dictionary<string, Column> GetColumnList()
        {
            var res = new Dictionary<string, Column>();
            var t = (TableOrView)table.DatabaseObject;

            // Primary key columns
            if ((columnContext & ColumnContext.PrimaryKey) != 0 && t.PrimaryKey != null)
            {
                foreach (var cd in t.PrimaryKey.Columns.Values)
                {
                    if (!res.ContainsKey(cd.ColumnName))
                    {
                        res.Add(cd.ColumnName, cd);
                    }
                }
            }

            // Other columns
            foreach (var cr in table.ColumnReferences)
            {
                // Avoid hint and special contexts
                if (((columnContext & cr.ColumnContext) != 0 || (columnContext & ColumnContext.NonReferenced) != 0)
                    && !res.ContainsKey(cr.ColumnName))
                {
                    res.Add(cr.ColumnName, t.Columns[cr.ColumnName]);
                }
            }

            return res;
        }

        private string GetNullString()
        {
            string nullstring;

            switch (nullType)
            {
                case ColumnListNullType.Nothing:
                    nullstring = String.Empty;
                    break;
                case ColumnListNullType.Null:
                    nullstring = " NULL";
                    break;
                case ColumnListNullType.NotNull:
                    nullstring = " NOT NULL";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return nullstring;
        }

        private string GetFormatString()
        {
            // 0: table alias
            // 1: escaped column name
            // 2: original column name
            // 3: column type
            // 4: null string

            string format = null;

            switch (listType)
            {
                case ColumnListType.ForCreateTable:
                    format = "[{1}] {3}{4}";
                    break;
                case ColumnListType.ForCreateView:
                case ColumnListType.ForInsert:
                    format = "[{1}]";
                    break;
                case ColumnListType.ForSelectWithOriginalName:
                    format = "{0}[{2}] AS [{1}]";
                    break;
                case ColumnListType.ForSelectWithEscapedName:
                    format = "{0}[{1}] AS [{1}]";
                    break;
                case ColumnListType.ForSelectWithOriginalNameNoAlias:
                    format = "{0}[{2}]";
                    break;
                case ColumnListType.ForSelectWithEscapedNameNoAlias:
                    format = "{0}[{1}]";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return format;
        }

        private string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
        }

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
            var nullstring = GetNullString();
            var format = GetFormatString();

            var columnlist = new StringBuilder();
            var columns = GetColumnList();

            foreach (var column in columns.Values)
            {
                if (leadingComma || columnlist.Length != 0)
                {
                    columnlist.Append(", ");
                }

                string alias;

                if (tableAlias == String.Empty)
                {
                    alias = String.Empty;
                }
                else if (tableAlias != null)
                {
                    alias = QuoteIdentifier(tableAlias) + ".";
                }
                else if (String.IsNullOrWhiteSpace(table.Alias))
                {
                    alias = String.Empty;
                }
                else
                {
                    alias = QuoteIdentifier(table.Alias) + ".";
                }

                columnlist.AppendFormat(
                    format,
                    alias,
                    SqlQueryCodeGenerator.EscapePropagatedColumnName(table, column.Name),
                    column.Name,
                    column.DataType.NameWithLength,
                    nullstring);
            }

            return columnlist.ToString();
        }

        public string GetJoinString()
        {
            var columnlist = new StringBuilder();
            var columns = GetColumnList();

            foreach (var column in columns.Values)
            {
                if (columnlist.Length != 0)
                {
                    columnlist.Append(" AND ");
                }

                string alias1 = QuoteIdentifier(tableAlias);
                string alias2 = QuoteIdentifier(joinedTableAlias);

                columnlist.AppendFormat(
                    "{0}.[{2}] = {1}.[{2}]",
                    alias1,
                    alias2,
                    SqlQueryCodeGenerator.EscapePropagatedColumnName(table, column.Name));
            }

            return columnlist.ToString();
        }
    }
}
