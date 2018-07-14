using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class ColumnReference : ReferenceBase
    {
        #region Private member variables

        private TableReference tableReference;
        private DataTypeReference parentDataTypeReference;

        private List<string> nameParts;
        private int columnNamePartIndex;
        private string columnName;
        private string columnAlias;
        private DataTypeReference dataTypeReference;

        private bool isStar;
        private bool isMultiPartIdentifier;
        private bool isComplexExpression;
        private int selectListIndex;

        private ColumnContext columnContext;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the reference to the table defining the column
        /// </summary>
        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        /// <summary>
        /// Gets or sets the reference to the data type defining the column
        /// </summary>
        public DataTypeReference ParentDataTypeReference
        {
            get { return parentDataTypeReference; }
            set { parentDataTypeReference = value; }
        }

        public List<string> NameParts
        {
            get { return nameParts; }
        }

        public int ColumnNamePartIndex
        {
            get { return columnNamePartIndex; }
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

        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
        }

        public bool IsStar
        {
            get { return isStar; }
            set { isStar = value; }
        }

        public bool IsMultiPartIdentifier
        {
            get { return isMultiPartIdentifier; }
            set { isMultiPartIdentifier = value; }
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

        public override string UniqueName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion
        #region Constructors and initializers

        public ColumnReference()
        {
            InitializeMembers();
        }

        public ColumnReference(Node node)
            : base(node)
        {
        }

        public ColumnReference(ColumnReference old)
        {
            CopyMembers(old);
        }

        public ColumnReference(TableReference tableReference, ColumnReference old)
        {
            CopyMembers(old);

            this.tableReference = tableReference;
        }

        public ColumnReference(TableReference tableReference, DataTypeReference parentDataTypeReference, ColumnReference old, DataTypeReference dataTypeReference)
        {
            CopyMembers(old);

            this.tableReference = tableReference;
            this.parentDataTypeReference = parentDataTypeReference;
            this.dataTypeReference = dataTypeReference;
        }

        public ColumnReference(DataTypeReference parentDataTypeReference, ColumnReference old)
        {
            CopyMembers(old);

            this.parentDataTypeReference = parentDataTypeReference;
        }

        public ColumnReference(Node node, TableReference tableReference, string columnName, DataTypeReference dataTypeReference)
            : base(node)
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

        private void InitializeMembers()
        {
            this.tableReference = null;
            this.parentDataTypeReference = null;

            this.nameParts = null;
            this.columnNamePartIndex = -1;
            this.columnName = null;
            this.columnAlias = null;
            this.dataTypeReference = null;

            this.isStar = false;
            this.isMultiPartIdentifier = false;
            this.isComplexExpression = false;
            this.selectListIndex = -1;

            this.columnContext = ColumnContext.None;
        }

        private void CopyMembers(ColumnReference old)
        {
            this.tableReference = old.tableReference;
            this.parentDataTypeReference = old.parentDataTypeReference;

            this.nameParts = Jhu.Graywulf.Util.DeepCloner.CloneList(old.nameParts);
            this.columnNamePartIndex = old.columnNamePartIndex;
            this.columnName = old.columnName;
            this.columnAlias = old.columnAlias;
            this.dataTypeReference = old.dataTypeReference;

            this.isStar = old.isStar;
            this.isMultiPartIdentifier = old.isMultiPartIdentifier;
            this.isComplexExpression = old.isComplexExpression;
            this.selectListIndex = old.selectListIndex;

            this.columnContext = old.columnContext;
        }
        
        public override object Clone()
        {
            return new ColumnReference(this);
        }

        #endregion

        public static ColumnReference CreateStar()
        {
            var cr = new ColumnReference()
            {
                IsStar = true,
                ColumnName = "*",
                tableReference = new TableReference()
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
            // Depending on the number of parts, the column identifier can be
            // first, second or third; all the rest is property access of UDT columns
            var mpi = ci.FindDescendant<MultiPartIdentifier>();
            var cr = new ColumnReference(ci)
            {
                isMultiPartIdentifier = true
            };

            cr.nameParts = new List<string>(mpi.PartCount);
            for (int i = 0; i < mpi.NameParts.Length; i++)
            {
                cr.nameParts.Add(mpi.NameParts[i].Value);
            }

            return cr;
        }

        public static ColumnReference Interpret(StarColumnIdentifier ci)
        {
            var ti = ci.TableOrViewIdentifier;
            var cr = new ColumnReference(ci)
            {
                TableReference = ti?.TableReference ?? new TableReference(),
                isStar = true,
                columnName = "*"
            };

            return cr;
        }

        public static ColumnReference Interpret(ColumnName cn)
        {
            var cr = new ColumnReference(cn)
            {
                columnName = RemoveIdentifierQuotes(cn.Value),
            };

            return cr;
        }

        public static ColumnReference Interpret(ColumnDefinition cd)
        {
            var cr = cd.ColumnName.ColumnReference;
            cr.DataTypeReference = cd.DataTypeIdentifier.DataTypeReference;
            return cr;
        }

        public static ColumnReference Interpret(IndexColumnDefinition ic)
        {
            var cr = new ColumnReference(ic)
            {
                columnName = RemoveIdentifierQuotes(ic.ColumnName.Value)
            };

            return cr;
        }

        public static ColumnReference Interpret(IncludedColumnDefinition ic)
        {
            var cr = new ColumnReference(ic)
            {
                columnName = RemoveIdentifierQuotes(ic.ColumnName.Value)
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
            ColumnReference cr;
            var exp = ce.Expression;
            var star = ce.StarColumnIdentifier;
            var alias = ce.ColumnAlias;

            if (exp != null && exp.IsSingleColumn)
            {
                var ci = exp.FindDescendantRecursive<ColumnIdentifier>();
                cr = ci.ColumnReference;
            }
            else if (star != null)
            {
                cr = star.ColumnReference;
            }
            else
            {
                // This is a variable assignment or a complex expression
                cr = new ColumnReference(ce)
                {
                    isComplexExpression = true,
                };
            }

            cr.columnAlias = RemoveIdentifierQuotes(alias?.Value);

            return cr;
        }

        public static ColumnReference Interpret(UpdateSetMutator usm)
        {
            var cr = new ColumnReference()
            {
                columnName = usm.ColumnName.Value,
                tableReference = new TableReference(),
            };

            return cr;
        }

        public bool TryMatch(TableReference tr, ColumnReference other)
        {
            if (tr == null && !other.isComplexExpression)
            {
                // other must be a direct column reference, ie resolved and having a TableReference set
                // or must be a complex expression with an alias set
                throw new InvalidOperationException();
            }
            else if ((tableReference == null || tableReference.IsUndefined) && !isComplexExpression)
            {
                if (!String.IsNullOrWhiteSpace(columnName))
                {
                    // No table is specified, only compare by column name
                    return CompareByColumnName(columnName, other.columnName, other.columnAlias);
                }
                else if (isMultiPartIdentifier)
                {
                    // This is a real multi-part identifier with at least two parts
                    // Matching logic is as follows:
                    // 1) if first part matches column name or alias, the rest is properties
                    // 2) if first part doesn't match a column name or alias, try as table name and
                    //    then the column names must match
                    // 3) if first part doesn't match as table name, try as a schema, etc.

                    if (CompareByColumnName(nameParts[0], other.columnName, other.columnAlias))
                    {
                        // 1)
                        columnNamePartIndex = 0;
                        return true;
                    }
                    else if (
                        nameParts.Count > 1 &&
                        CompareByTableName(nameParts[0], tr.TableName, tr.Alias) &&
                        CompareByColumnName(nameParts[1], other.columnName, other.columnAlias))
                    {
                        // 2)
                        columnNamePartIndex = 1;
                        return true;
                    }
                    else if (
                        nameParts.Count > 2 &&
                        CompareByTableName(nameParts[0], nameParts[1], tr.SchemaName, tr.TableName, tr.Alias) &&
                        CompareByColumnName(nameParts[2], other.columnName, other.columnAlias))
                    {
                        // 3)
                        columnNamePartIndex = 2;
                        return true;
                    }
                    else
                    {
                        // Multi-part identifier cannot be bound
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else if (tr == null || tr.IsUndefined)
            {
                throw new NotImplementedException();

                // TODO: verify if this can happen
                // if this is an alias

                //res &= this.tableReference == null && SchemaManager.Comparer.Compare(this.columnName, other.columnAlias) == 0;
            }
            else
            {
                if (isMultiPartIdentifier)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    // Now both have the table reference set, make sure they are equal
                    if (tableReference.TryMatch(tr) &&
                    CompareByColumnName(columnName, other.columnName, other.columnAlias))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private static bool CompareByColumnName(string columnName, string otherName, string otherAlias)
        {
            // If the other column is aliased, always compare by alias, otherwise fall back to compare by name
            if (otherAlias != null)
            {
                return SchemaManager.Comparer.Compare(columnName, otherAlias) == 0;
            }
            else
            {
                return SchemaManager.Comparer.Compare(columnName, otherName) == 0;
            }
        }

        private static bool CompareByTableName(string tableName, string otherName, string otherAlias)
        {
            // If the other table is aliased, always compare by alias, otherwise fall back to compare by name
            if (otherAlias != null)
            {
                return SchemaManager.Comparer.Compare(tableName, otherAlias) == 0;
            }
            else
            {
                return SchemaManager.Comparer.Compare(tableName, otherName) == 0;
            }
        }

        private bool CompareByTableName(string schemaName, string tableName, string otherSchema, string otherName, string otherAlias)
        {
            // If other is aliased matching by schema is not possible. Otherwise match both schame and table name
            if (otherAlias != null)
            {
                return false;
            }
            else
            {
                return
                    SchemaManager.Comparer.Compare(schemaName, otherSchema) == 0 &&
                    SchemaManager.Comparer.Compare(tableName, otherName) == 0;
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
