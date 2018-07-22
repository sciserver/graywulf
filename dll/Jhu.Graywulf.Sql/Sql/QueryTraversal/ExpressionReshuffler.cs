﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    abstract class ExpressionReshuffler
    {
        private SqlQueryVisitor visitor;
        private SqlQueryVisitorSink sink;

        protected ExpressionReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
        {
            this.visitor = visitor;
            this.sink = sink;
        }

        protected virtual void Output(Token n)
        {
            sink.AcceptVisitor(visitor, n);
        }

        public abstract void Route(Token node);

        public abstract void Flush();
    }
}
