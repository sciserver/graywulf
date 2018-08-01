using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    /// <summary>
    /// Implements a generic SQL tree traversal algorithm to be used with name
    /// resolution and query rewriting
    /// </summary>
    public class SqlQueryVisitor
    {
        #region Private member variables

        private SqlQueryVisitorOptions options;
        private SqlQueryVisitorSink sink;

        private int statementCounter;
        private Stack<Statement> statementStack;
        private Stack<QueryContext> queryContextStack;
        private Stack<TableContext> tableContextStack;
        private Stack<ColumnContext> columnContextStack;
        private CommonTableExpression commonTableExpression;
        private Stack<QuerySpecification> querySpecificationStack;
        private Stack<ExpressionReshuffler> expressionReshufflerStack;
        private Stack<TokenList> memberAccessListStack;

        #endregion
        #region Properties

        public SqlQueryVisitorOptions Options
        {
            get { return options; }
            set { options = value; }
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

        public Statement CurrentStatement
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

        public QuerySpecification CurrentQuerySpecification
        {
            get { return querySpecificationStack.Count == 0 ? null : querySpecificationStack.Peek(); }
        }

        public int QuerySpecificationDepth
        {
            get { return querySpecificationStack.Count; }
        }

        private ExpressionReshuffler ExpressionReshuffler
        {
            get { return expressionReshufflerStack.Count == 0 ? null : expressionReshufflerStack.Peek(); }
        }

        public TokenList CurrentMemberAccessList
        {
            get { return memberAccessListStack.Peek(); }
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
            this.options = new SqlQueryVisitorOptions();
            this.sink = null;

            this.statementCounter = 0;
            this.statementStack = new Stack<Statement>();
            this.queryContextStack = new Stack<QueryContext>();
            this.tableContextStack = new Stack<TableContext>();
            this.columnContextStack = new Stack<ColumnContext>();
            this.commonTableExpression = null;
            this.querySpecificationStack = new Stack<QuerySpecification>();
            this.expressionReshufflerStack = new Stack<ExpressionReshuffler>();
            this.memberAccessListStack = new Stack<TokenList>();
        }

        #endregion
        #region Entry points

        public void Execute(StatementBlock node)
        {
            PushAllContextNone();
            TraverseStatementBlock(node);
            PopAllContext();
        }

        // TODO: add entry points for expressions

        public void Execute(Expression node)
        {
            PushAllContextNone();
            TraverseExpression(node);
            PopAllContext();
        }

        public void Execute(LogicalExpression node)
        {
            PushAllContextNone();
            TraverseLogicalExpression(node);
            PopAllContext();
        }

        public void Execute(Predicate node)
        {
            PushAllContextNone();
            TraversePredicate(node);
            PopAllContext();
        }

        private void PushAllContextNone()
        {
            queryContextStack.Push(QueryContext.None);
            tableContextStack.Push(TableContext.None);
            columnContextStack.Push(ColumnContext.None);
        }

        private void PopAllContext()
        {
            queryContextStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        #endregion
        #region Common node travelsal and reshuffler dispatch

        private IEnumerable<Token> SelectDirection(TokenList tokens)
        {
            var reshuffler = ExpressionReshuffler;

            if (reshuffler == null)
            {
                return tokens.Forward;
            }
            else if (reshuffler.Direction == TraversalDirection.Forward)
            {
                return tokens.Forward;
            }
            else if (reshuffler.Direction == TraversalDirection.Backward)
            {
                return tokens.Backward;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual void VisitNode(Token node)
        {
            // Depending on the traversal mode, we either route nodes to the sink
            // directly or we reshuffle to produce postfix notation for expression
            // tree building

            var reshuffler = ExpressionReshuffler;

            if (reshuffler != null)
            {
                reshuffler.Route(node);
            }
            else
            {
                sink.AcceptVisitor(this, node);
            }
        }

        protected virtual void VisitReference(IDatabaseObjectReference node)
        {
            if (options.VisitSchemaReferences)
            {
                if (node is IDataTypeReference dr)
                {
                    sink.Accept(dr);
                }

                if (node is IVariableReference vr)
                {
                    sink.Accept(vr);
                }

                if (node is IFunctionReference fr)
                {
                    sink.Accept(fr);
                }

                if (node is IColumnReference cr)
                {
                    sink.Accept(cr);
                }

                if (node is ITableReference tr)
                {
                    sink.Accept(tr);
                }
            }
        }

        protected virtual void VisitInlineNode(Token node)
        {
            // Depending on the traversal mode, we either route nodes to the sink
            // directly or we reshuffle to produce postfix notation for expression
            // tree building

            var reshuffler = ExpressionReshuffler;

            if (reshuffler != null)
            {
                reshuffler.Route(node);
            }
            else
            {
                TraverseInlineNode(node);
            }
        }

        public virtual void TraverseInlineNode(Token node)
        {
            // This is an expression inline or predicate inline that needs special handling such
            // as the OVER clause of windowed function calls.

            // Turn off expression context
            queryContextStack.Push(QueryContext & ~QueryContext.Expression & ~QueryContext.LogicalExpression);
            expressionReshufflerStack.Push(null);

            VisitNode(node);

            switch (node)
            {
                case OverClause n:
                    TraverseOverClause(n);
                    break;
                case SimpleCaseExpression n:
                    TraverseSimpleCaseExpression(n);
                    break;
                case SearchedCaseExpression n:
                    TraverseSearchedCaseExpression(n);
                    break;
                case Predicate n:
                    TraversePredicate(n);
                    break;

                // Special function arguments
                case LogicalArgument n:
                    TraverseLogicalArgument(n);
                    break;
                default:
                    throw new NotImplementedException();
            }

            queryContextStack.Pop();
            expressionReshufflerStack.Pop();
        }

        #endregion
        #region Statements

        private void TraverseStatementBlock(StatementBlock node)
        {
            VisitNode(node);

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
            TraverseLogicalExpression(node.Condition);
        }

        private void TraverseReturnStatement(ReturnStatement node)
        {
            // it might have a query in the parameter
            // do we support functions or stored procedures?
            throw new NotImplementedException();
        }

        private void TraverseIfStatement(IfStatement node)
        {
            TraverseLogicalExpression(node.Condition);
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
            VisitNode(node.Variable);
            VisitNode(node);
        }

        private void TraverseCreateTableStatement(CreateTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Create);

            // Do not visit table indentifier here because table is
            // non-existing and would cause problems with name resolution
            TraverseTableDefinition(node.TableDefinition);
            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseDropTableStatement(DropTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Drop);

            VisitNode(node.TargetTable);
            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseTruncateTableStatement(TruncateTableStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Truncate);

            VisitNode(node.TargetTable);
            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseCreateIndexStatement(CreateIndexStatement node)
        {
            var cds = node.IndexDefinition;
            var ics = node.IncludedColumns;

            tableContextStack.Push(TableContext | TableContext.Alter);

            VisitNode(node.TargetTable);
            VisitNode(node.IndexName);
            TraverseIndexDefinition(cds);

            if (ics != null)
            {
                TraverseIncludedColumns(ics);
            }

            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseIndexDefinition(IndexColumnDefinitionList node)
        {
            foreach (var cd in node.EnumerateDescendants<IndexColumnDefinition>())
            {
                VisitNode(cd);
            }
        }

        private void TraverseIncludedColumns(IncludedColumnList node)
        {
            foreach (var ic in node.EnumerateDescendants<IncludedColumnDefinition>())
            {
                VisitNode(ic);
            }
        }

        private void TraverseDropIndexStatement(DropIndexStatement node)
        {
            tableContextStack.Push(TableContext | TableContext.Alter);

            VisitNode(node.TargetTable);
            VisitNode(node.IndexName);
            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseSelectStatement(SelectStatement node)
        {
            queryContextStack.Push(QueryContext.SelectStatement);
            statementStack.Push(node);

            TraverseQuery(node);

            queryContextStack.Pop();
            statementStack.Pop();

            VisitNode(node);
        }

        private void TraverseInsertStatement(InsertStatement node)
        {
            queryContextStack.Push(QueryContext.InsertStatement);
            statementStack.Push(node);

            tableContextStack.Push(TableContext | TableContext.Insert);

            // Target table
            TraverseTargetTableSpecification(node.TargetTable);

            // Target column list, must be traversed before any table resolution to
            // make sure these all reference the target table
            var cl = node.ColumnList;

            columnContextStack.Push(ColumnContext | ColumnContext.Insert);

            if (cl != null)
            {
                TraverseInsertColumnList(cl);
            }

            columnContextStack.Pop();

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
                querySpecificationStack.Push(qs);
                TraverseOrderByClause(orderby);
                querySpecificationStack.Pop();
            }

            VisitNode(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            queryContextStack.Pop();
            statementStack.Pop();
        }

        private void TraverseInsertColumnList(InsertColumnList node)
        {
            foreach (var column in node.EnumerateDescendants<ColumnIdentifier>())
            {
                VisitNode(column);
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
            statementStack.Push(node);
            queryContextStack.Push(QueryContext.DeleteStatement);

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

            tableContextStack.Push(TableContext | TableContext.Delete);
            TraverseTargetTableSpecification(node.TargetTable);
            tableContextStack.Pop();

            if (where != null)
            {
                TraverseWhereClause(where);
            }

            VisitNode(node);

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

            // Target table
            // First visit left hand side only to make sure all columns are
            // target table columns
            tableContextStack.Push(TableContext | TableContext.Update);
            columnContextStack.Push(ColumnContext | ColumnContext.Update);

            TraverseTargetTableSpecification(node.TargetTable);
            TraverseUpdateSetList1(node.UpdateSetList);

            tableContextStack.Pop();
            columnContextStack.Pop();

            if (where != null)
            {
                TraverseWhereClause(where);
            }

            // Second traversal of the set list, this time the right-had sides,
            // which can have references to all the tables
            TraverseUpdateSetList2(node.UpdateSetList);

            VisitNode(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            queryContextStack.Pop();
            statementStack.Pop();
        }

        private void TraverseUpdateSetList1(UpdateSetList node)
        {
            foreach (var set in node.EnumerateSetColumns())
            {
                var leftvar = set.LeftHandSide.UserVariable;
                if (leftvar != null)
                {
                    VisitNode(leftvar);
                }

                var leftcol = set.LeftHandSide.ColumnIdentifier;
                if (leftcol != null)
                {
                    VisitNode(leftcol);
                }
            }
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
            var dt = node.DataTypeSpecification;
            var exp = node.Expression;

            VisitNode(node.DataTypeSpecification);

            if (exp != null)
            {
                TraverseExpression(exp);
            }

            VisitNode(node);
        }

        private void TraverseDeclareTableStatement(DeclareTableStatement node)
        {
            var td = node.TableDeclaration;
            TraverseTableDeclaration(td);
            VisitNode(node);
        }

        private void TraverseTableDeclaration(TableDeclaration node)
        {
            var td = node.TableDefinition;

            TraverseTableDefinition(td);
            VisitNode(node);
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

            if (tc != null)
            {
                // TODO
            }

            if (ti != null)
            {
                // TODO
            }
        }

        private void TraverseColumnDefinition(ColumnDefinition node)
        {
            var exp = node.DefaultDefinition?.Expression;

            if (exp != null)
            {
                TraverseExpression(exp);
            }

            TraverseDataTypeWithSize(node.DataTypeWithSize);
            VisitNode(node);
        }

        private void TraverseDataTypeWithSize(DataTypeSpecification node)
        {
            VisitNode(node.DataTypeIdentifier);
            VisitNode(node);
        }

        private void TraverseTableConstraint(TableConstraint node)
        {
            VisitNode(node);
        }

        private void TraverseTableIndex(TableIndex node)
        {
            VisitNode(node);
        }

        #endregion
        #region Expressions

        private void CreateExpressionReshuffler()
        {
            switch (options.ExpressionTraversal)
            {
                case ExpressionTraversalMethod.Infix:
                    expressionReshufflerStack.Push(null);
                    break;
                case ExpressionTraversalMethod.Postfix:
                    expressionReshufflerStack.Push(new ExpressionReshuffler(this, sink, ExpressionTraversalMethod.Postfix, new ArithmeticExpressionRules()));
                    break;
                case ExpressionTraversalMethod.Prefix:
                    expressionReshufflerStack.Push(new ExpressionReshuffler(this, sink, ExpressionTraversalMethod.Prefix, new ArithmeticExpressionRules()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DestroyExpressionReshuffler()
        {
            var reshuffler = expressionReshufflerStack.Pop();
            if (reshuffler != null)
            {
                reshuffler.Flush();
            }
        }

        protected void TraverseExpression(Expression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            queryContextStack.Push(QueryContext | QueryContext.Expression);
            columnContextStack.Push(ColumnContext | ColumnContext.Expression);

            if (options.VisitExpressionSubqueries)
            {
                TraverseExpressionSubqueries(node);
            }

            CreateExpressionReshuffler();
            TraverseExpressionNode(node);
            DestroyExpressionReshuffler();

            queryContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseExpressionSubqueries(Node node)
        {
            foreach (var n in node.Stack)
            {
                if (n is LogicalExpression)
                {
                    // Do not descend into logical expressions
                    continue;
                }
                else if (n is Subquery sq)
                {
                    TraverseSubquery(sq);
                }
                else if (n is Node nn)
                {
                    TraverseExpressionSubqueries(nn);
                }
            };
        }

        private void TraverseExpressionNode(Node node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case UnaryOperator n:
                        VisitNode(n);
                        break;
                    case BinaryOperator n:
                        VisitNode(n);
                        break;
                    case Operand n:
                        TraverseOperand(n);
                        break;
                    case Expression n:
                        TraverseExpressionNode(n);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TraverseOperand(Operand node)
        {
            memberAccessListStack.Push(new TokenList());

            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case Subquery n:
                        // Do not traverse subqueries again, they've been processed
                        // in a previous pass
                        break;
                    case MemberAccessList n:
                        TraverseMemberAccessList(n);
                        break;
                    case Node n:
                        DispatchOperand(n);
                        memberAccessListStack.Peek().AddLast(n);
                        break;
                    default:
                        break;
                }
            }

            VisitNode(node);
            memberAccessListStack.Pop();
        }

        protected virtual void DispatchOperand(Node node)
        {
            switch (node)
            {
                case Constant n:
                    VisitNode(n);
                    break;

                case SystemVariable n:
                    VisitNode(n);
                    VisitReference(n);
                    break;
                case UserVariable n:
                    VisitNode(n);
                    VisitReference(n);
                    break;

                case ExpressionSubquery n:
                    // Do not traverse here, subqueries should have been traversed
                    // earlier if requested
                    VisitNode(n);
                    break;
                case ExpressionBrackets n:
                    TraverseExpressionBrackets(n);
                    break;

                case SimpleCaseExpression n:
                    TraverseSimpleCaseExpression(n);
                    break;
                case SearchedCaseExpression n:
                    TraverseSearchedCaseExpression(n);
                    break;

                case SystemFunctionCall n:
                    TraverseSystemFunctionCall(n);
                    VisitReference(n);
                    break;
                case WindowedFunctionCall n:        // Also: StarFunctionCall and AggregateFunctionCall
                    TraverseWindowedFunctionCall(n);
                    VisitReference(n);
                    break;
                case ScalarFunctionCall n:
                    TraverseScalarFunctionCall(n);
                    VisitReference(n);
                    break;
                case TableValuedFunctionCall n:
                    TraverseTableValuedFunctionCall(n);
                    VisitReference(n);
                    break;

                // Special function calls
                case SpecialFunctionCall n:
                    TraverseSpecialFunctionCall(n);
                    break;

                case UdtStaticMemberAccessList n:
                    TraverseUdtStaticMemberAccessList(n);
                    break;

                case UdtMethodCall n:
                    TraverseUdtMethodCall(n);
                    VisitReference(n);
                    break;
                case UdtPropertyAccess n:
                    TraverseUdtPropertyAccess(n);
                    VisitReference(n);
                    break;

                case ObjectName n:
                    VisitNode(n);
                    break;
                case ColumnIdentifier n:
                    VisitNode(n);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void TraverseExpressionBrackets(ExpressionBrackets node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpressionNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseUdtStaticMemberAccessList(UdtStaticMemberAccessList node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case DataTypeIdentifier n:
                        VisitNode(n);
                        break;
                    case StaticMemberAccessOperator n:
                        VisitNode(n);
                        break;
                    case UdtStaticMethodCall n:
                        TraverseUdtStaticMethodCall(n);
                        break;
                    case UdtStaticPropertyAccess n:
                        TraverseUdtStaticPropertyAccess(n);
                        break;
                }
            }
        }

        private void TraverseMemberAccessList(MemberAccessList node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case MemberAccessOperator n:
                        VisitNode(n);
                        break;
                    case MemberCall n:
                        TraverseMemberCall(n);
                        memberAccessListStack.Peek().AddLast(n);
                        break;
                    case UdtMethodCall n:
                        TraverseUdtMethodCall(n);
                        memberAccessListStack.Peek().AddLast(n);
                        break;
                    case MemberAccess n:
                        TraverseMemberAccess(n);
                        memberAccessListStack.Peek().AddLast(n);
                        break;
                    case UdtPropertyAccess n:
                        TraverseUdtPropertyAccess(n);
                        memberAccessListStack.Peek().AddLast(n);
                        break;
                    case MemberAccessList n:
                        TraverseMemberAccessList(n);
                        break;

                        // TODO: extend this with property and method calls
                        // to handle resolved parsing trees
                }
            }
        }

        private void TraverseMemberAccess(MemberAccess node)
        {
            VisitNode(node.MemberName);
            VisitNode(node);
        }

        private void TraverseUdtPropertyAccess(UdtPropertyAccess node)
        {
            VisitNode(node.PropertyName);
            VisitNode(node);
        }

        private void TraverseUdtStaticPropertyAccess(UdtStaticPropertyAccess node)
        {
            VisitNode(node.PropertyName);
            VisitNode(node);
        }

        private void TraverseMemberCall(MemberCall node)
        {
            VisitNode(node.MemberName);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseSystemFunctionCall(SystemFunctionCall node)
        {
            VisitNode(node.FunctionName);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseScalarFunctionCall(ScalarFunctionCall node)
        {
            VisitNode(node.FunctionIdentifier);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseTableValuedFunctionCall(TableValuedFunctionCall node)
        {
            VisitNode(node.FunctionIdentifier);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseWindowedFunctionCall(WindowedFunctionCall node)
        {
            var over = node.OverClause;

            VisitNode(node.FunctionIdentifier);
            VisitNode(node);
            TraverseFunctionArguments(node);

            if (over != null)
            {
                TraverseOverClause(node.OverClause);
            }
        }

        private void TraverseUdtMethodCall(UdtMethodCall node)
        {
            VisitNode(node.MethodName);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseUdtStaticMethodCall(UdtStaticMethodCall node)
        {
            VisitNode(node.MethodName);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseSpecialFunctionCall(SpecialFunctionCall node)
        {
            VisitNode(node.FunctionName);
            VisitNode(node);
            TraverseFunctionArguments(node);
        }

        private void TraverseFunctionArguments(FunctionCall node)
        {
            int argumentCount = 0;

            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;

                    // Argument separators
                    case Comma n:
                        VisitNode(n);
                        break;
                    case Literal n 
                    when SqlParser.ComparerInstance.Compare(n.Value, "AS") == 0:
                        VisitNode(n);
                        break;
                    case Literal n
                    when SqlParser.ComparerInstance.Compare(n.Value, "USING") == 0:
                        VisitNode(n);
                        break;

                    // Various types of arguments
                    case StringConstant n:
                        VisitNode(n);
                        argumentCount++;
                        break;
                    case Argument n:
                        TraverseArgument(n);
                        argumentCount++;
                        break;
                    case LogicalArgument n:
                        TraverseLogicalArgument(n);
                        argumentCount++;
                        break;
                    case StarArgument n:
                        VisitNode(n);
                        argumentCount++;
                        break;
                    case DataTypeArgument n:
                        VisitNode(n);
                        argumentCount++;
                        break;
                    case DatePart n:
                        VisitNode(n);
                        argumentCount++;
                        break;
                    case ArgumentList n:
                        TraverseArgumentList(n, ref argumentCount);
                        break;

                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
            }

            node.ArgumentCount = argumentCount;
        }

        private void TraverseArgumentList(ArgumentList node, ref int argumentCount)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case Comma n:
                        VisitNode(n);
                        break;
                    case Argument n:
                        TraverseArgument(n);
                        argumentCount++;
                        break;
                    case ArgumentList n:
                        TraverseArgumentList(n, ref argumentCount);
                        break;
                }
            }
        }

        private void TraverseArgument(Argument node)
        {
            TraverseExpressionNode(node.Expression);
            VisitNode(node);
        }

        private void TraverseOverClause(OverClause node)
        {
            // If inside expression context, just pass the node to the reshuffler
            // If it's a callback from the reshuffler already, just traverse

            if (QueryContext.HasFlag(QueryContext.Expression))
            {
                VisitInlineNode(node);
            }
            else
            {
                foreach (var nn in SelectDirection(node.Stack))
                {
                    switch (nn)
                    {
                        case BracketOpen n:
                            VisitNode(n);
                            break;
                        case PartitionByClause n:
                            TraversePartitionByClause(n);
                            break;
                        case OrderByClause n:
                            TraverseOrderByClause(n);
                            break;
                        case BracketClose n:
                            VisitNode(n);
                            break;
                    }
                }
            }
        }

        private void TraversePartitionByClause(PartitionByClause node)
        {
            tableContextStack.Push(TableContext | TableContext.PartitionBy);
            columnContextStack.Push(ColumnContext | ColumnContext.PartitionBy);

            VisitNode(node);
            TraverseExpression(node.Argument.Expression);

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseOrderByClause(OrderByClause node)
        {
            tableContextStack.Push(TableContext | TableContext.OrderBy);
            columnContextStack.Push(ColumnContext | ColumnContext.OrderBy);

            VisitNode(node);
            TraverseOrderByArgumentList(node.ArgumentList);

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseOrderByArgumentList(OrderByArgumentList node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case Comma n:
                        VisitNode(n);
                        break;
                    case OrderByArgument n:
                        TraverseExpression(n.Expression);
                        VisitNode(n);
                        break;
                    case OrderByArgumentList n:
                        TraverseOrderByArgumentList(n);
                        break;
                }
            }
        }

        private void TraverseSimpleCaseExpression(SimpleCaseExpression node)
        {
            // If inside an expression context, just pass the node to the reshuffler
            // if it is an inline callback from the reshuffler already, just traverse

            if (QueryContext.HasFlag(QueryContext.Expression))
            {
                VisitInlineNode(node);
            }
            else
            {
                foreach (var nn in SelectDirection(node.Stack))
                {
                    switch (nn)
                    {
                        case Expression n:
                            TraverseExpression(n);
                            break;
                        case SimpleCaseWhenList n:
                            TraverseSimpleCaseWhenList(n);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void TraverseSimpleCaseWhenList(SimpleCaseWhenList node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case SimpleCaseWhen n:
                        TraverseSimpleCaseWhen(n);
                        break;
                    case SimpleCaseWhenList n:
                        TraverseSimpleCaseWhenList(n);
                        break;
                }
            }
        }

        private void TraverseSimpleCaseWhen(SimpleCaseWhen node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }
        }

        private void TraverseSearchedCaseExpression(SearchedCaseExpression node)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Special functions
        
        private void TraverseLogicalArgument(LogicalArgument node)
        {
            if (QueryContext.HasFlag(QueryContext.Expression))
            {
                VisitInlineNode(node);
            }
            else
            {
                TraverseLogicalExpression((LogicalExpression)node.Stack.First);
            }
        }

        private void TraverseDataTypeArgument(DataTypeArgument node)
        {
            VisitReference(node);
            VisitNode(node);
        }

        #endregion
        #region Boolean expression traversal

        private void CreateLogicalExpressionReshuffler()
        {
            switch (options.LogicalExpressionTraversal)
            {
                case ExpressionTraversalMethod.Infix:
                    expressionReshufflerStack.Push(null);
                    break;
                case ExpressionTraversalMethod.Postfix:
                    expressionReshufflerStack.Push(new ExpressionReshuffler(this, sink, ExpressionTraversalMethod.Postfix, new LogicalExpressionRules()));
                    break;
                case ExpressionTraversalMethod.Prefix:
                    expressionReshufflerStack.Push(new ExpressionReshuffler(this, sink, ExpressionTraversalMethod.Prefix, new LogicalExpressionRules()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DestroyLogicalExpressionReshuffler()
        {
            var reshuffler = expressionReshufflerStack.Pop();
            if (reshuffler != null)
            {
                reshuffler.Flush();
            }
        }

        protected void TraverseLogicalExpression(LogicalExpression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            queryContextStack.Push(QueryContext | QueryContext.LogicalExpression);

            if (options.VisitPredicateSubqueries)
            {
                TraverseLogicalExpressionSubqueries(node);
            }

            CreateLogicalExpressionReshuffler();
            TraverseLogicalExpressionNode(node);
            DestroyExpressionReshuffler();

            queryContextStack.Pop();
        }

        private void TraverseLogicalExpressionSubqueries(Node node)
        {
            foreach (var n in node.Stack)
            {
                if (n is Expression)
                {
                    // Do not descend into expressions
                    continue;
                }
                else if (n is Subquery sq)
                {
                    TraverseSubquery(sq);
                }
                else if (n is Node nn)
                {
                    TraverseLogicalExpressionSubqueries(nn);
                }
            };
        }

        private void TraverseLogicalExpressionNode(Node node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case LogicalNotOperator n:
                        VisitNode(n);
                        break;
                    case LogicalOperator n:
                        VisitNode(n);
                        break;
                    case LogicalExpressionBrackets n:
                        TraverseLogicalExpressionBrackets(n);
                        break;
                    case Predicate n:
                        TraversePredicate(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpressionNode(n);
                        break;
                    default:
                        break;
                }
            }
        }

        private void TraversePredicate(Predicate node)
        {
            columnContextStack.Push(ColumnContext | ColumnContext.Predicate);

            if (QueryContext.HasFlag(QueryContext.LogicalExpression))
            {
                VisitInlineNode(node);
            }
            else
            {
                // TODO: add option not to traverse predicates, just run
                // over the logical expression (as required for CNF/DNF)

                foreach (var nn in ((Node)node.Stack.First).Stack)
                {
                    switch (nn)
                    {
                        case Expression n:
                            TraverseExpression(n);
                            break;
                        case ArgumentList n:
                            int argc = 0;
                            TraverseArgumentList(n, ref argc);
                            break;
                        case Literal n:
                        case Operator op:
                        case Symbol s:
                            VisitNode(nn);
                            break;
                    }
                }
            }

            columnContextStack.Pop();
        }

        private void TraverseLogicalExpressionBrackets(LogicalExpressionBrackets node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpressionNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
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
                querySpecificationStack.Push(firstqs);
                TraverseOrderByClause(orderby);
                querySpecificationStack.Pop();
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
                VisitNode(ct);

                queryContextStack.Push(QueryContext.CommonTableExpression);
                TraverseQuery(ct.Subquery);
                queryContextStack.Pop();
            }

            commonTableExpression = null;

            VisitNode(cte);
        }

        private void TraverseQueryExpression(QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                TraverseQuerySpecification(qs);
            }

            VisitNode(qe);
        }

        private void TraverseQuerySpecification(QuerySpecification qs)
        {
            var into = qs.IntoClause;
            var from = qs.FromClause;
            var sl = qs.SelectList;
            var where = qs.WhereClause;
            var groupby = qs.GroupByClause;
            var having = qs.HavingClause;

            qs.ParentQuerySpecification = CurrentQuerySpecification;

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

            VisitNode(qs);
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
                VisitNode(ts);

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        protected virtual void DispatchTableSource(TableSource node)
        {
            switch (node)
            {
                case FunctionTableSource n:
                    TraverseFunctionTableSource(n);
                    break;
                case SimpleTableSource n:
                    TraverseSimpleTableSource(n);
                    break;
                case VariableTableSource n:
                    TraverseVariableTableSource(n);
                    break;
                case SubqueryTableSource n:
                    TraverseSubqueryTableSource(n);
                    break;
            }
        }

        private void TraverseFunctionTableSource(FunctionTableSource node)
        {
            TraverseTableValuedFunctionCall(node.FunctionCall);
            VisitNode(node);
        }

        private void TraverseSimpleTableSource(SimpleTableSource node)
        {
            VisitNode(node.TableOrViewIdentifier);
            VisitNode(node);
        }

        private void TraverseVariableTableSource(VariableTableSource node)
        {
            VisitNode(node.Variable);
            VisitNode(node);
        }

        private void TraverseSubqueryTableSource(SubqueryTableSource node)
        {
            tableContextStack.Push(TableContext | TableContext.Subquery);

            // Subquery has already been traversed!
            VisitNode(node);

            tableContextStack.Pop();
        }

        private void TraverseJoinConditions(TableSourceExpression node)
        {
            var ts = node.FindDescendant<TableSourceSpecification>();
            var jt = node.FindDescendant<JoinedTable>();

            while (ts != null)
            {
                var jc = jt?.FindDescendant<LogicalExpression>();

                if (jc != null)
                {
                    columnContextStack.Push(ColumnContext | ColumnContext.JoinOn);

                    TraverseLogicalExpression(jc);

                    columnContextStack.Pop();
                }

                ts = jt?.FindDescendant<TableSourceSpecification>();
                jt = jt?.FindDescendant<JoinedTable>();
            }
        }

        private void TraverseSubquery(Subquery sq)
        {
            queryContextStack.Push(QueryContext.Subquery);
            tableContextStack.Push(TableContext.None);
            columnContextStack.Push(ColumnContext.None);

            TraverseQuery(sq);

            queryContextStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
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
                    VisitNode(var);
                }

                if (exp != null)
                {
                    TraverseExpression(exp);
                }

                if (star != null)
                {
                    TraverseStarColumnIdentifier(star);
                }

                VisitNode(ce);
            }

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        private void TraverseStarColumnIdentifier(StarColumnIdentifier node)
        {
            var ti = node.TableOrViewIdentifier;

            if (ti != null)
            {
                VisitNode(ti);
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

            var exp = node.FindDescendant<LogicalExpression>();
            TraverseLogicalExpression(exp);

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

            var be = node.FindDescendant<LogicalExpression>();
            TraverseLogicalExpression(be);

            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        #endregion
        #region Node visitors

        private void TraverseTargetTableSpecification(TargetTableSpecification node)
        {
            var uv = node.Variable;
            var ti = node.TableOrViewIdentifier;

            if (uv != null)
            {
                VisitNode(uv);
            }
            else if (ti != null)
            {
                VisitNode(ti);
            }
            else
            {
                throw new NotImplementedException();
            }

            VisitNode(node);
        }

        #endregion
    }
}
