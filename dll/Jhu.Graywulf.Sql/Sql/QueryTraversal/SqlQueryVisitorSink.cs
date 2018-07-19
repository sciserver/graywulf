using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    public abstract class SqlQueryVisitorSink
    {
        protected internal abstract void AcceptVisitor(SqlQueryVisitor visitor, Token node);
        
        #region Schema object references

        public virtual void Accept(IDataTypeReference node)
        {
        }

        public virtual void Accept(IVariableReference node)
        {
        }

        public virtual void Accept(IFunctionReference node)
        {
        }

        public virtual void Accept(IColumnReference node)
        {
        }

        public virtual void Accept(ITableReference node)
        {
        }

        #endregion

#if false
        #region Statements

        public virtual void Accept(StatementBlock node)
        {
        }

        public virtual void Accept(CreateTableStatement node)
        {
        }

        public virtual void Accept(DropTableStatement node)
        {
        }

        public virtual void Accept(TruncateTableStatement node)
        {
        }

        public virtual void Accept(CreateIndexStatement node)
        {
        }

        public virtual void Accept(DropIndexStatement node)
        {
        }

        public virtual void Accept(SelectStatement node)
        {
        }

        public virtual void Accept(InsertStatement node)
        {
        }

        public virtual void Accept(DeleteStatement node)
        {
        }

        public virtual void Accept(UpdateStatement node)
        {
        }

        public virtual void Accept(DeclareVariableStatement node)
        {
        }

        public virtual void Accept(DeclareTableStatement node)
        {
        }

        public virtual void Accept(SetVariableStatement node)
        {
        }

        #endregion
        #region Variable and table declarations

        public virtual void Accept(VariableDeclaration node)
        {
        }

        public virtual void Accept(TableDeclaration node)
        {
        }

        public virtual void Accept(ColumnDefinition node)
        {
        }

        public virtual void Accept(TableConstraint node)
        {
        }

        public virtual void Accept(TableIndex node)
        {
        }

        public virtual void Accept(IndexColumnDefinition node)
        {
        }

        public virtual void Accept(IncludedColumnDefinition node)
        {
        }

        #endregion
        #region Identifiers and expression elements

        public virtual void Accept(UnaryOperator node)
        {
        }

        public virtual void Accept(ArithmeticOperator node)
        {
        }

        public virtual void Accept(BitwiseOperator node)
        {
        }

        public virtual void Accept(BracketOpen node)
        {
        }

        public virtual void Accept(BracketClose node)
        {
        }

        public virtual void Accept(Constant node)
        {
        }

        public virtual void Accept(UserVariable node)
        {
        }

        public virtual void Accept(SystemVariable node)
        {
        }

        public virtual void Accept(CountStar node)
        {
        }

        public virtual void Accept(ColumnIdentifier node)
        {
        }

        public virtual void Accept(ExpressionSubquery node)
        {
        }

        public virtual void Accept(UdtPropertyAccess node)
        {
        }

        public virtual void Accept(UdtStaticPropertyAccess node)
        {
        }

        public virtual void Accept(UdtStaticMethodCall node)
        {
        }

        public virtual void Accept(UdtMethodCall node)
        {
        }

        public virtual void Accept(ScalarFunctionCall node)
        {
        }

        public virtual void Accept(TableValuedFunctionCall node)
        {
        }

        public virtual void Accept(WindowedFunctionCall node)
        {
        }

        public virtual void Accept(ArgumentListStart node)
        {
        }

        public virtual void Accept(ArgumentListEnd node)
        {
        }

        public virtual void Accept(Argument node)
        {
        }

        public virtual void Accept(PartitionByClause node)
        {
        }

        public virtual void Accept(OrderByClause node)
        {
        }

        public virtual void Accept(OrderByArgument node)
        {
        }

        public virtual void Accept(OverClause node)
        {
        }

        public virtual void Accept(SemiJoinSubquery node)
        {
        }

        // TODO: move these in different region

        public virtual void Accept(DataTypeIdentifier node)
        {
        }

        public virtual void Accept(TableOrViewIdentifier node)
        {
        }

        public virtual void Accept(FunctionIdentifier node)
        {
        }

        public virtual void Accept(MethodName node)
        {
        }

        public virtual void Accept(IndexName node)
        {
        }

        #endregion
        #region Logical expressions

        public virtual void Accept(LogicalNotOperator node)
        {
        }

        public virtual void Accept(LogicalOperator node)
        {
        }

        public virtual void Accept(ComparisonPredicate node)
        {
        }

        public virtual void Accept(LikePredicate node)
        {
        }

        public virtual void Accept(BetweenPredicate node)
        {
        }

        public virtual void Accept(IsNullPredicate node)
        {
        }

        public virtual void Accept(InExpressionListPredicate node)
        {
        }

        public virtual void Accept(InSemiJoinPredicate node)
        {
        }

        public virtual void Accept(ComparisonSemiJoinPredicate node)
        {
        }

        public virtual void Accept(ExistsSemiJoinPredicate node)
        {
        }

        #endregion
        #region Queries

        public virtual void Accept(CommonTableExpression node)
        {
        }

        public virtual void Accept(CommonTableSpecification node)
        {
        }

        public virtual void Accept(QueryExpression node)
        {
        }

        public virtual void Accept(QuerySpecification node)
        {
        }

        public virtual void Accept(ColumnExpression node)
        {
        }
        
        public virtual void Accept(TableSourceSpecification node)
        {
        }
        
        public virtual void Accept(TargetTableSpecification node)
        {
        }

        public virtual void Accept(FunctionTableSource node)
        {
        }

        public virtual void Accept(SimpleTableSource node)
        {
        }

        public virtual void Accept(VariableTableSource node)
        {
        }

        public virtual void Accept(SubqueryTableSource node)
        {
        }

        #endregion

#endif
    }
}
