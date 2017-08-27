using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.SqlCodeGen
{
    public abstract class SqlColumnListGeneratorBase
    {
        // TODO: add IDENTITY and ASC/DESC for index columns

        private const string columnNull = " NULL";
        private const string columnNotNull = " NOT NULL";

        private List<ColumnReference> columns;
        private string tableAlias;
        private string joinedTableAlias;
        private ColumnListType listType;
        private ColumnListNullType nullType;
        private bool leadingSeparator;

        public List<ColumnReference> Columns
        {
            get { return columns; }
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

        public bool LeadingSeparator
        {
            get { return leadingSeparator; }
            set { leadingSeparator = value; }
        }

        protected SqlColumnListGeneratorBase()
        {
            InitializeMembers();
        }

        protected SqlColumnListGeneratorBase(IEnumerable<ColumnReference> columns)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>(columns);
        }

        protected SqlColumnListGeneratorBase(TableOrView table)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>();

            var tr = new TableReference(table, null, true);

            foreach (var c in table.Columns.Values.OrderBy(ci => ci.ID))
            {
                var cr = new ColumnReference(tr, c);
                this.columns.Add(cr);
            }
        }

        protected SqlColumnListGeneratorBase(Index index)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>();

            var tr = new TableReference((TableOrView)index.DatabaseObject, null, true);

            foreach (var c in index.Columns.Values)
            {
                var cr = new ColumnReference(tr, c);
                this.columns.Add(cr);
            }
        }

        private void InitializeMembers()
        {
            this.columns = new List<ColumnReference>();
            this.tableAlias = null;
            this.joinedTableAlias = null;
            this.listType = ColumnListType.SelectWithOriginalNameNoAlias;
            this.nullType = ColumnListNullType.Defined;
            this.leadingSeparator = false;
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

        protected abstract string QuoteIdentifier(string identifier);

        protected virtual string GetNullString()
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

        protected virtual string GetFormatString()
        {
            // 0: table alias
            // 1: escaped column name
            // 2: original column name
            // 3: column type
            // 4: null string
            // 5: joined table alias

            string format = null;

            switch (listType)
            {
                case ColumnListType.CreateTableWithOriginalName:
                    format = "{2} {3}{4}";
                    break;
                case ColumnListType.CreateTableWithEscapedName:
                    format = "{1} {3}{4}";
                    break;
                case ColumnListType.CreateView:
                case ColumnListType.CreateIndex:
                case ColumnListType.Insert:
                    format = "{2}";
                    break;
                case ColumnListType.SelectWithOriginalName:
                    format = "{0}{2} AS {1}";
                    break;
                case ColumnListType.SelectWithEscapedName:
                    format = "{0}{1}";
                    break;
                case ColumnListType.SelectWithOriginalNameNoAlias:
                    format = "{0}{2}";
                    break;
                case ColumnListType.SelectWithEscapedNameNoAlias:
                    format = "{0}{1}";
                    break;
                case ColumnListType.JoinConditionWithOriginalName:
                    format = "{0}{2} = {5}{2}";
                    break;
                case ColumnListType.JoinConditionWithEscapedName:
                    format = "{0}{1} = {5}{1}";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return format;
        }

        protected virtual string GetSeparator()
        {
            string separator = null;

            switch (listType)
            {
                case ColumnListType.JoinConditionWithOriginalName:
                    separator = " AND ";
                    break;
                default:
                    separator = ", ";
                    break;
            }

            return separator;
        }

        private string GetTableAlias(TableReference table)
        {
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

            return alias;
        }

        private string GetJoinedTableAlias()
        {
            return QuoteIdentifier(joinedTableAlias) + ".";
        }

        private string GetNullSpec(ColumnReference column, string nullstring)
        {
            string nullspec;

            if (nullType == ColumnListNullType.Defined)
            {
                nullspec = column.DataType.IsNullable ? columnNull : columnNotNull;
            }
            else
            {
                nullspec = nullstring;
            }

            return nullspec;
        }

        /// <summary>
        /// Returns a SQL snippet with the list of primary keys
        /// and propagated columns belonging to the table.
        /// </summary>
        /// <returns>A SQL snippet with the list of columns.</returns>
        public string Execute()
        {
            var columnlist = new StringBuilder();
            Execute(columnlist);
            return columnlist.ToString();
        }

        /// <summary>
        /// Returns a SQL snippet with the list of primary keys
        /// and propagated columns belonging to the table.
        /// </summary>
        public void Execute(StringBuilder columnlist)
        {
            var nullstring = GetNullString();
            var format = GetFormatString();
            var separator = GetSeparator();

            int q = 0;
            foreach (var column in columns)
            {
                if (leadingSeparator || q != 0)
                {
                    columnlist.Append(separator);
                }

                var alias = GetTableAlias(column.TableReference);
                var jalias = GetJoinedTableAlias();
                string nullspec = GetNullSpec(column, nullstring);

                columnlist.AppendFormat(
                    format,
                    alias,
                    QuoteIdentifier(EscapePropagatedColumnName(column.TableReference, column.ColumnName)),
                    QuoteIdentifier(column.ColumnName),
                    column.DataType.TypeNameWithLength,
                    nullspec,
                    jalias);

                q++;
            }
        }

    }
}
