using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public class DataFileColumnMetadata : IVariableMetadata, ICloneable
    {
        private string summary;
        private string unit;
        private string content;
        private string format;

        // TODO: add scale, zero offset and null

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

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public DataFileColumnMetadata()
        {
            InitializeMembers();
        }

        public DataFileColumnMetadata(DataFileColumnMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.summary = String.Empty;
            this.unit = String.Empty;
            this.content = String.Empty;
            this.format = "{0}";
        }

        private void CopyMembers(DataFileColumnMetadata old)
        {
            this.summary = old.summary;
            this.unit = old.unit;
            this.content = old.content;
            this.format = old.format;
        }

        public object Clone()
        {
            return new DataFileColumnMetadata(this);
        }
    }
}
