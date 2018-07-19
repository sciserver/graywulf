using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract class SqlQueryVisitorSink
    {
        #region Statements

        public virtual void VisitStatementBlock(StatementBlock node)
        {
        }

        public virtual void VisitCreateTableStatement(CreateTableStatement node)
        {
        }

        public virtual void VisitDropTableStatement(DropTableStatement node)
        {
        }

        public virtual void VisitTruncateTableStatement(TruncateTableStatement node)
        {
        }

        public virtual void VisitCreateIndexStatement(CreateIndexStatement node)
        {
        }

        public virtual void VisitDropIndexStatement(DropIndexStatement node)
        {
        }

        public virtual void VisitSelectStatement(SelectStatement node)
        {
        }

        public virtual void VisitInsertStatement(InsertStatement node)
        {
        }

        public virtual void VisitDeleteStatement(DeleteStatement node)
        {
        }

        public virtual void VisitUpdateStatement(UpdateStatement node)
        {
        }

        public virtual void VisitDeclareTableStatement(DeclareTableStatement node)
        {
        }

        public virtual void VisitDeclareVariableStatement(DeclareTableStatement node)
        {
        }

        public virtual void VisitSetVariableStatement(SetVariableStatement node)
        {
        }

        #endregion
        #region Variable and table declarations

        public virtual void VisitVariableDeclaration(VariableDeclaration node)
        {
        }

        public virtual void VisitTableDeclaration(TableDeclaration node)
        {
        }

        public virtual void VisitColumnDefinition(ColumnDefinition node)
        {
        }

        public virtual void VisitTableConstraint(TableConstraint node)
        {
        }

        public virtual void VisitTableIndex(TableIndex node)
        {
        }

        public virtual void VisitIndexColumnDefinition(IndexColumnDefinition node)
        {
        }

        public virtual void VisitIncludedColumnDefinition(IncludedColumnDefinition node)
        {
        }

        #endregion
        #region Identifiers and expression elements

        public virtual void VisitUnaryOperator(UnaryOperator node)
        {
        }

        public virtual void VisitArithmeticOperator(ArithmeticOperator node)
        {
        }

        public virtual void VisitBitwiseOperator(BitwiseOperator node)
        {
        }

        public virtual void VisitExpressionBracketOpen(BracketOpen node)
        {
        }

        public virtual void VisitExpressionBracketClose(BracketClose node)
        {
        }

        public virtual void VisitConstant(Constant node)
        {
        }

        public virtual void VisitUserVariable(UserVariable node)
        {
        }

        public virtual void VisitSystemVariable(SystemVariable node)
        {
        }

        public virtual void VisitCountStar(CountStar node)
        {
        }

        public virtual void VisitColumnIdentifier(ColumnIdentifier node)
        {
        }

        public virtual void VisitExpressionSubquery(ExpressionSubquery node)
        {
        }

        public virtual void VisitUdtPropertyAccess(UdtPropertyAccess node)
        {
        }

        public virtual void VisitUdtStaticPropertyAccess(UdtStaticPropertyAccess node)
        {
        }

        public virtual void VisitUdtStaticMethodCall(UdtStaticMethodCall node)
        {
        }

        public virtual void VisitUdtMethodCall(UdtMethodCall node)
        {
        }

        public virtual void VisitScalarFunctionCall(ScalarFunctionCall node)
        {
        }

        public virtual void VisitTableValuedFunctionCall(TableValuedFunctionCall node)
        {
        }

        public virtual void VisitWindowedFunctionCall(WindowedFunctionCall node)
        {
        }

        public virtual void VisitArgumentListStart(ArgumentListStart node)
        {
        }

        public virtual void VisitArgumentListEnd(ArgumentListEnd node)
        {
        }

        public virtual void VisitArgument(Argument node)
        {
        }

        public virtual void VisitPartitionByClause(PartitionByClause node)
        {
        }

        public virtual void VisitOrderByClause(OrderByClause node)
        {
        }

        public virtual void VisitOrderByArgument(OrderByArgument node)
        {
        }

        public virtual void VisitOverClause(OverClause node)
        {
        }

        public virtual void VisitSemiJoinSubquery(SemiJoinSubquery node)
        {
        }

        // TODO: move these in different region

        public virtual void VisitDataTypeIdentifier(DataTypeIdentifier node)
        {
        }

        public virtual void VisitTableOrViewIdentifier(TableOrViewIdentifier node)
        {
        }

        public virtual void VisitFunctionIdentifier(FunctionIdentifier node)
        {
        }

        public virtual void VisitMethodName(MethodName node)
        {
        }

        public virtual void VisitIndexName(IndexName node)
        {
        }

        #endregion
        #region Logical expressions

        public virtual void VisitLogicalNotOperator(LogicalNotOperator node)
        {
        }

        public virtual void VisitLogicalOperator(LogicalOperator node)
        {
        }

        public virtual void VisitComparisonPredicate(ComparisonPredicate node)
        {
        }

        public virtual void VisitLikePredicate(LikePredicate node)
        {
        }

        public virtual void VisitBetweenPredicate(BetweenPredicate node)
        {
        }

        public virtual void VisitIsNullPredicate(IsNullPredicate node)
        {
        }

        public virtual void VisitInExpressionListPredicate(InExpressionListPredicate node)
        {
        }

        public virtual void VisitInSemiJoinPredicate(InSemiJoinPredicate node)
        {
        }

        public virtual void VisitComparisonSemiJoinPredicate(ComparisonSemiJoinPredicate node)
        {
        }

        public virtual void VisitExistsSemiJoinPredicate(ExistsSemiJoinPredicate node)
        {
        }

        #endregion
        #region Queries

        public virtual void VisitCommonTableExpression(CommonTableExpression node)
        {
        }

        public virtual void VisitCommonTableSpecification(CommonTableSpecification node)
        {
        }

        public virtual void VisitQueryExpression(QueryExpression node)
        {
        }

        public virtual void VisitQuerySpecification(QuerySpecification node)
        {
        }

        public virtual void VisitColumnExpression(ColumnExpression node)
        {
        }
        
        public virtual void VisitTableSourceSpecification(TableSourceSpecification node)
        {
        }
        
        public virtual void VisitTargetTableSpecification(TargetTableSpecification node)
        {
        }

        public virtual void VisitFunctionTableSource(FunctionTableSource node)
        {
        }

        public virtual void VisitSimpleTableSource(SimpleTableSource node)
        {
        }

        public virtual void VisitVariableTableSource(VariableTableSource node)
        {
        }

        public virtual void VisitSubqueryTableSource(SubqueryTableSource node)
        {
        }

        #endregion
        #region Schema object references

        public virtual void VisitDataTypeReference(IDataTypeReference node)
        {
        }

        public virtual void VisitVariableReference(IVariableReference node)
        {
        }

        public virtual void VisitFunctionReference(IFunctionReference node)
        {
        }

        public virtual void VisitColumnReference(IColumnReference node)
        {
        }

        public virtual void VisitTableReference(ITableReference node)
        {
        }

        #endregion
    }
}
