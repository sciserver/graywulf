using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class VariableReference : ReferenceBase
    {
        #region Property storage variables

        private Variable variable;

        private string variableName;
        private VariableContext variableContext;

        private DataTypeReference dataTypeReference;
        private TableReference tableReference;

        #endregion
        #region Properties

        public Variable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }

        public override string UniqueName
        {
            get { return variableName; }
            set { throw new InvalidOperationException(); }
        }

        public VariableContext VariableContext
        {
            get { return variableContext; }
            set { variableContext = value; }
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

        public VariableReference(Node node)
            :base(node)
        {
            InitializeMembers();
        }

        public VariableReference(VariableReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.variable = null;

            this.variableName = null;
            this.variableContext = VariableContext.None;
            this.dataTypeReference = null;
            this.tableReference = null;
        }

        private void CopyMembers(VariableReference old)
        {
            this.variable = old.variable;

            this.variableName = old.variableName;
            this.variableContext = old.variableContext;
            this.dataTypeReference = old.dataTypeReference;
            this.tableReference = old.tableReference;
        }

        public override object Clone()
        {
            return new VariableReference(this);
        }

        #endregion

        public static VariableReference Interpret(Parsing.UserVariable variable)
        {
            var vr = new VariableReference(variable)
            {
                variableName = variable.VariableName,
                variableContext = VariableContext.Scalar,
                IsUserDefined = true,
            };
            return vr;
        }

        public static VariableReference Interpret(Parsing.SystemVariable variable)
        {
            var vr = new VariableReference(variable)
            {
                variableName = variable.VariableName,
                variableContext = VariableContext.Scalar | VariableContext.System,
            };
            return vr;
        }

        public void Interpret(Parsing.VariableDeclaration vd)
        {
            if (vd.IsCursor)
            {
                this.variableContext = VariableContext.Cursor;
            }
            else
            {
                this.variableContext = VariableContext.Scalar;
            }

            dataTypeReference = vd.DataTypeIdentifier.DataTypeReference;
        }

        public void Interpret(Parsing.TableDeclaration td)
        {
            variableContext = VariableContext.Table;
            dataTypeReference = new DataTypeReference();
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
