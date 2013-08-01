using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public class DatabaseObjectMetadata
    {
        private string summary;
        private string remarks;
        private string example;

        public string Summary 
        {
            get { return summary; }
            set { summary = value; }
        }

        public string Remarks 
        {
            get { return remarks; }
            set { remarks = value; }
        }

        public string Example
        {
            get { return example; }
            set { example = value; }
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
            this.summary = String.Empty;
            this.remarks = String.Empty;
            this.example = String.Empty;
        }

        private void CopyMembers(DatabaseObjectMetadata old)
        {
            this.summary = old.summary;
            this.remarks = old.remarks;
            this.example = old.example;
        }
    }
}
