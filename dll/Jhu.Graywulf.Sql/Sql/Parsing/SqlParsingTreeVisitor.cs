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
            bool res = false;

            switch (node)
            {
                // Statement
                case StatementBlock n:
                    res = VisitStatementBlock(n);
                    break;

                // Select list
                case ColumnExpression ce:
                    res = VisitColumnExpression(ce);
                    break;
                case ColumnIdentifier n:
                    res = VisitColumnIdentifier(n);
                    break;
                case StarColumnIdentifier n:
                    res = VisitStarColumnIdentifier(n);
                    break;
                case ColumnName n:
                    res = VisitColumnName(n);
                    break;

                // Expressions
                case Expression n:
                    res = VisitExpression(n);
                    break;
                case Constant n:
                    res = VisitConstant(n);
                    break;
                case UserVariable n:
                    res = VisitUserVariable(n);
                    break;
                case SystemVariable n:
                    res = VisitSystemVariable(n);
                    break;

                // Boolean expressions
                case BooleanExpression n:
                    VisitBooleanExpression(n);
                    break;

                // Functions
                case ScalarFunctionCall n:
                    res = VisitScalarFunctionCall(n);
                    break;
                case TableValuedFunctionCall n:
                    res = VisitTableValuedFunctionCall(n);
                    break;
                case FunctionIdentifier fi:
                    res = VisitFunctionIdentifier(fi);
                    break;

                // Data types
                case DataTypeIdentifier dt:
                    res = VisitDataTypeIdentifier(dt);
                    break;

                // Subqueries
                case ExpressionSubquery n:
                    res = VisitExpressionSubquery(n);
                    break;
                case SemiJoinSubquery n:
                    res = VisitSemiJoinSubquery(n);
                    break;
                case CommonTableSubquery n:
                    res = VisitCommonTableSubquery(n);
                    break;
                case Subquery n:
                    res = VisitSubquery(n);
                    break;

                // Table identifiers
                case TableSourceIdentifier t:
                    res = VisitTableSourceIdentifier(t);
                    break;
                case TableOrViewIdentifier t:
                    res = VisitTableOrViewIdentifier(t);
                    break;
                case TableAlias ta:
                    res = VisitTableAlias(ta);
                    break;
                case TargetTableSpecification tts:
                    res = VisitTargetTableSpecification(tts);
                    break;

                // Table and index creation
                case ColumnDefinition cd:
                    res = VisitColumnDefinition(cd);
                    break;
                case IndexColumnDefinition cd:
                    res = VisitIndexColumnDefinition(cd);
                    break;

                // Magic token and default behavior
                case MagicTokenBase mt:
                    res = VisitMagicToken(mt);
                    break;
                default:
                    // Continue tree traversal
                    return false;
            }

            return res;
        }

        #endregion
        #region Statement visitors

        /// <summary>
        /// Visit statements of the script sequentially. This is fine because SQL variables
        /// are not scoped.
        /// </summary>
        /// <param name="statementBlock"></param>
        protected virtual bool VisitStatementBlock(StatementBlock statementBlock)
        {
            foreach (var statement in statementBlock.EnumerateDescendants<Statement>(true))
            {
                VisitStatement(statement);
            }

            return true;
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

            // If node is not handled, descend using generic node visitor 
            if (!res)
            {
                return VisitNode(statement);
            }
            else
            {
                return true;
            }
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
