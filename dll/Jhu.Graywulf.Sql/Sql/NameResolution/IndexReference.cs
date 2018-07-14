using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class IndexReference : DatabaseObjectReference
    {
        #region Property storage variables

        // TODO: handle columns

        #endregion

        #region Properties

        public string IndexName
        {
            get { return DatabaseObjectName; }
            set { DatabaseObjectName = value; }
        }

        public Index Index
        {
            get { return (Index)DatabaseObject; }
            set { DatabaseObject = value; }
        }

        public override string UniqueName
        {
            get { return DatabaseObjectName; }
        }

        #endregion
        #region Constructors and initializer

        public IndexReference()
        {
            InitializeMembers();
        }

        public IndexReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }
        
        public IndexReference(Index index, bool copyColumns)
            : base(index)
        {
            InitializeMembers();

            throw new NotImplementedException();
        }

        public IndexReference(IndexReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(IndexReference old)
        {
        }

        public override object Clone()
        {
            return new IndexReference(this);
        }

        #endregion

        public static IndexReference Interpret(IndexName ii)
        {
            var ir = new IndexReference()
            {
                IndexName = RemoveIdentifierQuotes(ii?.Value),
            };

            return ir;
        }
    }
}
