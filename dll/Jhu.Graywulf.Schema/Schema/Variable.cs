using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    public abstract class Variable
    {
        private DatabaseObject parent;
        private int id;
        private string name;
        private DataType dataType;
        private LazyProperty<VariableMetadata> metadata;

        public DatabaseObject Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Ordinal ID of the objects (column, parameter, etc).
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Name of the variable
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Data type
        /// </summary>
        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public VariableMetadata Metadata
        {
            get { return metadata.Value; }
            set { metadata.Value = value; }
        }

        public Variable()
        {
            InitializeMembers();
        }

        public Variable(DatabaseObject parent)
        {
            InitializeMembers();

            this.parent = parent;
        }

        public Variable(Variable old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.parent = null;
            this.id = -1;
            this.name = String.Empty;
            this.dataType = null;
            this.metadata = new LazyProperty<VariableMetadata>(LoadMetadata);
        }

        private void CopyMembers(Variable old)
        {
            this.parent = old.parent;
            this.id = old.id;
            this.name = old.name;
            this.dataType = old.dataType;
            this.metadata = new LazyProperty<VariableMetadata>(LoadMetadata);
        }

        private VariableMetadata LoadMetadata()
        {
            if (Parent != null && Parent.Dataset != null)
            {
                Parent.Dataset.LoadAllVariableMetadata(this.Parent);
                return Metadata;
            }
            else
            {
                return new VariableMetadata();
            }
        }
    }
}
