﻿using System;
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

        private int pass;
        private Stack<int> indexStack;

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

        /// <summary>
        /// When the node is visited multiple times, it keeps track of the number of passes.
        /// </summary>
        /// <remarks>
        /// It is currently not generic, only used with a few nodes
        /// </remarks>
        public int Pass
        {
            get { return pass; }
        }

        /// <summary>
        /// When multiple children of the same node type are enumerated, this
        /// keeps track of the index.
        /// </summary>
        /// <remarks>
        /// It is currently not generic, only used with a few nodes
        /// </remarks>
        public int Index
        {
            get { return indexStack.Peek(); }
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
            this.options = CreateOptions();
            this.sink = null;

            this.pass = 0;
            this.indexStack = new Stack<int>();

            this.statementCounter = -1;
            this.statementStack = new Stack<Statement>();
            this.queryContextStack = new Stack<QueryContext>();
            this.tableContextStack = new Stack<TableContext>();
            this.columnContextStack = new Stack<ColumnContext>();
            this.commonTableExpression = null;
            this.querySpecificationStack = new Stack<QuerySpecification>();
            this.expressionReshufflerStack = new Stack<ExpressionReshuffler>();
            this.memberAccessListStack = new Stack<TokenList>();
        }

        protected virtual SqlQueryVisitorOptions CreateOptions()
        {
            return new SqlQueryVisitorOptions();
        }

        #endregion
        #region Entry points

        public void Execute(StatementBlock node)
        {
            statementCounter = -1;
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

        #endregion
        #region Context management

        protected void PushQueryContext(QueryContext value)
        {
            queryContextStack.Push(value);
        }

        protected QueryContext PopQueryContext()
        {
            return queryContextStack.Pop();
        }

        protected void PushTableContext(TableContext value)
        {
            tableContextStack.Push(value);
        }

        protected TableContext PopTableContext()
        {
            return tableContextStack.Pop();
        }

        protected void PushColumnContext(ColumnContext value)
        {
            columnContextStack.Push(value);
        }

        protected ColumnContext PopColumnContext()
        {
            return columnContextStack.Pop();
        }

        protected void PushAllContextNone()
        {
            queryContextStack.Push(QueryContext.None);
            tableContextStack.Push(TableContext.None);
            columnContextStack.Push(ColumnContext.None);
        }

        protected void PopAllContext()
        {
            queryContextStack.Pop();
            tableContextStack.Pop();
            columnContextStack.Pop();
        }

        protected void PushExpressionReshuffler(ExpressionTraversalMethod method, ExpressionReshufflerRules rules)
        {
            switch (method)
            {
                case ExpressionTraversalMethod.Infix:
                    expressionReshufflerStack.Push(null);
                    break;
                case ExpressionTraversalMethod.Postfix:
                case ExpressionTraversalMethod.Prefix:
                    expressionReshufflerStack.Push(
                        new ExpressionReshuffler(
                            this,
                            sink,
                            method,
                            rules));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void PopExpressionReshuffler()
        {
            var reshuffler = expressionReshufflerStack.Pop();
            if (reshuffler != null)
            {
                reshuffler.Flush();
            }
        }

        #endregion
        #region Common node travelsal and reshuffler dispatch

        protected IEnumerable<Token> SelectDirection(TokenList tokens)
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
            VisitNode(node, 0);
        }

        protected virtual void VisitNode(Token node, int pass)
        {
            switch (node)
            {
                case Literal n when !options.VisitLiterals:
                    return;
                case Symbol n when !options.VisitSymbols:
                    return;
            }

            this.pass = pass;

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
                sink.AcceptVisitor(this, node);
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

        internal void TraverseInlineNode(Token node)
        {
            // This is an expression inline or predicate inline that needs special handling such
            // as the OVER clause of windowed function calls.

            // Turn off expression context
            PushQueryContext(QueryContext & ~QueryContext.AnyExpression);
            expressionReshufflerStack.Push(null);

            VisitNode(node);
            DispatchInlineNode(node);

            PopQueryContext();
            expressionReshufflerStack.Pop();
        }

        protected virtual void DispatchInlineNode(Token node)
        {
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
        }

        #endregion
        #region Statements

        private void TraverseStatementBlock(StatementBlock node)
        {
            VisitNode(node, 0);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case AnyStatement n:
                        TraverseStatement(n.SpecificStatement);
                        break;
                    case StatementBlock n:
                        TraverseStatementBlock(n);
                        break;
                    case StatementSeparator n:
                        VisitNode(n);
                        break;
                }

            }

            VisitNode(node, 1);
        }

        private void TraverseStatement(Statement node)
        {
            statementStack.Push(node);
            statementCounter++;
            DispatchStatement(node);
            statementStack.Pop();
        }

        protected virtual void DispatchStatement(Statement node)
        {
            switch (node)
            {
                // Flow control statements
                case LabelDefinition n:
                    TraverseLabelDefinition(n);
                    break;
                case GotoStatement n:
                    TraverseGotoStatement(n);
                    break;
                case BeginEndStatement n:
                    TraverseBeginEndStatement(n);
                    break;
                case WhileStatement n:
                    TraverseWhileStatement(n);
                    break;
                case BreakStatement n:
                    TraverseBreakStatement(n);
                    break;
                case ContinueStatement n:
                    TraverseContinueStatement(n);
                    break;
                case ReturnStatement n:
                    TraverseReturnStatement(n);
                    break;
                case IfStatement n:
                    TraverseIfStatement(n);
                    break;

                // Error handling
                case TryCatchStatement n:
                    TraverseTryCatchStatement(n);
                    break;
                case ThrowStatement n:
                    TraverseThrowStatement(n);
                    break;

                // Misc statements
                case PrintStatement n:
                    TraversePrintStatement(n);
                    break;

                // Cursors
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

                // Variables
                case DeclareVariableStatement n:
                    TraverseDeclareVariableStatement(n);
                    break;
                case SetVariableStatement n:
                    TraverseSetVariableStatement(n);
                    break;

                // Tables
                case DeclareTableStatement n:
                    TraverseDeclareTableStatement(n);
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

                // Index
                case CreateIndexStatement n:
                    TraverseCreateIndexStatement(n);
                    break;
                case DropIndexStatement n:
                    TraverseDropIndexStatement(n);
                    break;

                // Queries and other DML
                case SelectStatement n:
                    TraverseSelectStatementInternal(n);
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

        private void TraverseLabelDefinition(LabelDefinition node)
        {
            VisitNode(node);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Label n:
                        VisitNode(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                    case Symbol n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseGotoStatement(GotoStatement node)
        {
            VisitNode(node);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Label n:
                        VisitNode(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseBeginEndStatement(BeginEndStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case StatementBlock n:
                        TraverseStatementBlock(n);
                        break;
                }

            }
        }

        private void TraverseWhileStatement(WhileStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpression(n);
                        break;
                    case AnyStatement n:
                        TraverseStatement(n.SpecificStatement);
                        break;
                }
            }
        }

        private void TraverseBreakStatement(BreakStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseContinueStatement(ContinueStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseReturnStatement(ReturnStatement node)
        {
            // it might have a query in the parameter
            // do we support functions or stored procedures?

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseTryCatchStatement(TryCatchStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case StatementBlock n:
                        TraverseStatementBlock(n);
                        break;
                }
            }
        }

        private void TraverseIfStatement(IfStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpression(n);
                        break;
                    case AnyStatement n:
                        TraverseStatement(n.SpecificStatement);
                        break;
                    case StatementSeparator n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseThrowStatement(ThrowStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case NumericConstant n:
                        VisitNode(n);
                        break;
                    case StringConstant n:
                        VisitNode(n);
                        break;
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraversePrintStatement(PrintStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }
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
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case ValueAssignmentOperator n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseCreateTableStatement(CreateTableStatement node)
        {
            PushTableContext(TableContext | TableContext.Create);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case TableDefinition n:
                        TraverseTableDefinition(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        private void TraverseDropTableStatement(DropTableStatement node)
        {
            PushTableContext(TableContext | TableContext.Drop);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        private void TraverseTruncateTableStatement(TruncateTableStatement node)
        {
            PushTableContext(TableContext | TableContext.Truncate);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        private void TraverseCreateIndexStatement(CreateIndexStatement node)
        {
            PushTableContext(TableContext | TableContext.Alter);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case IndexTypeSpecification n:
                        TraverseIndexTypeSpecification(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case IndexName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case IndexColumnDefinitionList n:
                        TraverseIndexColumnDefinitionList(n);
                        break;
                    case IncludedColumnDefinitionList n:
                        TraverseIncludedColumnDefinitionList(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        private void TraverseDropIndexStatement(DropIndexStatement node)
        {
            PushTableContext(TableContext | TableContext.Alter);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case IndexName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        private void TraverseSelectStatementInternal(SelectStatement node)
        {
            PushQueryContext(QueryContext.SelectStatement);
            statementStack.Push(node);

            DispatchSelectStatement(node);

            PopQueryContext();
            statementStack.Pop();

            VisitNode(node);
        }

        protected virtual void DispatchSelectStatement(SelectStatement node)
        {
            TraverseSelectStatement(node);
        }

        protected void TraverseSelectStatement(SelectStatement node)
        {
            TraverseQuery(node);

            var option = node.FindDescendant<OptionClause>();
            if (option != null)
            {
                TraverseOptionClause(option);
            }
        }

        private void TraverseInsertStatement(InsertStatement node)
        {
            PushQueryContext(QueryContext.InsertStatement);
            statementStack.Push(node);

            // Common table expression
            var cte = node.CommonTableExpression;
            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
                commonTableExpression = cte;
            }

            PushTableContext(TableContext | TableContext.Insert);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case IntoClause n1:
                    case TargetTableSpecification n2:
                        // Target table
                        TraverseTargetTableSpecification(node.TargetTable);
                        break;
                }
            }

            PopTableContext();

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case InsertColumnList n:
                        // Target column list, must be traversed before any table resolution to
                        // make sure these all reference the target table
                        TraverseInsertColumnList(n);
                        break;
                    case ValuesClause n:
                        TraverseValuesClause(n);
                        break;
                    case DefaultValues n:
                        VisitNode(n);
                        break;
                }
            }

            // Query
            var qe = node.QueryExpression;
            if (qe != null)
            {
                int index = 0;
                TraverseQueryExpression(qe, ref index);
            }

            var orderby = node.OrderByClause;
            if (orderby != null)
            {
                var firstqs = qe.FirstQuerySpecification;

                querySpecificationStack.Push(firstqs);
                PushTableContext(TableContext | TableContext.OrderBy);

                TraverseOrderByClause(orderby);

                querySpecificationStack.Pop();
                PopTableContext();
            }

            var option = node.FindDescendant<OptionClause>();
            if (option != null)
            {
                TraverseOptionClause(option);
            }

            VisitNode(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            PopQueryContext();
            statementStack.Pop();
        }

        private void TraverseInsertColumnList(InsertColumnList node)
        {
            PushColumnContext(ColumnContext | ColumnContext.Insert);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case ColumnIdentifierList n:
                        TraverseColumnIdentifierList(n);
                        break;
                }
            }

            VisitNode(node);

            PopColumnContext();
        }

        private void TraverseColumnIdentifierList(ColumnIdentifierList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnIdentifier n:
                        VisitNode(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case ColumnIdentifierList n:
                        TraverseColumnIdentifierList(n);
                        break;
                }
            }
        }

        private void TraverseValuesClause(ValuesClause node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ValueGroupList n:
                        TraverseValueGroupList(n);
                        break;
                }
            }
        }

        private void TraverseValueGroupList(ValueGroupList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ValueGroup n:
                        TraverseValueGroup(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case ValueGroupList n:
                        TraverseValueGroupList(n);
                        break;
                }
            }
        }

        private void TraverseValueGroup(ValueGroup node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case ValueList n:
                        TraverseValueList(n);
                        break;
                }
            }
        }

        private void TraverseValueList(ValueList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case DefaultValue n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case ValueList n:
                        TraverseValueList(n);
                        break;
                }
            }
        }

        private void TraverseDeleteStatement(DeleteStatement node)
        {
            statementStack.Push(node);
            PushQueryContext(QueryContext.DeleteStatement);

            var cte = node.CommonTableExpression;
            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
                commonTableExpression = cte;
            }

            // From clause of query part should happend before anything else
            var from = node.FromClause;
            if (from != null)
            {
                TraverseFromClause(from);
            }

            PushTableContext(TableContext | TableContext.Delete);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TargetTableSpecification n:
                        TraverseTargetTableSpecification(node.TargetTable);
                        break;
                }
            }

            PopTableContext();

            var where = node.WhereClause;
            if (where != null)
            {
                TraverseWhereClause(where);
            }

            VisitNode(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            PopQueryContext();
            statementStack.Pop();
        }

        private void TraverseUpdateStatement(UpdateStatement node)
        {
            PushQueryContext(QueryContext.UpdateStatement);
            statementStack.Push(node);

            // Common table expression
            var cte = node.CommonTableExpression;
            if (cte != null)
            {
                TraverseCommonTableExpression(cte);
                commonTableExpression = cte;
            }

            var from = node.FromClause;
            if (from != null)
            {
                TraverseFromClause(from);
            }

            // Target table
            PushTableContext(TableContext | TableContext.Update);
            PushColumnContext(ColumnContext | ColumnContext.Update);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TargetTableSpecification n:
                        TraverseTargetTableSpecification(n);
                        break;
                    case UpdateSetList n:
                        TraverseUpdateSetList(node.UpdateSetList);
                        break;
                }
            }

            PopTableContext();
            PopColumnContext();

            var where = node.WhereClause;
            if (where != null)
            {
                TraverseWhereClause(where);
            }

            VisitNode(node);

            if (cte != null)
            {
                commonTableExpression = null;
            }

            PopQueryContext();
            statementStack.Pop();
        }

        private void TraverseUpdateSetList(UpdateSetList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case UpdateSetColumn n:
                        TraverseUpdateSetColumn(n);
                        break;
                    case UpdateSetMutator n:
                        TraverseUpdateSetMutator(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case UpdateSetList n:
                        TraverseUpdateSetList(n);
                        break;
                }
            }
        }

        private void TraverseUpdateSetColumn(UpdateSetColumn node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case UpdateSetColumnLeftHandSide n:
                        TraverseUpdateSetColumnLeftHandSide(n);
                        break;
                    case ValueAssignmentOperator n:
                        VisitNode(n);
                        break;
                    case UpdateSetColumnRightHandSide n:
                        TraverseUpdateSetColumnRightHandSide(n);
                        break;
                }
            }
        }

        private void TraverseUpdateSetColumnLeftHandSide(UpdateSetColumnLeftHandSide node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case ColumnIdentifier n:
                        VisitNode(n);
                        break;
                    case Equals1 n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseUpdateSetColumnRightHandSide(UpdateSetColumnRightHandSide node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case DefaultValue n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }
        }

        private void TraverseUpdateSetMutator(UpdateSetMutator node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnName n:
                        VisitNode(n);
                        break;
                    case MemberAccessOperator n:
                        VisitNode(n);
                        break;
                    case UdtMethodCall n:
                        TraverseUdtMethodCall(n);
                        break;
                }
            }
        }

        #endregion
        #region Declarations

        private void TraverseDeclareVariableStatement(DeclareVariableStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case VariableDeclarationList n:
                        TraverseVariableDeclarationList(n);
                        break;
                }
            }
        }

        private void TraverseVariableDeclarationList(VariableDeclarationList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Comma n:
                        VisitNode(n);
                        break;
                    case VariableDeclaration n:
                        TraverseVariableDeclaration(n);
                        break;
                    case VariableDeclarationList n:
                        TraverseVariableDeclarationList(n);
                        break;
                }
            }
        }

        private void TraverseVariableDeclaration(VariableDeclaration node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ValueAssignmentOperator n:
                        VisitNode(n);
                        break;
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case DataTypeSpecification n:
                        TraverseDataTypeSpecification(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseDeclareTableStatement(DeclareTableStatement node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableDeclaration n:
                        TraverseTableDeclaration(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseTableDeclaration(TableDeclaration node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case TableDefinition n:
                        TraverseTableDefinition(n);
                        break;
                }
            }

            VisitNode(node);
        }

        #endregion
        #region DDL statements

        private void TraverseTableDefinition(TableDefinition node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case TableDefinitionList n:
                        TraverseTableDefinitionList(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseTableDefinitionList(TableDefinitionList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Comma n:
                        VisitNode(n);
                        break;
                    case ColumnDefinition n:
                        TraverseColumnDefinition(n);
                        break;
                    case TableConstraint n:
                        TraverseTableConstraint(n);
                        break;
                    case TableIndex n:
                        TraverseTableIndex(n);
                        break;
                    case TableDefinitionList n:
                        TraverseTableDefinitionList(n);
                        break;
                }
            }
        }


        private void TraverseColumnDefinition(ColumnDefinition node)
        {
            VisitNode(node, 0);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case DataTypeSpecification n:
                        TraverseDataTypeSpecification(n);
                        VisitReference(n);
                        break;
                    case ColumnSpecificationList n:
                        TraverseColumnSpecificationList(n);
                        break;
                }
            }

            VisitNode(node, 1);
        }

        private void TraverseColumnSpecificationList(ColumnSpecificationList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnNullSpecification n:
                        TraverseColumnNullSpecification(n);
                        break;
                    case ColumnDefaultSpecification n:
                        TraverseColumnDefaultSpecification(n);
                        break;
                    case ColumnIdentitySpecification n:
                        TraverseColumnIdentityDefinition(n);
                        break;
                    case ColumnConstraint n:
                        TraverseColumnConstraint(n);
                        break;
                    case ColumnIndex n:
                        TraverseColumnIndex(n);
                        break;
                    case ColumnSpecificationList n:
                        TraverseColumnSpecificationList(n);
                        break;
                }
            }
        }

        private void TraverseColumnNullSpecification(ColumnNullSpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseColumnDefaultSpecification(ColumnDefaultSpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ConstraintName n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseColumnIdentityDefinition(ColumnIdentitySpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case NumericConstant n:
                        VisitNode(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseColumnConstraint(ColumnConstraint node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ConstraintName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case ColumnDefaultSpecification n:
                        TraverseColumnDefaultSpecification(n);
                        break;
                    case IndexTypeSpecification n:
                        TraverseIndexTypeSpecification(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseColumnIndex(ColumnIndex node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case IndexName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case IndexTypeSpecification n:
                        TraverseIndexTypeSpecification(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseTableConstraint(TableConstraint node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ConstraintName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case IndexTypeSpecification n:
                        TraverseIndexTypeSpecification(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case IndexColumnDefinitionList n:
                        TraverseIndexColumnDefinitionList(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseIndexTypeSpecification(IndexTypeSpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseTableIndex(TableIndex node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case IndexName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case IndexTypeSpecification n:
                        TraverseIndexTypeSpecification(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case IndexColumnDefinitionList n:
                        TraverseIndexColumnDefinitionList(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseIndexColumnDefinitionList(IndexColumnDefinitionList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case IndexColumnDefinition n:
                        TraverseIndexColumnDefinition(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case IndexColumnDefinitionList n:
                        TraverseIndexColumnDefinitionList(n);
                        break;
                }
            }
        }

        private void TraverseIndexColumnDefinition(IndexColumnDefinition node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseIncludedColumnDefinitionList(IncludedColumnDefinitionList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case IncludedColumnDefinition n:
                        TraverseIncludedColumnDefinition(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case IncludedColumnDefinitionList n:
                        TraverseIncludedColumnDefinitionList(n);
                        break;
                }
            }
        }

        private void TraverseIncludedColumnDefinition(IncludedColumnDefinition node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnName n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                }
            }
        }

        private void TraverseDataTypeSpecification(DataTypeSpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case DataTypeIdentifier n:
                        VisitNode(n);
                        break;
                    case DataTypeScaleAndPrecision n:
                        TraverseDataTypeScaleAndPrecision(n);
                        break;
                    case DataTypeSize n:
                        TraverseDataTypeSize(n);
                        break;
                }
            }

            VisitNode(node);
        }

        private void TraverseDataTypeScaleAndPrecision(DataTypeScaleAndPrecision node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case NumericConstant n:
                        VisitNode(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseDataTypeSize(DataTypeSize node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case NumericConstant n:
                        VisitNode(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
            }
        }

        #endregion
        #region Expressions

        protected void TraverseExpression(Expression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            PushQueryContext(QueryContext | QueryContext.Expression);
            PushColumnContext(ColumnContext | ColumnContext.Expression);

            if (options.VisitExpressionSubqueries)
            {
                TraverseExpressionSubqueries(node);
            }

            PushExpressionReshuffler(options.ExpressionTraversal, new ArithmeticExpressionRules());
            TraverseExpressionNode(node);
            PopExpressionReshuffler();

            PopQueryContext();
            PopColumnContext();
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

        private void TraverseExpressionNode(Expression node)
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
                        VisitNode(n);
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
            PushTableContext(TableContext | TableContext.PartitionBy);
            PushColumnContext(ColumnContext | ColumnContext.PartitionBy);

            VisitNode(node);
            TraverseExpression(node.Argument.Expression);

            PopTableContext();
            PopColumnContext();
        }

        private void TraverseOrderByClause(OrderByClause node)
        {
            PushColumnContext(ColumnContext | ColumnContext.OrderBy);

            VisitNode(node);
            TraverseOrderByArgumentList(node.ArgumentList);

            PopColumnContext();
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

        private void TraverseOptionClause(OptionClause node)
        {
            // TODO, maybe
            throw new NotImplementedException();
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
                        case Literal n
                        when SqlParser.ComparerInstance.Compare(n.Value, "CASE") == 0 ||
                             SqlParser.ComparerInstance.Compare(n.Value, "ELSE") == 0 ||
                             SqlParser.ComparerInstance.Compare(n.Value, "END") == 0:
                            VisitNode(n);
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
                    case Literal n
                    when SqlParser.ComparerInstance.Compare(n.Value, "WHEN") == 0 ||
                         SqlParser.ComparerInstance.Compare(n.Value, "THEN") == 0:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseSearchedCaseExpression(SearchedCaseExpression node)
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
                        case Literal n
                        when SqlParser.ComparerInstance.Compare(n.Value, "CASE") == 0 ||
                             SqlParser.ComparerInstance.Compare(n.Value, "ELSE") == 0 ||
                             SqlParser.ComparerInstance.Compare(n.Value, "END") == 0:
                            VisitNode(n);
                            break;
                        case SearchedCaseWhenList n:
                            TraverseSearchedCaseWhenList(n);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void TraverseSearchedCaseWhenList(SearchedCaseWhenList node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case SearchedCaseWhen n:
                        TraverseSearchedCaseWhen(n);
                        break;
                    case SearchedCaseWhenList n:
                        TraverseSearchedCaseWhenList(n);
                        break;
                }
            }
        }

        private void TraverseSearchedCaseWhen(SearchedCaseWhen node)
        {
            foreach (var nn in SelectDirection(node.Stack))
            {
                switch (nn)
                {
                    case LogicalExpression n:
                        TraverseLogicalExpression(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                    case Literal n
                    when SqlParser.ComparerInstance.Compare(n.Value, "WHEN") == 0 ||
                         SqlParser.ComparerInstance.Compare(n.Value, "THEN") == 0:
                        VisitNode(n);
                        break;
                }
            }
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

        protected void TraverseLogicalExpression(LogicalExpression node)
        {
            // Visit immediate subquries first, then do a bottom-up
            // traversal of the tree by not going deeper than the subqueries.

            PushQueryContext(QueryContext | QueryContext.LogicalExpression);

            if (options.VisitPredicateSubqueries)
            {
                TraverseLogicalExpressionSubqueries(node);
            }

            PushExpressionReshuffler(options.LogicalExpressionTraversal, new LogicalExpressionRules());
            TraverseLogicalExpressionNode(node);
            PopExpressionReshuffler();

            PopQueryContext();
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
            PushColumnContext(ColumnContext | ColumnContext.Predicate);

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
                        case Literal n:
                        case Operator op:
                        case Symbol s:
                            VisitNode(nn);
                            break;
                        case Expression n:
                            TraverseExpression(n);
                            break;
                        case ArgumentList n:
                            int argc = 0;
                            TraverseArgumentList(n, ref argc);
                            break;
                        case Subquery n:
                            VisitNode(n);
                            break;
                    }
                }
            }

            PopColumnContext();
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
                commonTableExpression = cte;
            }

            int index = 0;
            TraverseQueryExpression(qe, ref index);

            if (orderby != null)
            {
                var firstqs = qe.FirstQuerySpecification;

                querySpecificationStack.Push(firstqs);
                PushTableContext(TableContext | TableContext.OrderBy);

                TraverseOrderByClause(orderby);

                querySpecificationStack.Pop();
                PopTableContext();
            }

            if (cte != null)
            {
                commonTableExpression = null;
            }
        }

        protected void TraverseCommonTableExpression(CommonTableExpression node)
        {
            VisitNode(node, 0);

            commonTableExpression = node;

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case CommonTableSpecificationList n:
                        TraverseCommonTableSpecificationList(n);
                        break;
                }
            }

            commonTableExpression = null;

            VisitNode(node, 1);
        }

        protected void TraverseCommonTableSpecificationList(CommonTableSpecificationList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case CommonTableSpecification n:
                        TraverseCommonTableSpecification(n);
                        break;
                    case CommonTableSpecificationList n:
                        TraverseCommonTableSpecificationList(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                }
            }
        }

        protected void TraverseCommonTableSpecification(CommonTableSpecification node)
        {
            // This needs to happen early otherwise recursive queries won't work
            VisitNode(node, 0);

            PushQueryContext(QueryContext.CommonTableExpression);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case TableAlias n:
                        VisitNode(n);
                        break;
                    case ColumnAliasBrackets n:
                        TraverseColumnAliasBrackets(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                    case CommonTableSubquery n:
                        TraverseQuery(n);
                        break;
                }
            }

            PopQueryContext();

            VisitNode(node, 1);
        }

        protected void TraverseColumnAliasBrackets(ColumnAliasBrackets node)
        {
            throw new NotImplementedException();
        }

        protected void TraverseQueryExpression(QueryExpression node, ref int index)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case QueryExpressionBrackets n:
                        TraverseQueryExpressionBrackets(n, ref index);
                        break;
                    case QueryExpression n:
                        TraverseQueryExpression(n, ref index);
                        break;
                    case QueryOperator n:
                        VisitNode(n);
                        break;
                    case QuerySpecification n:
                        indexStack.Push(index++);
                        TraverseQuerySpecificationInternal(n);
                        indexStack.Pop();
                        break;
                }
            }
        }

        protected void TraverseQueryExpressionBrackets(QueryExpressionBrackets node, ref int index)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case QueryExpression n:
                        TraverseQueryExpression(n, ref index);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                }
            }
        }

        private void TraverseQuerySpecificationInternal(QuerySpecification qs)
        {
            VisitNode(qs, 0);
            querySpecificationStack.Push(qs);

            DispatchQuerySpecification(qs);

            querySpecificationStack.Pop();
            VisitNode(qs, 1);
        }

        protected virtual void DispatchQuerySpecification(QuerySpecification qs)
        {
            TraverseQuerySpecification(qs);
        }

        protected void TraverseQuerySpecification(QuerySpecification qs)
        {
            var into = qs.IntoClause;
            var from = qs.FromClause;
            var sl = qs.SelectList;
            var where = qs.WhereClause;
            var groupby = qs.GroupByClause;
            var having = qs.HavingClause;

            // TODO: make this a dynamic dispatch so it can be extended
            // when the grammar is extended

            if (from != null)
            {
                TraverseFromClause(from);
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

            foreach (var nn in qs.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TopExpression n:
                        TraverseTopExpression(n);
                        break;
                }
            }

            // This has to come after from
            if (sl != null)
            {
                TraverseSelectList(sl);
            }

            // This needs to be done last
            if (into != null)
            {
                TraverseIntoClause(into);
            }
        }

        protected void TraverseFromClause(FromClause node)
        {
            PushTableContext(TableContext | TableContext.From);
            PushColumnContext(ColumnContext | ColumnContext.From);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableSourceExpression n:
                        TraverseTableSourceExpression(n);
                        break;
                }
            }

            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseTableSourceExpression(TableSourceExpression node)
        {
            // Traverse all subqueries first
            TraverseSubqueryTableSources(node);

            // Traverse table sources second
            TraverseTableSourceExpressionNode(node, 0);

            // Traverse join conditions third
            TraverseTableSourceExpressionNode(node, 1);
        }

        protected void TraverseSubqueryTableSources(TableSourceExpression node)
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

        protected void TraverseTableSourceExpressionNode(TableSourceExpression node, int pass)
        {
            if (pass == 0)
            {
                foreach (var nn in node.Stack)
                {
                    switch (nn)
                    {
                        case TableSourceSpecification ts:
                            DispatchTableSourceSpecification(ts);
                            break;
                        case JoinedTable n:
                            TraverseJoinedTable(n, pass);
                            break;
                    }
                }
            }
            else if (pass == 1)
            {
                foreach (var nn in node.Stack)
                {
                    switch (nn)
                    {
                        case JoinedTable n:
                            TraverseJoinedTable(n, pass);
                            break;
                    }
                }
            }
        }

        protected void TraverseJoinedTable(JoinedTable node, int pass)
        {
            if (pass == 0)
            {
                foreach (var nn in node.Stack)
                {
                    switch (nn)
                    {
                        case JoinOperator n:
                            TraverseJoinOperator(n);
                            VisitNode(n);
                            break;
                        case TableSourceSpecification n:
                            DispatchTableSource(n.SpecificTableSource);
                            VisitNode(n);
                            break;
                        case JoinedTable n:
                            TraverseJoinedTable(n, pass);
                            break;
                    }
                }
            }
            else if (pass == 1)
            {
                foreach (var nn in node.Stack)
                {
                    switch (nn)
                    {
                        case JoinCondition n:
                            TraverseJoinCondition(n);
                            break;
                        case JoinedTable n:
                            TraverseJoinedTable(n, pass);
                            break;
                    }
                }
            }
        }

        protected void TraverseJoinOperator(JoinOperator node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                }
            }
        }

        protected void TraverseJoinCondition(JoinCondition node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        PushColumnContext(ColumnContext | ColumnContext.JoinOn);
                        TraverseLogicalExpression(n);
                        PopColumnContext();
                        break;
                }
            }
        }

        protected virtual void DispatchTableSourceSpecification(TableSourceSpecification node)
        {
            TraverseTableSourceSpecification(node);
        }

        private void TraverseTableSourceSpecification(TableSourceSpecification node)
        {
            DispatchTableSource(node.SpecificTableSource);
            VisitNode(node);
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

        protected virtual void TraverseFunctionTableSource(FunctionTableSource node)
        {
            TraverseTableValuedFunctionCall(node.FunctionCall);
            VisitNode(node);
        }

        protected virtual void TraverseSimpleTableSource(SimpleTableSource node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        break;
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableAlias n:
                        VisitNode(n);
                        break;
                    case TableSampleClause n:
                        TraverseTableSampleClause(n);
                        break;
                    case TableHintClause n:
                        TraverseTableHintClause(n);
                        break;
                }
            }

            VisitNode(node);
        }

        protected virtual void TraverseVariableTableSource(VariableTableSource node)
        {
            VisitNode(node.Variable);
            VisitNode(node);
        }

        protected virtual void TraverseSubqueryTableSource(SubqueryTableSource node)
        {
            PushTableContext(TableContext | TableContext.Subquery);

            // Subquery has already been traversed!
            VisitNode(node);

            PopTableContext();
        }

        protected void TraverseTableSampleClause(TableSampleClause node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case NumericConstant n:
                        VisitNode(n);
                        break;
                }
            }
        }

        protected void TraverseTableHintClause(TableHintClause node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case TableHintList n:
                        TraverseTableHintList(n);
                        break;
                }
            }
        }

        protected void TraverseTableHintList(TableHintList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case TableHint n:
                        TraverseTableHint(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case TableHintList n:
                        TraverseTableHintList(n);
                        break;
                }
            }
        }

        protected void TraverseTableHint(TableHint node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case HintName n:
                        VisitNode(n);
                        break;
                    case HintArguments n:
                        TraverseHintArguments(n);
                        break;
                }
            }
        }

        protected void TraverseHintArguments(HintArguments node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case BracketOpen n:
                        VisitNode(n);
                        break;
                    case BracketClose n:
                        VisitNode(n);
                        break;
                    case HintArgumentList n:
                        TraverseHintArgumentList(n);
                        break;
                }
            }
        }

        protected void TraverseHintArgumentList(HintArgumentList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case HintArgument n:
                        TraverseHintArgument(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case HintArgumentList n:
                        TraverseHintArgumentList(n);
                        break;
                }
            }
        }

        protected void TraverseHintArgument(HintArgument node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }
        }

        protected void TraverseSubquery(Subquery sq)
        {
            PushQueryContext(QueryContext.Subquery);
            PushTableContext(TableContext.None);
            PushColumnContext(ColumnContext.None);

            TraverseQuery(sq);

            PopQueryContext();
            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseSelectList(SelectList node)
        {
            PushTableContext(TableContext | TableContext.SelectList);
            PushColumnContext(ColumnContext | ColumnContext.SelectList);

            TraverseSelectListNode(node);
            VisitNode(node);

            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseSelectListNode(SelectList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case ColumnExpression n:
                        TraverseColumnExpression(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case SelectList n:
                        TraverseSelectListNode(n);
                        break;
                }
            }
        }

        protected void TraverseColumnExpression(ColumnExpression node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case ColumnAlias n:
                        VisitNode(n);
                        break;
                    case ValueAssignmentOperator n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                    case StarColumnIdentifier n:
                        TraverseStarColumnIdentifier(n);
                        break;
                }
            }

            VisitNode(node);
        }

        protected void TraverseStarColumnIdentifier(StarColumnIdentifier node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        break;
                    case Dot n:
                        VisitNode(n);
                        break;
                    case Mul n:
                        VisitNode(n);
                        break;
                }
            }

            VisitNode(node);
        }

        protected void TraverseIntoClause(IntoClause node)
        {
            PushTableContext(TableContext | TableContext.Into);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TargetTableSpecification n:
                        TraverseTargetTableSpecification(n);
                        break;
                }
            }

            VisitNode(node);

            PopTableContext();
        }

        protected void TraverseWhereClause(WhereClause node)
        {
            PushTableContext(TableContext | TableContext.Where);
            PushColumnContext(ColumnContext | ColumnContext.Where);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpression(n);
                        break;
                }
            }

            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseGroupByClause(GroupByClause node)
        {
            PushTableContext(TableContext | TableContext.GroupBy);
            PushColumnContext(ColumnContext | ColumnContext.GroupBy);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case GroupByList n:
                        TraverseGroupByList(n);
                        break;
                }
            }

            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseGroupByList(GroupByList node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Expression n:
                        TraverseExpression(n);
                        break;
                    case Comma n:
                        VisitNode(n);
                        break;
                    case GroupByList n:
                        TraverseGroupByList(n);
                        break;
                }
            }
        }

        protected void TraverseHavingClause(HavingClause node)
        {
            PushTableContext(TableContext | TableContext.Having);
            PushColumnContext(ColumnContext | ColumnContext.Having);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case LogicalExpression n:
                        TraverseLogicalExpression(n);
                        break;
                }
            }

            PopTableContext();
            PopColumnContext();
        }

        protected void TraverseTopExpression(TopExpression node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case Expression n:
                        TraverseExpression(n);
                        break;
                }
            }
        }

        #endregion
        #region Node visitors

        protected void TraverseTargetTableSpecification(TargetTableSpecification node)
        {
            PushTableContext(TableContext | TableContext.Target);

            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case UserVariable n:
                        VisitNode(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        break;
                    case TableAlias n:
                        VisitNode(n);
                        break;
                    case TableHintClause n:
                        TraverseTableHintClause(n);
                        break;
                }
            }

            PopTableContext();

            VisitNode(node);
        }

        #endregion
    }
}
