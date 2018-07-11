using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRewriting;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract class SqlParsingTreeVisitor : ParsingTreeVisitorBase
    {
        #region Generic node visitor

        protected override bool VisitNode(Node node)
        {
            switch (node)
            {
                // Statements
                case StatementBlock n:
                    return VisitStatementBlock(n);
                case Statement n:
                    return VisitStatement(n);
                    
                // Select list
                case ColumnExpression ce:
                    return VisitColumnExpression(ce);
                case ColumnIdentifier n:
                    return VisitColumnIdentifier(n);
                case StarColumnIdentifier n:
                    return VisitStarColumnIdentifier(n);
                case ColumnName n:
                    return VisitColumnName(n);

                // Expressions
                case Expression n:
                    return VisitExpression(n);
                case Constant n:
                    return VisitConstant(n);
                case UserVariable n:
                    return VisitUserVariable(n);
                case SystemVariable n:
                    return VisitSystemVariable(n);

                // Boolean expressions
                case BooleanExpression n:
                    return VisitBooleanExpression(n);

                // Functions
                case ScalarFunctionCall n:
                    return VisitScalarFunctionCall(n);
                case TableValuedFunctionCall n:
                    return VisitTableValuedFunctionCall(n);
                case FunctionIdentifier fi:
                    return VisitFunctionIdentifier(fi);

                // Data types
                case DataTypeIdentifier dt:
                    return VisitDataTypeIdentifier(dt);

                // Subqueries
                case ExpressionSubquery n:
                    return VisitExpressionSubquery(n);
                case SemiJoinSubquery n:
                    return VisitSemiJoinSubquery(n);
                case CommonTableSubquery n:
                    return VisitCommonTableSubquery(n);
                case Subquery n:
                    return VisitSubquery(n);

                // Table identifiers
                case TableSourceIdentifier t:
                    return VisitTableSourceIdentifier(t);
                case TableOrViewIdentifier t:
                    return VisitTableOrViewIdentifier(t);
                case TableAlias ta:
                    return VisitTableAlias(ta);
                case TargetTableSpecification tts:
                    return VisitTargetTableSpecification(tts);

                // Table and index creation
                case ColumnDefinition cd:
                    return VisitColumnDefinition(cd);
                case IndexColumnDefinition cd:
                    return VisitIndexColumnDefinition(cd);

                // Magic token and default behavior
                case MagicTokenBase mt:
                    return VisitMagicToken(mt);
                default:
                    // Continue tree traversal
                    return false;
            }
        }

        #endregion
        #region Statement visitors

        protected virtual bool VisitStatementBlock(StatementBlock sb)
        {
            return false;
        }

        protected virtual bool VisitStatement(Statement statement)
        {
            var s = statement.SpecificStatement;
            bool res;

            switch (s)
            {
                case WhileStatement ss:
                    res = VisitWhileStatement(ss);
                    break;
                case ReturnStatement ss:
                    res = VisitReturnStatement(ss);
                    break;
                case IfStatement ss:
                    res = VisitIfStatement(ss);
                    break;
                case ThrowStatement ss:
                    res = VisitThrowStatement(ss);
                    break;
                case DeclareVariableStatement ss:
                    res = VisitDeclareVariableStatement(ss);
                    break;
                case DeclareTableStatement ss:
                    res = VisitDeclareTableStatement(ss);
                    break;
                case DeclareCursorStatement ss:
                    res = VisitDeclareCursorStatement(ss);
                    break;
                case SetCursorStatement ss:
                    res = VisitSetCursorStatement(ss);
                    break;
                case CursorOperationStatement ss:
                    res = VisitCursorOperationStatement(ss);
                    break;
                case FetchStatement ss:
                    res = VisitFetchStatement(ss);
                    break;
                case SetVariableStatement ss:
                    res = VisitSetVariableStatement(ss);
                    break;
                case CreateTableStatement ss:
                    res = VisitCreateTableStatement(ss);
                    break;
                case DropTableStatement ss:
                    res = VisitDropTableStatement(ss);
                    break;
                case TruncateTableStatement ss:
                    res = VisitTruncateTableStatement(ss);
                    break;
                case CreateIndexStatement ss:
                    res = VisitCreateIndexStatement(ss);
                    break;
                case DropIndexStatement ss:
                    res = VisitDropIndexStatement(ss);
                    break;
                case SelectStatement ss:
                    res = VisitSelectStatement(ss);
                    break;
                case InsertStatement ss:
                    res = VisitInsertStatement(ss);
                    break;
                case DeleteStatement ss:
                    res = VisitDeleteStatement(ss);
                    break;
                case UpdateStatement us:
                    res = VisitUpdateStatement(us);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return res;
        }

        protected virtual bool VisitWhileStatement(WhileStatement statement)
        {
            return false;
        }

        protected virtual bool VisitReturnStatement(ReturnStatement statement)
        {
            return false;
        }

        protected virtual bool VisitIfStatement(IfStatement statement)
        {
            return false;
        }

        protected virtual bool VisitThrowStatement(ThrowStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDeclareVariableStatement(DeclareVariableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDeclareTableStatement(DeclareTableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDeclareCursorStatement(DeclareCursorStatement statement)
        {
            return false;
        }

        protected virtual bool VisitSetCursorStatement(SetCursorStatement statement)
        {
            return false;
        }

        protected virtual bool VisitCursorOperationStatement(CursorOperationStatement statement)
        {
            return false;
        }

        protected virtual bool VisitFetchStatement(FetchStatement statement)
        {
            return false;
        }

        protected virtual bool VisitSetVariableStatement(SetVariableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitCreateTableStatement(CreateTableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDropTableStatement(DropTableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitTruncateTableStatement(TruncateTableStatement statement)
        {
            return false;
        }

        protected virtual bool VisitCreateIndexStatement(CreateIndexStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDropIndexStatement(DropIndexStatement statement)
        {
            return false;
        }

        protected virtual bool VisitSelectStatement(SelectStatement statement)
        {
            return false;
        }

        protected virtual bool VisitInsertStatement(InsertStatement statement)
        {
            return false;
        }

        protected virtual bool VisitDeleteStatement(DeleteStatement statement)
        {
            return false;
        }

        protected virtual bool VisitUpdateStatement(UpdateStatement statement)
        {
            return false;
        }

        #endregion
        #region Select list

        protected virtual bool VisitColumnExpression(ColumnExpression ce)
        {
            return false;
        }

        protected virtual bool VisitColumnIdentifier(ColumnIdentifier n)
        {
            return false;
        }

        protected virtual bool VisitStarColumnIdentifier(StarColumnIdentifier n)
        {
            return false;
        }

        protected virtual bool VisitColumnName(ColumnName node)
        {
            return false;
        }

        #endregion
        #region Expressions

        protected virtual bool VisitExpression(Expression expression)
        {
            return false;
        }

        protected virtual bool VisitConstant(Constant n)
        {
            return false;
        }

        protected virtual bool VisitUserVariable(UserVariable n)
        {
            return false;
        }

        protected virtual bool VisitSystemVariable(SystemVariable n)
        {
            return false;
        }

        #endregion
        #region Boolean expressions

        protected virtual bool VisitBooleanExpression(BooleanExpression expression)
        {
            return false;
        }

        #endregion
        #region Functions


        protected virtual bool VisitScalarFunctionCall(ScalarFunctionCall n)
        {
            return false;
        }

        protected virtual bool VisitTableValuedFunctionCall(TableValuedFunctionCall n)
        {
            return false;
        }

        protected virtual bool VisitFunctionIdentifier(FunctionIdentifier fi)
        {
            return false;
        }

        #endregion
        #region Data types

        protected virtual bool VisitDataTypeIdentifier(DataTypeIdentifier dt)
        {
            return false;
        }

        #endregion
        #region Subqueries
        protected virtual bool VisitExpressionSubquery(ExpressionSubquery n)
        {
            return false;
        }

        protected virtual bool VisitSemiJoinSubquery(SemiJoinSubquery n)
        {
            return false;
        }

        protected virtual bool VisitCommonTableSubquery(CommonTableSubquery n)
        {
            return false;
        }

        protected virtual bool VisitSubquery(Subquery n)
        {
            return false;
        }

        #endregion
        #region Table identifiers

        protected virtual bool VisitTableSourceIdentifier(TableSourceIdentifier t)
        {
            return false;
        }

        protected virtual bool VisitTableOrViewIdentifier(TableOrViewIdentifier t)
        {
            return false;
        }

        protected virtual bool VisitTableAlias(TableAlias ta)
        {
            return false;
        }

        protected virtual bool VisitTargetTableSpecification(TargetTableSpecification tts)
        {
            return false;
        }

        #endregion
        #region Table and index creation

        protected virtual bool VisitColumnDefinition(ColumnDefinition cd)
        {
            return false;
        }

        protected virtual bool VisitIndexColumnDefinition(IndexColumnDefinition cd)
        {
            return false;
        }

        #endregion
        #region Magic token and default behavior

        protected virtual bool VisitMagicToken(MagicTokenBase mt)
        {
            return false;
        }

        #endregion
    }
}
