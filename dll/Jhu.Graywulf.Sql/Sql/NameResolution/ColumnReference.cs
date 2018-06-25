using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class ColumnReference
    {
        #region Private member variables

        private TableReference tableReference;
        private DataTypeReference dataTypeReference;

        private string columnName;
        private string columnAlias;

        private bool isStar;
        private bool isComplexExpression;
        private int selectListIndex;

        private bool isResolved;
        private ColumnContext columnContext;

        #endregion
        #region Properties
        
        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
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

        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
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

        #endregion
        #region Constructors and initializers

        public ColumnReference()
        {
            InitializeMembers();
        }

        public ColumnReference(ColumnReference old)
        {
            CopyMembers(old);
        }

        public ColumnReference(TableReference tableReference, string columnName, DataTypeReference dataTypeReference)
        {
            InitializeMembers();

            this.tableReference = tableReference;
            this.dataTypeReference = dataTypeReference;
            this.columnName = columnName;
        }

        public ColumnReference(Column column, TableReference tableReference, DataTypeReference dataTypeReference)
        {
            InitializeMembers();

            this.tableReference = tableReference;
            this.dataTypeReference = dataTypeReference;

            this.columnName = column.Name;

            if (column.IsKey)
            {
                this.columnContext |= ColumnContext.PrimaryKey;
            }

            if (column.IsIdentity)
            {
                this.columnContext |= ColumnContext.Identity;
            }

            // TODO: copy metadata here
        }

        public ColumnReference(string name, Schema.DataType dataType)
        {
            InitializeMembers();

            this.columnName = name;
        }

        private void InitializeMembers()
        {
            this.tableReference = null;
            this.dataTypeReference = null;

            this.columnName = null;
            this.columnAlias = null;

            this.isStar = false;
            this.isComplexExpression = false;
            this.selectListIndex = -1;

            this.IsResolved = false;
            this.columnContext = ColumnContext.None;
        }

        private void CopyMembers(ColumnReference old)
        {
            this.tableReference = old.tableReference;
            this.dataTypeReference = old.dataTypeReference;

            this.columnName = old.columnName;
            this.columnAlias = old.columnAlias;

            this.isStar = old.isStar;
            this.isComplexExpression = old.isComplexExpression;
            this.selectListIndex = old.selectListIndex;

            this.isResolved = old.isResolved;
            this.columnContext = old.columnContext;
        }

        #endregion

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

        public static ColumnReference Interpret(ColumnDefinition cd)
        {
            var cr = new ColumnReference()
            {
                DataTypeReference = cd.DataTypeReference,
                columnName = Util.RemoveIdentifierQuotes(cd.ColumnName.Value)
            };

            return cr;
        }

        public static ColumnReference Interpret(IndexColumnDefinition ic)
        {
            var cr = new ColumnReference()
            {
                columnName = Util.RemoveIdentifierQuotes(ic.ColumnName.Value)
            };

            return cr;
        }

        public static ColumnReference Interpret(IncludedColumnDefinition ic)
        {
            var cr = new ColumnReference()
            {
                columnName = Util.RemoveIdentifierQuotes(ic.ColumnName.Value)
            };

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

            if ((this.tableReference == null || this.tableReference.IsUndefined) && !this.isComplexExpression)
            {
                // No table is specified, only compare by column name
                res &= this.CompareByName(other);
            }
            else if (other.tableReference == null || other.tableReference.IsUndefined)
            {
                // TODO: verify if this can happen
                // if this is an alias
                res &= this.tableReference == null && SchemaManager.Comparer.Compare(this.columnName, other.columnAlias) == 0;
            }
            else
            {
                // Now both have the table reference set, make sure they are equal

                // compare the two table references
                res &= (this.tableReference.Compare(other.tableReference));

                // compare the two names
                res &= this.CompareByName(other);
            }

            return res;
        }

        private bool CompareByName(ColumnReference other)
        {
            // If the other column is aliased, always compare by alias, otherwise fall back to compare by name
            if (other.columnAlias != null)
            {
                return SchemaManager.Comparer.Compare(this.columnName, other.columnAlias) == 0;
            }
            else
            {
                return SchemaManager.Comparer.Compare(this.columnName, other.columnName) == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string ToString()
        {
            var res = String.Empty;

            if (tableReference != null && !TableReference.IsUndefined)
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

            if (columnAlias != null)
            {
                res += String.Format(" AS [{0}]", columnAlias);
            }

            return res;
        }
    }
}
