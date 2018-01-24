using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DatasetMetadata : Metadata, ICloneable
    {
        [NonSerialized]
        private DateTime dateCreated;

        [NonSerialized]
        private DateTime dateModified;

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

        public DatasetMetadata()
        {
            InitializeMembers();
        }

        public DatasetMetadata(DatasetMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dateCreated = DateTime.MinValue;
            this.dateModified = DateTime.MinValue;
        }

        private void CopyMembers(DatasetMetadata old)
        {
            this.dateCreated = old.dateCreated;
            this.dateModified = old.dateModified;
        }

        public object Clone()
        {
            return new DatasetMetadata(this);
        }
    }
}
