using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public class VariableMetadata
    {
        private string summary;
        private string unit;
        private string content;

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        //public string Utype { get; set; }
        //public string Enum { get; set; }
        //public string UCD { get; set; }

        public VariableMetadata()
        {
            InitializeMembers();
        }

        public VariableMetadata(VariableMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.summary = String.Empty;
            this.unit = String.Empty;
            this.content = String.Empty;
        }

        private void CopyMembers(VariableMetadata old)
        {
            this.summary = old.summary;
            this.unit = old.unit;
            this.content = old.content;
        }
    }
}
