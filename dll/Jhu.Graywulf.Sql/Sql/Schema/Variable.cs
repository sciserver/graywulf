using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Variable
    {
        [NonSerialized]
        private DatabaseObject parent;

        [NonSerialized]
        private int id;

        [NonSerialized]
        private string name;

        [NonSerialized]
        private DataType dataType;

        [NonSerialized]
        private LazyProperty<VariableMetadata> metadata;

        [IgnoreDataMember]
        public DatabaseObject Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Ordinal ID of the objects (column, parameter, etc).
        /// </summary>
        [DataMember]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Name of the variable
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Data type
        /// </summary>
        [DataMember]
        public DataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        [DataMember]
        public VariableMetadata Metadata
        {
            get { return metadata.Value; }
            set { metadata.Value = value; }
        }

        [IgnoreDataMember]
        public virtual string FullyResolvedName
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the internal object key.
        /// </summary>
        [IgnoreDataMember]
        public string UniqueKey
        {
            get { return name; }
            set { name = value; }
        }

        public Variable()
        {
            InitializeMembers(new StreamingContext());
        }

        public Variable(DatabaseObject parent)
        {
            InitializeMembers(new StreamingContext());

            this.parent = parent;
        }

        public Variable(Variable old)
        {
            CopyMembers(old);
        }

        [OnSerializing]
        private void InitializeMembers(StreamingContext context)
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
                return (VariableMetadata)Metadata;
            }
            else
            {
                return new VariableMetadata();
            }
        }
    }
}
