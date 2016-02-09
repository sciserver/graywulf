using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser
{
    public class ColumnReference
    {
        private ColumnExpression columnExpression;
        private ColumnIdentifier columnIdentifier;
        private TableReference tableReference;

        private string columnName;
        private DataType dataType;
        private string columnAlias;

        private bool isStar;
        private bool isComplexExpression;
        private int selectListIndex;
        private ColumnContext columnContext;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public string ColumnAlias
        {
            get { return columnAlias; }
            set { columnAlias = value; }
        }

        public bool IsStar
        {
            get { return isStar; }
            set { isStar = value; }
        }

        public bool IsComplexExpression
        {
            get { return isComplexExpression; }
        }

        public int SelectListIndex
        {
            get { return selectListIndex; }
            set { selectListIndex = value; }
        }

        public ColumnContext ColumnContext
        {
            get { return columnContext; }
            set { columnContext = value; }
        }

        public bool IsReferenced
        {
            get
            {
                return columnContext != 0;
            }
        }

        public ColumnReference()
        {
            InitializeMembers();
        }

        public ColumnReference(ColumnReference old)
        {
            CopyMembers(old);
        }

        public ColumnReference(TableReference tableReference, string columnName, DataType columnType)
        {
            InitializeMembers();

            this.tableReference = tableReference;
            this.columnName = columnName;
            this.dataType = columnType;
        }

        public ColumnReference(TableReference tableReference, Column columnDescription)
        {
            InitializeMembers();

            this.tableReference = tableReference;
            this.columnName = columnDescription.Name;
            this.dataType = columnDescription.DataType;

            if (columnDescription.IsKey)
            {
                this.columnContext = Graywulf.SqlParser.ColumnContext.PrimaryKey;
            }

            // TODO: copy metadata here
        }

        public ColumnReference(string name, DataType dataType)
        {
            InitializeMembers();

            this.tableReference = null;
            this.columnName = name;
            this.dataType = dataType;
        }

        private void InitializeMembers()
        {
            this.columnExpression = null;
            this.columnIdentifier = null;
            this.tableReference = null;

            this.columnName = null;
            this.dataType = DataTypes.Unknown;
            this.columnAlias = null;

            this.isStar = false;
            this.isComplexExpression = false;
            this.selectListIndex = -1;
            this.columnContext = Graywulf.SqlParser.ColumnContext.None;
        }

        private void CopyMembers(ColumnReference old)
        {
            this.columnExpression = old.columnExpression;
            this.columnIdentifier = old.columnIdentifier;
            this.tableReference = old.tableReference;

            this.columnName = old.columnName;
            this.dataType = old.dataType;
            this.columnAlias = old.columnAlias;

            this.isStar = old.isStar;
            this.isComplexExpression = old.isComplexExpression;
            this.selectListIndex = old.selectListIndex;
            this.columnContext = old.columnContext;
        }

        public static ColumnReference CreateStar()
        {
            var cr = new ColumnReference()
            {
                IsStar = true,
                ColumnName = "*"
            };

            return cr;
        }

        public static ColumnReference CreateStar(TableReference tableReference)
        {
            var cr = CreateStar();
            cr.tableReference = tableReference;

            return cr;
        }

        public static ColumnReference Interpret(ColumnIdentifier ci)
        {
            var cr = new ColumnReference();

            cr.columnIdentifier = ci;
            cr.tableReference = new TableReference(ci);

            var star = ci.FindDescendant<Mul>();

            if (star != null)
            {
                cr.isStar = true;
                cr.columnName = star.Value;
            }
            else
            {
                cr.isStar = false;
                cr.columnName = Util.RemoveIdentifierQuotes(ci.FindDescendant<ColumnName>().Value);
            }

            cr.isComplexExpression = false;

            return cr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ce"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static ColumnReference Interpret(ColumnExpression ce)
        {
            var exp = ce.FindDescendant<Expression>();

            var cr = new ColumnReference();
            if (exp.IsSingleColumn)
            {
                cr.isComplexExpression = false;
                cr = Interpret(exp.FindDescendantRecursive<ColumnIdentifier>());
            }
            else
            {
                cr.isComplexExpression = true;
            }

            ColumnAlias ca = ce.FindDescendant<ColumnAlias>();
            if (ca != null)
            {
                cr.columnAlias = ca.Value;
            }

            cr.columnExpression = ce;

            return cr;
        }

        public bool Compare(ColumnReference other)
        {
            // other must be a direct column reference, ie having a TableReference set
            // or must be a complex expression with an alias set
            if (other.tableReference == null && !other.isComplexExpression)
            {
                throw new InvalidOperationException();
            }


            bool res = true;

            if (this.tableReference == null && !this.isComplexExpression)
            {
                // compare to a complex expression by alias

                res &= SchemaManager.Comparer.Compare(this.columnName, other.columnName) == 0 ||
                    SchemaManager.Comparer.Compare(this.columnName, other.columnAlias) == 0;
            }
            else if (other.tableReference == null)
            {
                // if this is an alias
                res &= this.tableReference == null && StringComparer.CurrentCultureIgnoreCase.Compare(this.columnName, other.columnAlias) == 0;
            }
            else
            {
                // Now both have the table reference set, make sure they are equal

                // compare the two table references
                res &= (this.tableReference.Compare(other.tableReference, true));

                // compare the two names
                res &= (StringComparer.CurrentCultureIgnoreCase.Compare(this.columnName, other.columnName) == 0);

                // either only one of them has column alias, or
                // if both do, the aliases are equat
                res &= (this.columnAlias == null || other.columnAlias == null ||
                       (StringComparer.CurrentCultureIgnoreCase.Compare(this.columnAlias, other.columnAlias) == 0));


                //      none of them have column aliases
                res &= this.columnAlias == null && other.columnAlias == null ||

                    this.columnAlias == null && other.columnAlias != null && StringComparer.CurrentCultureIgnoreCase.Compare(this.columnName, other.columnName) == 0 ||

                    this.columnAlias != null;

                // this one doesn't have an alias, so it's name must match the other name
                // **** this seems to be the tricky one
                //this.columnAlias == null && other.columnAlias != null && StringComparer.CurrentCultureIgnoreCase.Compare(other.columnAlias, other.ColumnName) == 0 ||
                // ???
                //this.columnAlias != null;
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public string ToString()
        {
            var res = String.Empty;

            if (tableReference != null)
            {
                res += tableReference.ToString();
                res += ".";
            }

            if (isStar)
            {
                res += "*";
            }
            else
            {
                res += String.Format("[{0}]", columnName);
            }

            return res;
        }
    }
}
