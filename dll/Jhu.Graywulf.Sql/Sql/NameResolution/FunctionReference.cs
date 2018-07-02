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

        public static FunctionReference Interpret(UdfIdentifier fi)
        {
            var ds = fi.FindDescendant<DatasetPrefix>();
            var fpi = fi.FindDescendant<FourPartIdentifier>();

            var fr = new FunctionReference(fi)
            {
                DatasetName = Util.RemoveIdentifierQuotes(ds?.DatasetName),
                DatabaseName = Util.RemoveIdentifierQuotes(fpi.NamePart3),
                SchemaName = Util.RemoveIdentifierQuotes(fpi.NamePart2),
                DatabaseObjectName = Util.RemoveIdentifierQuotes(fpi.NamePart1),
                IsUserDefined = true
            };

            // This is a system function call
            if (fr.DatasetName == null && fr.DatabaseName == null && fr.SchemaName == null)
            {
                fr.IsUserDefined = false;
            }

            return fr;
        }

        public static FunctionReference Interpret(UdtVariableMethodIdentifier fi)
        {
            // TODO
            throw new NotImplementedException();
        }

        public static FunctionReference Interpret(UdtStaticMethodIdentifier fi)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
