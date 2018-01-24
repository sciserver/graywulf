using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class DatabaseObjectMetadata : Metadata, ICloneable
    {
        [NonSerialized]
        private string example;

        [NonSerialized]
        private bool system;

        [NonSerialized]
        private DateTime dateCreated;

        [NonSerialized]
        private DateTime dateModified;

        [NonSerialized]
        private string @class;

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

        [DataMember]
        public string Class
        {
            get { return @class; }
            set { @class = value; }
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
            this.@class = String.Empty;
        }

        private void CopyMembers(DatabaseObjectMetadata old)
        {
            this.example = old.example;
            this.system = old.system;
            this.dateCreated = old.dateCreated;
            this.dateModified = old.dateModified;
            this.@class = old.@class;
        }

        public object Clone()
        {
            return new DatabaseObjectMetadata(this);
        }
    }
}
