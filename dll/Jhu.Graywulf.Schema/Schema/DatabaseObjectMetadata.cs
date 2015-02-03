using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class DatabaseObjectMetadata : Metadata, ICloneable
    {
        private string example;
        private bool system;
        private DateTime dateCreated;
        private DateTime dateModified;

        [DataMember]
        public string Example
        {
            get { return example; }
            set { example = value; }
        }

        [DataMember]
        public bool System
        {
            get { return system; }
            set { system = value; }
        }

        [DataMember]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        [DataMember]
        public DateTime DateModified
        {
            get { return dateModified; }
            set { dateModified = value; }
        }

        public DatabaseObjectMetadata()
        {
            InitializeMembers();
        }

        public DatabaseObjectMetadata(DatabaseObjectMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.example = String.Empty;
            this.system = false;
            this.dateCreated = DateTime.MinValue;
            this.dateModified = DateTime.MinValue;
        }

        private void CopyMembers(DatabaseObjectMetadata old)
        {
            this.example = old.example;
            this.system = old.system;
            this.dateCreated = old.dateCreated;
            this.dateModified = old.dateModified;
        }

        public object Clone()
        {
            return new DatabaseObjectMetadata(this);
        }
    }
}
