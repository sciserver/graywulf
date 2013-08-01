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

        protected abstract string QuoteIdentifier(string identifier);

        protected virtual string UnquoteIdentifier(string identifier)
        {
            return Util.RemoveIdentifierQuotes(identifier);
        }

        // ----

        public abstract string GenerateTableSelectStarQuery(string linkedServerName, string databaseName, string schemaName, string tableName, int top);

        public abstract string GenerateTableSelectStarQuery(string linkedServerName, TableReference table, int top);

        public abstract string GenerateMostRestrictiveTableQuery(TableReference table, int top);
    }
}
