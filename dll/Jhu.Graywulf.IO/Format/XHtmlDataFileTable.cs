using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality responsible for reading and writing a single
    /// table within an html file.
    /// </summary>
    [Serializable]
    public class XHtmlDataFileTable : XmlDataFileBlock, ICloneable
    {
        #region Properties
        
        /// <summary>
        /// Gets the objects wrapping the whole html file.
        /// </summary>
        private XHtmlDataFile File
        {
            get { return (XHtmlDataFile)file; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Initializes a new html table object.
        /// </summary>
        /// <param name="file"></param>
        public XHtmlDataFileTable(XHtmlDataFile file)
            : base(file)
        {
            InitializeMembers();
        }

        public XHtmlDataFileTable(XHtmlDataFileTable old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(XHtmlDataFileTable old)
        {
        }

        public override object Clone()
        {
            return new XHtmlDataFileTable(this);
        }

        #endregion

        protected internal override void OnReadHeader()
        {
            throw new NotImplementedException();
        }

        protected override bool ReadNextRowParts(out string[] parts, bool skipComments)
        {
            throw new NotImplementedException();
        }

        protected internal override void OnReadToFinish()
        {
            throw new NotImplementedException();
        }

        protected internal override void OnReadFooter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the resource header into the stream.
        /// </summary>
        protected override void OnWriteHeader()
        {
            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTable);
            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTHead);
            
            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTR);

            // Write columns
            for (int i = 0; i < Columns.Count; i++)
            {
                File.XmlWriter.WriteElementString(Constants.HtmlKeywordTH, Columns[i].Name);
            }

            File.XmlWriter.WriteEndElement();   // tr
            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTR);

            // Write columns
            for (int i = 0; i < Columns.Count; i++)
            {
                File.XmlWriter.WriteElementString(Constants.HtmlKeywordTH, Columns[i].DataType.TypeNameWithLength);
            }

            File.XmlWriter.WriteEndElement();   // tr
            
            File.XmlWriter.WriteEndElement();   // thead

            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTBody);
        }

        /// <summary>
        /// Writes the next row into the stream.
        /// </summary>
        /// <param name="values"></param>
        protected override void OnWriteNextRow(object[] values)
        {
            File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTR);
            
            for (int i = 0; i < Columns.Count; i++)
            {
                if (values[i] == DBNull.Value)
                {
                    // Leave field blank
                    File.XmlWriter.WriteStartElement(Constants.HtmlKeywordTD);
                    File.XmlWriter.WriteEndElement();
                }
                else
                {
                    File.XmlWriter.WriteElementString(Constants.HtmlKeywordTD, ColumnFormatters[i](values[i], "{0}"));
                }
            }
            
            File.XmlWriter.WriteEndElement();   // tr
        }

        /// <summary>
        /// Writers the resource footer into the stream.
        /// </summary>
        protected override void OnWriteFooter()
        {
            File.XmlWriter.WriteEndElement();   // tbody
            File.XmlWriter.WriteEndElement();   // table
        }
    }
}
