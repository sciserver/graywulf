using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class VariableReference
    {
        #region Property storage variables

        private Node node;

        private Variable variable;
        private bool isUserDefined;

        private string variableName;

        private VariableReferenceType type;

        private DataTypeReference dataTypeReference;
        private TableReference tableReference;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the parser tree node this table reference references
        /// </summary>
        public Node Node
        {
            get { return node; }
            protected set { node = value; }
        }

        public Variable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public bool IsUserDefined
        {
            get { return isUserDefined; }
            set { isUserDefined = value; }
        }

        public bool IsSystem
        {
            get { return !isUserDefined; }
        }

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }

        public virtual string UniqueName
        {
            get { return variableName; }
        }

        public VariableReferenceType Type
        {
            get { return type; }
            set { type = value; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
        }
        
        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        #endregion
        #region Constructor and initializers

        public VariableReference()
        {
            InitializeMembers();
        }

        public VariableReference(VariableReference old)
        {
            CopyMembers(old);
        }

        public VariableReference(Parsing.UserVariable variable)
            : this()
        {
            InterpretUserVariable(variable);
        }

        public VariableReference(Parsing.TableVariable variable)
            : this()
        {
            InterpretTableVariable(variable);
        }

        public VariableReference(Parsing.SystemVariable variable)
            : this()
        {
            InterpretSystemVariable(variable);
        }

        private void InitializeMembers()
        {
            this.node = null;

            this.variable = null;
            this.isUserDefined = true;

            this.variableName = null;
            this.type = VariableReferenceType.Unknown;
            this.dataTypeReference = null;
            this.tableReference = null;
        }

        private void CopyMembers(VariableReference old)
        {
            this.node = old.node;

            this.variable = old.variable;
            this.isUserDefined = old.isUserDefined;

            this.variableName = old.variableName;
            this.type = old.type;
            this.dataTypeReference = old.dataTypeReference;
            this.tableReference = old.tableReference;
        }

        public virtual object Clone()
        {
            return new VariableReference(this);
        }

        #endregion

        public void InterpretUserVariable(Parsing.UserVariable variable)
        {
            variableName = variable.Name;
            type = VariableReferenceType.Scalar;
        }

        public void InterpretTableVariable(Parsing.TableVariable variable)
        {
            variableName = variable.Name;
            type = VariableReferenceType.Table;
        }

        public void InterpretSystemVariable(Parsing.SystemVariable variable)
        {
            variableName = variable.Name;
            type = VariableReferenceType.System;
        }

        public void InterpretVariableDeclaration(Parsing.VariableDeclaration vd)
        {
            if (vd.IsCursor)
            {
                this.type = VariableReferenceType.Cursor;
            }
            else
            {
                this.type = VariableReferenceType.Scalar;
            }

            dataTypeReference = vd.DataType.DataTypeReference;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is for debugging purposes only, never use it in code generators!
        /// </remarks>
        public override string ToString()
        {
            return UniqueName;
        }
    }
}
