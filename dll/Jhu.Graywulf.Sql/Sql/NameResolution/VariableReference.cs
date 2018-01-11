using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class VariableReference
    {
        private string name;
        private VariableReferenceType type;
        private DataTypeReference dataTypeReference;

        public string Name
        {
            get { return name; }
            set { name = value; }
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

        public Schema.DataType DataType
        {
            get { return dataTypeReference.DataType; }
            set { dataTypeReference.DataType = value; }
        }

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
            this.name = null;
            this.type = VariableReferenceType.Unknown;
            this.dataTypeReference = null;
        }

        private void CopyMembers(VariableReference old)
        {
            this.name = old.name;
            this.type = old.type;
            this.dataTypeReference = old.dataTypeReference;
        }

        public void InterpretUserVariable(Parsing.UserVariable variable)
        {
            name = variable.Name;
            type = VariableReferenceType.Scalar;
        }

        public void InterpretTableVariable(Parsing.TableVariable variable)
        {
            name = variable.Name;
            type = VariableReferenceType.Table;
        }

        public void InterpretSystemVariable(Parsing.SystemVariable variable)
        {
            name = variable.Name;
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

        public void InterpretTableDeclaration(Parsing.TableDeclaration td)
        {
            // TODO: implement
            throw new NotImplementedException();
        }
    }
}
