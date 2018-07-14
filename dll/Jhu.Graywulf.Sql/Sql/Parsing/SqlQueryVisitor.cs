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
    public class SqlQueryVisitor
    {
        #region Private member variables

        private SqlQueryVisitorSink sink;

        private int statementCounter;
        private Stack<Statement> statementStack;
        private Stack<QueryContext> queryContextStack;
        private Stack<TableContext> tableContextStack;
        private Stack<ColumnContext> columnContextStack;
        private CommonTableExpression commonTableExpression;
        private Stack<QuerySpecification> querySpecificationStack;

        #endregion
        #region Properties

        private SqlQueryVisitorSink Sink
        {
            get { return sink; }
        }

        public int StatementCounter
        {
            get { return statementCounter; }
        }

        public Stack<Statement> StatementStack
        {
            get { return statementStack; }
        }

        public int StatementDepth
        {
            get { return statementStack.Count; }
        }

        public Statement ParentStatement
        {
            get { return statementStack?.Peek(); }
        }

        public QueryContext QueryContext
        {
            get { return queryContextStack.Peek(); }
        }

        public TableContext TableContext
        {
            get { return tableContextStack.Peek(); }
        }

        public ColumnContext ColumnContext
        {
            get { return columnContextStack.Peek(); }
        }

        public CommonTableExpression CommonTableExpression
        {
            get { return commonTableExpression; }
        }

        public QuerySpecification ParentQuerySpecification
        {
            get { return querySpecificationStack.Count == 0 ? null : querySpecificationStack.Peek(); }
        }

        public int QuerySpecificationDepth
        {
            get { return querySpecificationStack.Count; }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryVisitor(SqlQueryVisitorSink sink)
        {
            InitializeMembers();

            this.sink = sink;
        }

        private void InitializeMembers()
        {
            this.sink = null;

            this.statementCounter = 0;
            this.statementStack = new Stack<Statement>();
            this.queryContextStack = new Stack<QueryContext>();
            this.tableContextStack = new Stack<TableContext>();
            this.columnContextStack = new Stack<ColumnContext>();
            this.commonTableExpression = null;
            this.querySpecificationStack = new Stack<QuerySpecification>();
        }

        #endregion
        #region Entry points

        public void Execute(StatementBlock node)
        {
            queryContextStack.Push(QueryContext.None);
            tableContextStack.Push(TableContext.None);
            columnContextStack.Push(ColumnContext.None);

            TraverseStatementBlock(node);

            queryContextStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        // TODO: add entry points for expressions

        #endregion
        #region Statements

        private void TraverseStatementBlock(StatementBlock node)
        {
            Sink.VisitStatementBlock(node);

            foreach (var s in node.EnumerateDescendants<AnyStatement>(true))
            {
                TraverseStatement(s.FindDescendant<Statement>());
            }
        }

        private void TraverseStatement(Statement node)
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

        protected virtual void DispatchStatement(Statement node)
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
            TraverseExpression(node.Expression);
            Sink.VisitUserVariable(node.Variable);
            Sink.VisitSetVariableStatement(node);
        }

        private void TraverseCreateTableStatement(CreateTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Create);

            TraverseTableDefinition(node.TableDefinition);
            Sink.VisitTableOrViewIdentifier(node.TargetTable);
            Sink.VisitCreateTableStatement(node);

            tableContextStack.Pop();
        }

        private void TraverseDropTableStatement(DropTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Drop);

            Sink.VisitTableOrViewIdentifier(node.TargetTable);
            Sink.VisitDropTableStatement(node);

            tableContextStack.Pop();
        }

        private void TraverseTruncateTableStatement(TruncateTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Truncate);

            Sink.VisitTableOrViewIdentifier(node.TargetTable);
            Sink.VisitTruncateTableStatement(node);

            tableContextStack.Pop();
        }

        private void TraverseCreateIndexStatement(CreateIndexStatement node)
        {
            var cds = node.IndexDefinition;
            var ics = node.IncludedColumns;

            tableContextStack.Push(TableContext | TableContext.Alter);

            Sink.VisitTableOrViewIdentifier(node.TargetTable);
            Sink.VisitIndexName(node.IndexName);
            TraverseIndexDefinition(cds);

            if (ics != null)
            {
                TraverseIncludedColumns(ics);
            }

            Sink.VisitCreateIndexStatement(node);

            tableContextStack.Pop();
        }

        private void TraverseIndexDefinition(IndexColumnDefinitionList node)
        {
            foreach (var cd in node.EnumerateDescendants<IndexColumnDefinition>())
            {
                Sink.VisitIndexColumnDefinition(cd);
            }
        }

        private void TraverseIncludedColumns(IncludedColumnList node)
        {
            foreach (var ic in node.EnumerateDescendants<IncludedColumnDefinition>())
            {
                Sink.VisitIncludedColumnDefinition(ic);
            }
        }

        private void TraverseDropIndexStatement(DropIndexStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Alter);

            Sink.VisitTableOrViewIdentifier(node.TargetTable);
            Sink.VisitIndexName(node.IndexName);
            Sink.VisitDropIndexStatement(node);

            tableContextStack.Pop();
        }

        private void TraverseSelectStatement(SelectStatement node)
        {
            queryContextStack.Push(QueryContext.SelectStatement);
            statementStack.Push(node);

            TraverseQuery(node);

            queryContextStack.Pop();
            statementStack.Pop();

            Sink.VisitSelectStatement(node);
        }

        private void TraverseInsertStatement(InsertStatement node)
        {
            queryContextStack.Push(QueryContext.InsertStatement);
            statementStack.Push(node);

            tableContextStack.Push(TableContext.Insert);

            // Target table
            TraverseTargetTableSpecification(node.TargetTable);

            // Target column list, must be traversed before any table resolution to
            // make sure these all reference the target table
            var cl = node.ColumnList;

            if (cl != null)
            {
                TraverseInsertColumnList(cl);
            }

            // Common table expression
            var cte = node.CommonTableExpression;

            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
            }

            if (cte != null)
            {
                commonTableExpression = cte;
            }

            // Values clause
            var vc = node.ValuesClause;

            if (vc != null)
            {
                TraverseValuesClause(vc);
            }

            tableContextStack.Pop();

            // Query
            var qe = node.QueryExpression;
            var orderby = node.OrderByClause;

            if (qe != null)
            {
                TraverseQueryExpression(qe);
            }

            if (orderby != null)
            {
                var qs = qe.FirstQuerySpecification;
                TraverseOrderByClause(qs, orderby);
            }

            Sink.VisitInsertStatement(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            queryContextStack.Pop();
            statementStack.Pop();
        }

        private void TraverseInsertColumnList(InsertColumnList node)
        {
            columnContextStack.Push(ColumnContext | ColumnContext.Insert);

            foreach (var column in node.EnumerateDescendants<ColumnIdentifier>())
            {
                Sink.VisitColumnIdentifier(column);
            }
        }

        private void TraverseValuesClause(ValuesClause node)
        {
            foreach (var group in node.EnumerateValueGroups())
            {
                foreach (var val in group.EnumerateValues())
                {
                    // It can be an Expression or DEFAULT, we ignore the latter
                    if (val is Expression exp)
                    {
                        TraverseExpression(exp);
                    }
                }
            }
        }

        private void TraverseDeleteStatement(DeleteStatement node)
        {
            queryContextStack.Push(QueryContext.DeleteStatement);
            statementStack.Push(node);

            // Target table
            tableContextStack.Push(TableContext.Delete);
            TraverseTargetTableSpecification(node.TargetTable);
            tableContextStack.Pop();

            // Rest of delete
            var cte = node.CommonTableExpression;
            var from = node.FromClause;
            var where = node.WhereClause;

            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
            }

            if (cte != null)
            {
                commonTableExpression = cte;
            }

            if (from != null)
            {
                TraverseFromClause(from);
            }

            if (where != null)
            {
                TraverseWhereClause(where);
            }

            Sink.VisitDeleteStatement(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            queryContextStack.Pop();
            statementStack.Pop();
        }

        private void TraverseUpdateStatement(UpdateStatement node)
        {
            queryContextStack.Push(QueryContext.DeleteStatement);
            statementStack.Push(node);

            // Target table
            tableContextStack.Push(TableContext.Update);
            TraverseTargetTableSpecification(node.TargetTable);
            tableContextStack.Pop();

            // First visit left hand side only to make sure all
            // target columns are
            TraverseUpdateSetList1(node.UpdateSetList);

            // Common table expression
            var cte = node.CommonTableExpression;
            var from = node.FromClause;
            var where = node.WhereClause;

            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
            }

            if (cte != null)
            {
                commonTableExpression = cte;
            }

            // Rest of update
            if (from != null)
            {
                TraverseFromClause(from);
            }

            if (where != null)
            {
                TraverseWhereClause(where);
            }

            // Second traversal of the set list, this time the right-had sides,
            // which can have references to all the tables
            TraverseUpdateSetList2(node.UpdateSetList);

            Sink.VisitUpdateStatement(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            queryContextStack.Pop();
            statementStack.Pop();
        }

        private void TraverseUpdateSetList1(UpdateSetList node)
        {
            columnContextStack.Push(ColumnContext | ColumnContext.Update);

            foreach (var set in node.EnumerateSetColumns())
            {
                var leftvar = set.LeftHandSide.UserVariable;
                if (leftvar != null)
                {
                    Sink.VisitUserVariable(leftvar);
                }

                var leftcol = set.LeftHandSide.ColumnIdentifier;
                if (leftcol != null)
                {
                    Sink.VisitColumnIdentifier(leftcol);
                }
            }

            columnContextStack.Pop();
        }

        private void TraverseUpdateSetList2(UpdateSetList node)
        {
            foreach (var set in node.EnumerateSetColumns())
            {
                var rightexp = set.RightHandSide.Expression;
                if (rightexp != null)
                {
                    TraverseExpression(rightexp);
                }
            }
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

            Sink.VisitDataTypeIdentifier(node.DataTypeIdentifier);

            if (exp != null)
            {
                TraverseExpression(exp);
            }

            Sink.VisitVariableDeclaration(node);
        }

        private void TraverseDeclareTableStatement(DeclareTableStatement node)
        {
            var td = node.TableDeclaration;
            TraverseTableDeclaration(td);
            Sink.VisitDeclareTableStatement(node);
        }

        private void TraverseTableDeclaration(TableDeclaration node)
        {
            var td = node.TableDefinition;

            TraverseTableDefinition(td);
            Sink.VisitTableDeclaration(node);
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

            Sink.VisitDataTypeIdentifier(node.DataTypeIdentifier);
            Sink.VisitColumnDefinition(node);
        }

        private void TraverseTableConstraint(TableConstraint node)
        {
            Sink.VisitTableConstraint(node);
        }

        private void TraverseTableIndex(TableIndex node)
        {
            Sink.VisitTableIndex(node);
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
            // TODO: consider extending this to observer operation precedence
            // during expressiont tree traversal

            // c.f. with logical expression's ExpressionVisitor class

            foreach (var n in node.Stack)
            {
                if (!(n is ExpressionSubquery) && n is Node nn)
                {
                    TraverseExpressionNodes(nn);
                }
            }

            DispatchExpressionNode(node);
        }

        protected virtual void DispatchExpressionNode(Node node)
        {
            switch (node)
            {
                case Constant n:
                    Sink.VisitConstant(n);
                    break;
                case ExpressionSubquery n:
                    Sink.VisitExpressionSubquery(n);
                    break;
                case SystemVariable n:
                    Sink.VisitSystemVariable(n);
                    break;
                case UserVariable n:
                    Sink.VisitUserVariable(n);
                    break;
                case UdtStaticMethodCall n:
                    Sink.VisitUdtStaticMethodCall(n);
                    break;
                case UdtStaticPropertyAccess n:
                    Sink.VisitUdtStaticPropertyAccess(n);
                    break;
                case CountStar n:
                    Sink.VisitCountStar(n);
                    break;
                case ColumnIdentifier n:
                    Sink.VisitColumnIdentifier(n);
                    break;
                case FunctionIdentifier n:
                    Sink.VisitFunctionIdentifier(n);
                    break;
                case UdtMethodCall n:
                    Sink.VisitUdtMethodCall(n);
                    break;
                case UdtPropertyAccess n:
                    Sink.VisitUdtPropertyAccess(n);
                    break;
            }
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
                // This needs to happen early otherwise recursive queries won't work
                Sink.VisitCommonTableSpecification(ct);

                queryContextStack.Push(QueryContext.CommonTableExpression);
                TraverseQuery(ct.Subquery);
                queryContextStack.Pop();
            }

            commonTableExpression = null;

            Sink.VisitCommonTableExpression(cte);
        }

        private void TraverseQueryExpression(QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                TraverseQuerySpecification(qs);
            }

            Sink.VisitQueryExpression(qe);
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

            Sink.VisitQuerySpecification(qs);
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
                Sink.VisitTableSourceSpecification(ts);

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        protected void DispatchTableSource(TableSource node)
        {
            switch (node)
            {
                case FunctionTableSource n:
                    Sink.VisitFunctionTableSource(n);
                    break;
                case SimpleTableSource n:
                    Sink.VisitSimpleTableSource(n);
                    break;
                case VariableTableSource n:
                    Sink.VisitVariableTableSource(n);
                    break;
                case SubqueryTableSource n:
                    Sink.VisitSubqueryTableSource(n);
                    break;
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

        private void TraverseSubquery(Subquery sq)
        {
            queryContextStack.Push(QueryContext.Subquery);

            TraverseQuery(sq);

            queryContextStack.Pop();
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
                    Sink.VisitUserVariable(var);
                }

                if (exp != null)
                {
                    TraverseExpression(exp);
                }

                if (star != null)
                {
                    TraverseStarColumnIdentifier(star);
                }

                Sink.VisitColumnExpression(ce);
            }

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseStarColumnIdentifier(StarColumnIdentifier node)
        {
            var ti = node.TableOrViewIdentifier;

            if (ti != null)
            {
                Sink.VisitTableOrViewIdentifier(ti);
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
                Sink.VisitOrderByArgument(arg);
            }

            querySpecificationStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        #endregion
        #region Node visitors

        private void TraverseTargetTableSpecification(TargetTableSpecification node)
        {
            var uv = node.Variable;
            var ti = node.TableOrViewIdentifier;

            tableContextStack.Push(TableContext | TableContext.Target);

            if (uv != null)
            {
                Sink.VisitUserVariable(uv);
            }
            else if (ti != null)
            {
                Sink.VisitTableOrViewIdentifier(ti);
            }
            else
            {
                throw new NotImplementedException();
            }

            Sink.VisitTargetTableSpecification(node);

            tableContextStack.Pop();
        }

        #endregion
    }
}
