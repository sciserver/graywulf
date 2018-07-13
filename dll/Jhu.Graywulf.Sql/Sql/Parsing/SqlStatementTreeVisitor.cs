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
    /// <summary>
    /// Implements a generic SQL tree traversal algorithm to be used with name
    /// resolution and query rewriting
    /// </summary>
    public abstract class SqlStatementTreeVisitor
    {
        #region Private member variables

        private int statementCounter;
        private Stack<Statement> statementStack;
        private Stack<QueryContext> queryContextStack;
        private Stack<TableContext> tableContextStack;
        private Stack<ColumnContext> columnContextStack;
        private CommonTableExpression commonTableExpression;
        private Stack<QuerySpecification> querySpecificationStack;

        #endregion
        #region Properties

        protected int StatementCounter
        {
            get { return statementCounter; }
        }

        protected Stack<Statement> StatementStack
        {
            get { return statementStack; }
        }

        protected int StatementDepth
        {
            get { return statementStack.Count; }
        }

        protected Statement ParentStatement
        {
            get { return statementStack?.Peek(); }
        }

        protected QueryContext QueryContext
        {
            get { return queryContextStack.Peek(); }
        }

        protected TableContext TableContext
        {
            get { return tableContextStack.Peek(); }
        }

        protected ColumnContext ColumnContext
        {
            get { return columnContextStack.Peek(); }
        }

        protected CommonTableExpression CommonTableExpression
        {
            get { return commonTableExpression; }
        }

        protected QuerySpecification ParentQuerySpecification
        {
            get { return querySpecificationStack.Count == 0 ? null : querySpecificationStack.Peek(); }
        }

        protected int QuerySpecificationDepth
        {
            get { return querySpecificationStack.Count; }
        }

        #endregion
        #region Constructors and initializers

        protected SqlStatementTreeVisitor()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.statementCounter = 0;
            this.statementStack = new Stack<Statement>();
            this.queryContextStack = new Stack<QueryContext>();
            this.tableContextStack = new Stack<TableContext>();
            this.columnContextStack = new Stack<ColumnContext>();
            this.commonTableExpression = null;
            this.querySpecificationStack = new Stack<QuerySpecification>();
        }

        #endregion
        #region Statements

        protected void TraverseStatements(StatementBlock node)
        {
            queryContextStack.Push(QueryContext.None);
            tableContextStack.Push(TableContext.None);
            columnContextStack.Push(ColumnContext.None);

            TraverseStatementBlock(node);

            queryContextStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseStatementBlock(StatementBlock node)
        {
            var res = VisitStatementBlock(node);

            if (!res)
            {
                foreach (var s in node.EnumerateDescendants<AnyStatement>(true))
                {
                    TraverseStatement(s.FindDescendant<Statement>());
                }
            }
        }

        protected virtual bool VisitStatementBlock(StatementBlock node)
        {
            return false;
        }

        protected void TraverseStatement(Statement node)
        {
            statementStack.Push(node);

            DispatchStatement(node);
            statementCounter++;

            // Call recursively for sub-statements
            foreach (var ss in node.EnumerateSubStatements())
            {
                TraverseStatement(ss);
            }

            statementStack.Pop();
        }

        private void DispatchStatement(Statement node)
        {
            switch (node)
            {
                case WhileStatement n:
                    TraverseWhileStatement(n);
                    break;
                case ReturnStatement n:
                    TraverseReturnStatement(n);
                    break;
                case IfStatement n:
                    TraverseIfStatement(n);
                    break;
                case ThrowStatement n:
                    TraverseThrowStatement(n);
                    break;
                case DeclareVariableStatement n:
                    TraverseDeclareVariableStatement(n);
                    break;
                case DeclareTableStatement n:
                    TraverseDeclareTableStatement(n);
                    break;
                case DeclareCursorStatement n:
                    TraverseDeclareCursorStatement(n);
                    break;
                case SetCursorStatement n:
                    TraverseSetCursorStatement(n);
                    break;
                case CursorOperationStatement n:
                    TraverseCursorOperationStatement(n);
                    break;
                case FetchStatement n:
                    TraverseFetchStatement(n);
                    break;
                case SetVariableStatement n:
                    TraverseSetVariableStatement(n);
                    break;
                case CreateTableStatement n:
                    TraverseCreateTableStatement(n);
                    break;
                case DropTableStatement n:
                    TraverseDropTableStatement(n);
                    break;
                case TruncateTableStatement n:
                    TraverseTruncateTableStatement(n);
                    break;
                case CreateIndexStatement n:
                    TraverseCreateIndexStatement(n);
                    break;
                case DropIndexStatement n:
                    TraverseDropIndexStatement(n);
                    break;
                case SelectStatement n:
                    TraverseSelectStatement(n);
                    break;
                case InsertStatement n:
                    TraverseInsertStatement(n);
                    break;
                case DeleteStatement n:
                    TraverseDeleteStatement(n);
                    break;
                case UpdateStatement n:
                    TraverseUpdateStatement(n);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TraverseWhileStatement(WhileStatement node)
        {
            TraverseBooleanExpression(node.Condition);
        }

        private void TraverseReturnStatement(ReturnStatement node)
        {
            // it might have a query in the parameter
            // do we support functions or stored procedures?
            throw new NotImplementedException();
        }

        private void TraverseIfStatement(IfStatement node)
        {
            TraverseBooleanExpression(node.Condition);
        }

        private void TraverseThrowStatement(ThrowStatement node)
        {
            // TODO: Resolve variables
            throw new NotImplementedException();
        }

        private void TraverseDeclareCursorStatement(DeclareCursorStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseSetCursorStatement(SetCursorStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseCursorOperationStatement(CursorOperationStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseFetchStatement(FetchStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseSetVariableStatement(SetVariableStatement node)
        {
            VisitUserVariable(node.Variable);
        }

        private void TraverseCreateTableStatement(CreateTableStatement node)
        {
            TraverseTableDefinition(node.TableDefinition);
            VisitTargetTableIdentifier(node.TargetTable);
            VisitCreateTableStatement(node);
        }

        protected bool VisitCreateTableStatement(CreateTableStatement node)
        {
            return false;
        }

        private void TraverseDropTableStatement(DropTableStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseTruncateTableStatement(TruncateTableStatement node)
        {
            // TODO: grammar szinten ketté kéne szedni a kimeneti és bemeneti táblákat

            VisitTargetTableIdentifier(node.TargetTable);
            VisitTruncateTableStatement(node);
        }

        protected virtual bool VisitTruncateTableStatement(TruncateTableStatement node)
        {
            return false;
        }


        private void TraverseCreateIndexStatement(CreateIndexStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseDropIndexStatement(DropIndexStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseSelectStatement(SelectStatement node)
        {
            queryContextStack.Push(QueryContext.SelectStatement);
            statementStack.Push(node);

            TraverseQuery(node);

            queryContextStack.Pop();
            statementStack.Pop();

            VisitSelectStatement(node);
        }

        protected virtual bool VisitSelectStatement(SelectStatement node)
        {
            return false;
        }

        private void TraverseInsertStatement(InsertStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseDeleteStatement(DeleteStatement node)
        {
            throw new NotImplementedException();
        }

        private void TraverseUpdateStatement(UpdateStatement node)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Declarations

        private void TraverseDeclareVariableStatement(DeclareVariableStatement node)
        {
            foreach (var vd in node.FindDescendant<VariableDeclarationList>().EnumerateDescendants<VariableDeclaration>())
            {
                TraverseVariableDeclaration(vd);
            }
        }

        private void TraverseVariableDeclaration(VariableDeclaration node)
        {
            var dt = node.DataTypeIdentifier;
            var exp = node.Expression;

            VisitDataTypeIdentifier(node.DataTypeIdentifier);

            if (exp != null)
            {
                TraverseExpression(exp);
            }

            VisitVariableDeclaration(node);
        }

        protected virtual bool VisitVariableDeclaration(VariableDeclaration node)
        {
            return false;
        }

        private void TraverseDeclareTableStatement(DeclareTableStatement node)
        {
            var td = node.TableDeclaration;
            TraverseTableDeclaration(td);
            VisitDeclareTableStatement(node);
        }

        private void TraverseTableDeclaration(TableDeclaration node)
        {
            var td = node.TableDefinition;

            TraverseTableDefinition(td);
            VisitTableDeclaration(node);
        }
    

        protected virtual bool VisitDeclareTableStatement(DeclareTableStatement node)
        {
            return false;
        }

        protected virtual bool VisitTableDeclaration(TableDeclaration node)
        {
            return false;
        }

        #endregion
        #region DDL statements

        private void TraverseTableDefinition(TableDefinition node)
        {
            foreach (var tdi in node.TableDefinitionList.EnumerateTableDefinitionItems())
            {
                TraverseTableDefinitionItem(tdi);
            }
        }

        private void TraverseTableDefinitionItem(TableDefinitionItem node)
        {
            var cd = node.ColumnDefinition;
            var tc = node.TableConstraint;
            var ti = node.TableIndex;

            if (cd != null)
            {
                TraverseColumnDefinition(cd);
            }
        }

        private void TraverseColumnDefinition(ColumnDefinition node)
        {
            var exp = node.DefaultDefinition?.Expression;

            if (exp != null)
            {
                TraverseExpression(exp);
            }

            VisitDataTypeIdentifier(node.DataTypeIdentifier);
            VisitColumnDefinition(node);
        }

        private void TraverseTableConstraint(TableConstraint node)
        {
            VisitTableConstraint(node);
        }

        private void TraverseTableIndex(TableIndex node)
        {
            VisitTableIndex(node);
        }

        protected virtual bool VisitTableDefinition(TableDefinition node)
        {
            return false;
        }

        protected virtual bool VisitColumnDefinition(ColumnDefinition node)
        {
            return false;
        }

        protected virtual bool VisitTableConstraint(TableConstraint node)
        {
            return false;
        }

        protected virtual bool VisitTableIndex(TableIndex node)
        {
            return false;
        }

        #endregion
        #region Expressions

        protected void TraverseExpression(Expression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            TraverseExpressionSubqueries(node);
            TraverseExpressionNodes(node);
        }

        protected void TraverseBooleanExpression(BooleanExpression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            TraverseExpressionSubqueries(node);
            TraverseExpressionNodes(node);
        }

        private void TraverseExpressionSubqueries(Node node)
        {
            foreach (var n in node.Stack)
            {
                if (n is Subquery sq)
                {
                    TraverseSubquery(sq);
                }
                else if (n is Node nn)
                {
                    TraverseExpressionSubqueries(nn);
                }
            };
        }

        private void TraverseExpressionNodes(Node node)
        {
            foreach (var n in node.Stack)
            {
                if (!(n is Subquery) && n is Node nn)
                {
                    TraverseExpressionNodes(nn);
                }
            }

            DispatchExpressionNode(node);
        }

        private bool DispatchExpressionNode(Node node)
        {
            switch (node)
            {
                case SystemVariable n:
                    return VisitSystemVariable(n);
                case UserVariable n:
                    return VisitUserVariable(n);
                case UdtStaticMethodCall n:
                    return VisitUdtStaticMethodCall(n);
                case UdtStaticPropertyAccess n:
                    return VisitUdtStaticPropertyAccess(n);
                case WindowedFunctionCall n:
                    return VisitWindowedFunctionCall(n);
                case ScalarFunctionCall n:
                    return VisitScalarFunctionCall(n);
                case CountStar n:
                    return VisitCountStar(n);
                case DataTypeIdentifier n:
                    return VisitDataTypeIdentifier(n);
                case FunctionIdentifier n:
                    return VisitFunctionIdentifier(n);
                case ColumnIdentifier n:
                    return VisitColumnIdentifier(n);

                // TODO: UDT member list

                default:
                    return false;
            }
        }

        protected virtual bool VisitSystemVariable(SystemVariable node)
        {
            return false;
        }

        protected virtual bool VisitUserVariable(UserVariable node)
        {
            return false;
        }

        protected virtual bool VisitUdtStaticMethodCall(UdtStaticMethodCall node)
        {
            return false;
        }

        protected virtual bool VisitUdtStaticPropertyAccess(UdtStaticPropertyAccess node)
        {
            return false;
        }

        protected virtual bool VisitWindowedFunctionCall(WindowedFunctionCall node)
        {
            return false;
        }

        protected virtual bool VisitScalarFunctionCall(ScalarFunctionCall node)
        {
            return false;
        }

        protected virtual bool VisitCountStar(CountStar node)
        {
            return false;
        }

        protected virtual bool VisitTableSourceIdentifier(TableOrViewIdentifier node)
        {
            return false;
        }

        protected virtual bool VisitDataTypeIdentifier(DataTypeIdentifier node)
        {
            return false;
        }

        protected virtual bool VisitFunctionIdentifier(FunctionIdentifier node)
        {
            return false;
        }
        
        protected virtual bool VisitColumnIdentifier(ColumnIdentifier node)
        {
            return false;
        }

        #endregion
        #region Queries

        protected void TraverseQuery(Node node)
        {
            var cte = node.FindDescendant<CommonTableExpression>();
            var qe = node.FindDescendant<QueryExpression>();
            var orderby = node.FindDescendant<OrderByClause>();

            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
            }

            if (cte != null)
            {
                commonTableExpression = cte;
            }

            TraverseQueryExpression(qe);

            if (orderby != null)
            {
                var firstqs = qe.FirstQuerySpecification;
                TraverseOrderByClause(firstqs, orderby);
            }

            if (cte != null)
            {
                commonTableExpression = null;
            }
        }

        private void TraverseCommonTableExpression(CommonTableExpression cte)
        {
            commonTableExpression = cte;

            foreach (var ct in cte.EnumerateCommonTableSpecifications())
            {
                // This needs to happen early otherwise recursive queris won't work
                VisitCommonTableSpecification(ct);

                queryContextStack.Push(QueryContext.CommonTableExpression);
                TraverseQuery(ct.Subquery);
                queryContextStack.Pop();
            }

            commonTableExpression = null;

            VisitCommonTableExpression(cte);
        }

        private void TraverseQueryExpression(QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                TraverseQuerySpecification(qs);
            }

            VisitQueryExpression(qe);
        }

        private void TraverseQuerySpecification(QuerySpecification qs)
        {
            var into = qs.IntoClause;
            var from = qs.FromClause;
            var sl = qs.SelectList;
            var where = qs.WhereClause;
            var groupby = qs.GroupByClause;
            var having = qs.HavingClause;

            querySpecificationStack.Push(qs);

            // TODO: make this a dynamic dispatch so it can be extended
            // when the grammar is extended

            if (from != null)
            {
                TraverseFromClause(from);
            }

            if (sl != null)
            {
                TraverseSelectList(sl);
            }

            if (where != null)
            {
                TraverseWhereClause(where);
            }

            if (groupby != null)
            {
                TraverseGroupByClause(groupby);
            }

            if (having != null)
            {
                TraverseHavingClause(having);
            }

            // This needs to be done last
            if (into != null)
            {
                TraverseIntoClause(into);
            }

            querySpecificationStack.Pop();

            VisitQuerySpecification(qs);
        }

        private void TraverseFromClause(FromClause node)
        {
            tableContextStack.Push(TableContext | TableContext.From);
            columnContextStack.Push(ColumnContext | ColumnContext.From);

            var tse = node.FindDescendant<TableSourceExpression>();

            if (tse != null)
            {
                TraverseTableSourceExpression(tse);
            }

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseTableSourceExpression(TableSourceExpression node)
        {
            TraverseSubqueryTableSources(node);
            TraverseTableSources(node);
            TraverseJoinConditions(node);
        }

        private void TraverseSubqueryTableSources(TableSourceExpression node)
        {
            var ts = node.FindDescendant<TableSourceSpecification>();
            var jt = node.FindDescendant<JoinedTable>();

            // Traverse subquery table sources first, and only in a next step visit all table sources
            while (ts != null)
            {
                if (ts.SpecificTableSource is SubqueryTableSource sq)
                {
                    TraverseSubquery(sq.Subquery);
                }

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        private void TraverseTableSources(TableSourceExpression node)
        {
            var ts = node.FindDescendant<TableSourceSpecification>();
            var jt = node.FindDescendant<JoinedTable>();

            while (ts != null)
            {
                DispatchTableSource(ts.SpecificTableSource);
                VisitTableSource(ts.SpecificTableSource);

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        private void TraverseJoinConditions(TableSourceExpression node)
        {
            var ts = node.FindDescendant<TableSourceSpecification>();
            var jt = node.FindDescendant<JoinedTable>();

            while (ts != null)
            {
                var jc = jt?.FindDescendant<BooleanExpression>();

                if (jc != null)
                {
                    columnContextStack.Push(ColumnContext | ColumnContext.JoinOn);

                    TraverseBooleanExpression(jc);

                    columnContextStack.Pop();
                }

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        private void DispatchTableSource(TableSource node)
        {
            switch (node)
            {
                case FunctionTableSource n:
                    VisitFunctionTableSource(n);
                    break;
                case SimpleTableSource n:
                    VisitSimpleTableSource(n);
                    break;
                case VariableTableSource n:
                    VisitVariableTableSource(n);
                    break;
                case SubqueryTableSource n:
                    VisitSubqueryTableSouce(n);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TraverseSubquery(Subquery sq)
        {
            queryContextStack.Push(QueryContext.Subquery);

            TraverseQuery(sq);

            queryContextStack.Pop();
        }

        protected virtual bool VisitTableSource(TableSource ts)
        {
            return false;
        }

        private void TraverseSelectList(SelectList node)
        {
            tableContextStack.Push(TableContext | TableContext.SelectList);
            columnContextStack.Push(ColumnContext | ColumnContext.SelectList);

            foreach (var ce in node.EnumerateColumnExpressions())
            {
                var var = ce.AssignedVariable;
                var exp = ce.Expression;
                var star = ce.StarColumnIdentifier;

                if (var != null)
                {
                    VisitUserVariable(var);
                }

                if (exp != null)
                {
                    TraverseExpression(exp);
                }

                if (star != null)
                {
                    TraverseStarColumnIdentifier(star);
                }

                VisitColumnExpression(ce);
            }

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseStarColumnIdentifier(StarColumnIdentifier node)
        {
            var ti = node.TableOrViewIdentifier;

            if (ti != null)
            {
                VisitTableSourceIdentifier(ti);
            }
        }

        private void TraverseIntoClause(IntoClause node)
        {
            tableContextStack.Push(TableContext | TableContext.Into);

            TraverseTargetTableSpecification(node.TargetTable);

            tableContextStack.Pop();
        }

        private void TraverseWhereClause(WhereClause node)
        {
            tableContextStack.Push(TableContext | TableContext.Where);
            columnContextStack.Push(ColumnContext | ColumnContext.Where);

            var exp = node.FindDescendant<BooleanExpression>();
            TraverseBooleanExpression(exp);

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseGroupByClause(GroupByClause node)
        {
            tableContextStack.Push(TableContext | TableContext.GroupBy);
            columnContextStack.Push(ColumnContext | ColumnContext.GroupBy);

            var gbl = node.FindDescendant<GroupByList>();

            foreach (var exp in gbl.EnumerateDescendants<Expression>())
            {
                TraverseExpression(exp);
            }

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseHavingClause(HavingClause node)
        {
            tableContextStack.Push(TableContext | TableContext.Having);
            columnContextStack.Push(ColumnContext | ColumnContext.Having);

            var be = node.FindDescendant<BooleanExpression>();
            TraverseBooleanExpression(be);

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseOrderByClause(QuerySpecification qs, OrderByClause node)
        {
            querySpecificationStack.Push(qs);
            tableContextStack.Push(TableContext | TableContext.OrderBy);
            columnContextStack.Push(ColumnContext | ColumnContext.OrderBy);

            var obl = node.FindDescendant<OrderByList>();
            foreach (var arg in obl.EnumerateDescendants<OrderByArgument>())
            {
                TraverseExpression(arg.Expression);
            }

            VisitOrderByClause(node);

            querySpecificationStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        protected virtual bool VisitCommonTableExpression(CommonTableExpression node)
        {
            return false;
        }

        protected virtual bool VisitCommonTableSpecification(CommonTableSpecification cts)
        {
            return false;
        }

        protected virtual bool VisitSubquery(Subquery node)
        {
            return false;
        }

        protected virtual bool VisitQueryExpression(QueryExpression qe)
        {
            return false;
        }

        protected virtual bool VisitQuerySpecification(QuerySpecification qs)
        {
            return false;
        }

        protected virtual bool VisitOrderByClause(OrderByClause orderBy)
        {
            return false;
        }

        protected virtual bool VisitColumnExpression(ColumnExpression ce)
        {
            return false;
        }

        protected virtual bool VisitSimpleTableSource(SimpleTableSource node)
        {
            return false;
        }

        protected virtual bool VisitFunctionTableSource(FunctionTableSource node)
        {
            return false;
        }

        protected virtual bool VisitVariableTableSource(VariableTableSource node)
        {
            return false;
        }

        protected virtual bool VisitSubqueryTableSouce(SubqueryTableSource node)
        {
            return false;
        }

        #endregion
        #region Node visitors

        private void TraverseTargetTableSpecification(TargetTableSpecification node)
        {
            var uv = node.Variable;
            var ti = node.TableOrViewIdentifier;

            if (uv != null)
            {
                VisitUserVariable(uv);
            }
            else if (ti != null)
            {
                VisitTargetTableIdentifier(ti);
            }
            else
            {
                throw new NotImplementedException();
            }

            VisitTargetTableSpecification(node);
        }

        protected virtual void VisitTargetTableSpecification(TargetTableSpecification node)
        {
        }

        protected virtual bool VisitTargetTableIdentifier(TableOrViewIdentifier node)
        {
            return false;
        }

        #endregion
    }
}
