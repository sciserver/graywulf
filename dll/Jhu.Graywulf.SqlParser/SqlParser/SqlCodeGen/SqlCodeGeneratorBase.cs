using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.SqlCodeGen
{
    public abstract class SqlCodeGeneratorBase : CodeGenerator
    {
        private bool resolveNames;
        private bool quoteIdentifiers;

        public bool ResolveNames
        {
            get { return resolveNames; }
            set { resolveNames = value; }
        }

        public bool QuoteIdentifiers
        {
            get { return quoteIdentifiers; }
            set { quoteIdentifiers = value; }
        }

        public SqlCodeGeneratorBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.resolveNames = false;
            this.quoteIdentifiers = false;
        }

        public abstract bool WriteColumnExpression(ColumnExpression node);

        public abstract bool WriteColumnIdentifier(ColumnIdentifier node);

        public virtual bool WriteTableAlias(TableAlias node)
        {
            Writer.Write(QuoteIdentifier(node.Value));
            return false;
        }

        public abstract bool WriteTableOrViewName(TableOrViewName node);

        public abstract bool WriteFunctionIdentifier(FunctionIdentifier node);

        public abstract bool WriteTableValuedFunctionCall(TableValuedFunctionCall node);

        public bool WriteIdentifier(Identifier node)
        {
            if (quoteIdentifiers)
            {
                Writer.Write(QuoteIdentifier(UnquoteIdentifier(node.Value)));
            }
            else
            {
                Writer.Write(node.Value);
            }

            return true;
        }

        // --

        protected abstract string QuoteIdentifier(string identifier);

        protected virtual string UnquoteIdentifier(string identifier)
        {
            return Util.RemoveIdentifierQuotes(identifier);
        }

        // ----

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkedServerName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <remarks>This is used by the web interface's 'peek' function</remarks>
        public abstract string GenerateSelectStarQuery(TableOrView tableOrView, int top);

        protected abstract string GenerateTopExpression(int top);

        public abstract string GenerateMostRestrictiveTableQuery(TableReference table, bool includePrimaryKey, int top);
    }
}
