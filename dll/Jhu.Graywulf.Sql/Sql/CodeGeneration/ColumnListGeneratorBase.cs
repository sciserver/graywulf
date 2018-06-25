using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    public abstract class ColumnListGeneratorBase
    {
        // TODO: add IDENTITY and ASC/DESC for index columns

        private const string columnNull = " NULL";
        private const string columnNotNull = " NOT NULL";
        private const string identity = " IDENTITY(1, 1)";

        private List<ColumnReference> columns;
        private string tableAlias;
        private string joinedTableAlias;
        private ColumnListType listType;
        private ColumnListNullRendering nullRendering;
        private ColumnListSeparatorRendering separatorRendering;
        private ColumnListIdentityRendering identityRendering;

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

        public ColumnListNullRendering NullRendering
        {
            get { return nullRendering; }
            set { nullRendering = value; }
        }

        public ColumnListSeparatorRendering SeparatorRendering
        {
            get { return separatorRendering; }
            set { separatorRendering = value; }
        }

        public ColumnListIdentityRendering IdentityRendering
        {
            get { return identityRendering; }
            set { identityRendering = value; }
        }

        protected ColumnListGeneratorBase()
        {
            InitializeMembers();
        }

        protected ColumnListGeneratorBase(IEnumerable<ColumnReference> columns)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>(columns);
        }

        protected ColumnListGeneratorBase(TableOrView table)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>();

            var tr = new TableReference(table, null, true);

            foreach (var c in table.Columns.Values.OrderBy(ci => ci.ID))
            {
                var cr = new ColumnReference(c, tr, new DataTypeReference(c.DataType));
                this.columns.Add(cr);
            }
        }

        protected ColumnListGeneratorBase(Index index)
        {
            InitializeMembers();

            this.columns = new List<ColumnReference>();

            var tr = new TableReference((TableOrView)index.DatabaseObject, null, true);

            foreach (var c in index.Columns.Values)
            {
                var cr = new ColumnReference(c, tr, new DataTypeReference(c.DataType));
                this.columns.Add(cr);
            }
        }

        private void InitializeMembers()
        {
            this.columns = new List<ColumnReference>();
            this.tableAlias = null;
            this.joinedTableAlias = null;
            this.listType = ColumnListType.SelectWithOriginalNameNoAlias;
            this.nullRendering = ColumnListNullRendering.Original;
            this.separatorRendering = ColumnListSeparatorRendering.Default;
            this.identityRendering = ColumnListIdentityRendering.Original;
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
            string format;

            if (String.IsNullOrWhiteSpace(table.Alias))
            {
                format = "{0}_{1}_{2}_{4}";
            }
            else
            {
                format = "{3}_{4}";
            }

            return String.Format(format,
                                 table.DatasetName,
                                 table.SchemaName,
                                 table.DatabaseObjectName,
                                 table.Alias,
                                 columnName);
        }

        public virtual string EscapePropagatedColumnName(TableReference table, string columnName)
        {
            return String.Format("__{0}", EscapeColumnName(table, columnName));
        }

        #endregion

        protected abstract string QuoteIdentifier(string identifier);

        protected virtual string GetNullString()
        {
            string nullstring;

            switch (nullRendering)
            {
                case ColumnListNullRendering.Never:
                case ColumnListNullRendering.Original:
                    nullstring = String.Empty;
                    break;
                case ColumnListNullRendering.Null:
                    nullstring = columnNull;
                    break;
                case ColumnListNullRendering.NotNull:
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
            // 5: identity string
            // 6: joined table alias

            string format = null;

            switch (listType)
            {
                case ColumnListType.CreateTableWithOriginalName:
                    format = "{2} {3}{4}{5}";
                    break;
                case ColumnListType.CreateTableWithEscapedName:
                    format = "{1} {3}{4}{5}";
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
                    format = "{0}{2} = {6}{2}";
                    break;
                case ColumnListType.JoinConditionWithEscapedName:
                    format = "{0}{1} = {6}{1}";
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

            if (nullRendering == ColumnListNullRendering.Original)
            {
                nullspec = column.DataTypeReference.DataType.IsNullable ? columnNull : columnNotNull;
            }
            else
            {
                nullspec = nullstring;
            }

            return nullspec;
        }

        private string GetIdentitySpec(ColumnReference column)
        {
            if (identityRendering == ColumnListIdentityRendering.Original &&
                column.ColumnContext.HasFlag(ColumnContext.Identity))
            {
                return identity;
            }
            else
            {
                return String.Empty;
            }
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
                if (separatorRendering.HasFlag(ColumnListSeparatorRendering.Leading) || q != 0)
                {
                    columnlist.Append(separator);
                }

                var alias = GetTableAlias(column.ParentTableReference);
                var jalias = GetJoinedTableAlias();
                var nullspec = GetNullSpec(column, nullstring);
                var identityspec = GetIdentitySpec(column);

                columnlist.AppendFormat(
                    format,
                    alias,
                    QuoteIdentifier(EscapePropagatedColumnName(column.ParentTableReference, column.ColumnName)),
                    QuoteIdentifier(column.ColumnName),
                    // TODO: replace this with reald code generator
                    column.DataTypeReference.DataType.TypeNameWithLength,
                    nullspec,
                    identityspec,
                    jalias);

                q++;
            }
        }

    }
}
