using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

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

        public virtual void VisitTruncateTableStatement(TruncateTableStatement node)
        {
        }

        public virtual void VisitSelectStatement(SelectStatement node)
        {
        }

        public virtual void VisitDeclareTableStatement(DeclareTableStatement node)
        {
        }

        public virtual void VisitDeclareVariableStatement(DeclareTableStatement node)
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

        #endregion
        #region Identifiers and expression elements


        public virtual void VisitTableOrViewIdentifier(TableOrViewIdentifier node)
        {
        }

        public virtual void VisitDataTypeIdentifier(DataTypeIdentifier node)
        {
        }

        public virtual void VisitUserVariable(UserVariable node)
        {
        }

        public virtual void VisitSystemVariable(SystemVariable node)
        {
        }

        public virtual void VisitConstant(Constant node)
        {
        }

        public virtual void VisitExpressionSubquery(ExpressionSubquery node)
        {
        }

        public virtual void VisitUdtStaticMethodCall(UdtStaticMethodCall node)
        {
        }

        public virtual void VisitUdtMethodCall(UdtMethodCall node)
        {
        }

        public virtual void VisitUdtPropertyAccess(UdtPropertyAccess node)
        {
        }

        public virtual void VisitUdtStaticPropertyAccess(UdtStaticPropertyAccess node)
        {
        }

        public virtual void VisitCountStar(CountStar node)
        {
        }

        public virtual void VisitColumnIdentifier(ColumnIdentifier node)
        {
        }

        public virtual void VisitFunctionIdentifier(FunctionIdentifier node)
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

        public virtual void VisitOrderByArgument(OrderByArgument node)
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
    }
}
