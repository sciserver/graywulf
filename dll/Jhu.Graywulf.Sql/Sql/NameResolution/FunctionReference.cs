using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class FunctionReference : DatabaseObjectReference
    {
        #region Property storage variables
        #endregion
        #region Properties

        public string FunctionName
        {
            get { return DatabaseObjectName; }
            set { DatabaseObjectName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string UniqueName
        {
            get
            {
                if (!IsSystem)
                {
                    return base.UniqueName;
                }
                else
                {
                    return DatabaseObjectName;
                }
            }
        }

        #endregion
        #region Constructors and initializers

        public FunctionReference()
        {
            InitializeMembers();
        }

        public FunctionReference(Node node)
            :base(node)
        {
            InitializeMembers();
        }

        public FunctionReference(FunctionReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(FunctionReference old)
        {
        }

        public override object Clone()
        {
            return new FunctionReference(this);
        }

        #endregion

        public static FunctionReference Interpret(FunctionName fn)
        {
            var fr = new FunctionReference(fn)
            {
                FunctionName = RemoveIdentifierQuotes(fn.Value),
            };

            return fr;
        }

        public static FunctionReference Interpret(FunctionIdentifier fi)
        {
            var ds = fi.FindDescendant<DatasetPrefix>();
            var database = fi.FindDescendant<DatabaseName>()?.Value;
            var schema = fi.FindDescendant<SchemaName>()?.Value;
            var function = fi.FindDescendant<FunctionName>().Value;

            var fr = new FunctionReference(fi)
            {
                DatasetName = RemoveIdentifierQuotes(ds?.DatasetName),
                DatabaseName = RemoveIdentifierQuotes(database),
                SchemaName = RemoveIdentifierQuotes(schema),
                FunctionName = RemoveIdentifierQuotes(function),
            };

            // This is a system function call
            if (fr.DatasetName == null && fr.DatabaseName == null && fr.SchemaName == null)
            {
                fr.IsUserDefined = false;
            }

            return fr;
        }

        public static FunctionReference Interpret(TokenList tokens, int colpart)
        {
            switch (colpart)
            {
                case 1:
                    return new FunctionReference((Node)tokens[0])
                    {
                        SchemaName = RemoveIdentifierQuotes(tokens[0].Value),
                        FunctionName = RemoveIdentifierQuotes(((MemberCall)tokens[1]).MemberName.Value),
                    };
                case 2:
                    return new FunctionReference((Node)tokens[0])
                    {
                        DatabaseName = RemoveIdentifierQuotes(tokens[0].Value),
                        SchemaName = RemoveIdentifierQuotes(tokens[1].Value),
                        FunctionName = RemoveIdentifierQuotes(((MemberCall)tokens[2]).MemberName.Value),
                    };
                default:
                    throw new InvalidOperationException();
            }
        }

        public override void LoadDatabaseObject(DatasetBase dataset)
        {
            DatabaseObject = dataset.GetObject(DatabaseName, SchemaName, DatabaseObjectName);

            // TODO: we could figure out here if it's a scalar function or else
        }

    }
}
