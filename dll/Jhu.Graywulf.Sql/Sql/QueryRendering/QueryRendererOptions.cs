using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.QueryRendering
{
    public class QueryRendererOptions
    {
        #region Private members

        private IdentifierQuoting identifierQuoting;

        private NameRendering tableNameRendering;
        private AliasRendering tableAliasRendering;
        private NameRendering columnNameRendering;
        private NameRendering udtMemberNameRendering;
        private AliasRendering columnAliasRendering;
        private NameRendering dataTypeNameRendering;
        private NameRendering functionNameRendering;
        private VariableRendering variableRendering;
        private NameRendering indexNameRendering;
        private NameRendering constraintNameRendering;

        #endregion
        #region Properties

        public IdentifierQuoting IdentifierQuoting
        {
            get { return identifierQuoting; }
            set { identifierQuoting = value; }
        }

        /// <summary>
        /// Gets or sets whether to use resolved names in the generated code
        /// </summary>
        public NameRendering TableNameRendering
        {
            get { return tableNameRendering; }
            set { tableNameRendering = value; }
        }

        public AliasRendering TableAliasRendering
        {
            get { return tableAliasRendering; }
            set { tableAliasRendering = value; }
        }

        public NameRendering ColumnNameRendering
        {
            get { return columnNameRendering; }
            set { columnNameRendering = value; }
        }

        public NameRendering UdtMemberNameRendering
        {
            get { return udtMemberNameRendering; }
            set { udtMemberNameRendering = value; }
        }

        public AliasRendering ColumnAliasRendering
        {
            get { return columnAliasRendering; }
            set { columnAliasRendering = value; }
        }

        public NameRendering DataTypeNameRendering
        {
            get { return dataTypeNameRendering; }
            set { dataTypeNameRendering = value; }
        }

        public NameRendering FunctionNameRendering
        {
            get { return functionNameRendering; }
            set { functionNameRendering = value; }
        }

        public VariableRendering VariableRendering
        {
            get { return variableRendering; }
            set { variableRendering = value; }
        }

        public NameRendering IndexNameRendering
        {
            get { return indexNameRendering; }
            set { indexNameRendering = value; }
        }

        public NameRendering ConstraintNameRendering
        {
            get { return constraintNameRendering; }
            set { constraintNameRendering = value; }
        }

        #endregion
        #region Constructors and initializers

        public QueryRendererOptions()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.identifierQuoting = IdentifierQuoting.AlwaysQuote;

            this.tableNameRendering = NameRendering.Default;
            this.tableAliasRendering = AliasRendering.Default;
            this.columnNameRendering = NameRendering.Default;
            this.udtMemberNameRendering = NameRendering.Default;
            this.columnAliasRendering = AliasRendering.Default;
            this.dataTypeNameRendering = NameRendering.Default;
            this.functionNameRendering = NameRendering.Default;
            this.variableRendering = VariableRendering.Default;
        }

        #endregion
    }
}
