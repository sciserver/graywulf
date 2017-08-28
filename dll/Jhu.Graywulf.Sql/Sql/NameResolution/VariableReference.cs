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
        private bool isCursor;
        private bool isTable;
        private DataTypeReference dataTypeReference;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsCursor
        {
            get { return isCursor; }
        }

        public bool IsTable
        {
            get { return isTable; }
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
        
        private void InitializeMembers()
        {
            this.name = null;
            this.isCursor = false;
            this.isTable = false;
            this.dataTypeReference = null;
        }

        private void CopyMembers(VariableReference old)
        {
            this.name = old.name;
            this.isCursor = old.isCursor;
            this.isTable = old.isTable;
            this.dataTypeReference = old.dataTypeReference;
        }

        public void InterpretUserVariable(Parsing.UserVariable variable)
        {
            name = variable.Name;
        }

        public void InterpretVariableDeclaration(Parsing.VariableDeclaration vd)
        {
            isCursor = vd.IsCursor;
            isTable = false;
            dataTypeReference = vd.DataType.DataTypeReference;
        }
    }
}
