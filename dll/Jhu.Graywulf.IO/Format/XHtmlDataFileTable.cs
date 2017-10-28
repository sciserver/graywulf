using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        protected internal override Task OnReadHeaderAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> OnReadNextRowPartsAsync(IList<string> parts, bool skipComments)
        {
            throw new NotImplementedException();
        }

        protected internal override Task OnReadToFinishAsync()
        {
            throw new NotImplementedException();
        }

        protected internal override Task OnReadFooterAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the resource header into the stream.
        /// </summary>
        protected override async Task OnWriteHeaderAsync()
        {
            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTable, null);
            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTHead, null);
            
            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTR, null);

            // Write columns
            for (int i = 0; i < Columns.Count; i++)
            {
                await File.XmlWriter.WriteElementStringAsync(null, Constants.HtmlKeywordTH, null, Columns[i].Name);
            }

            await File.XmlWriter.WriteEndElementAsync();   // tr
            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTR, null);

            // Write columns
            for (int i = 0; i < Columns.Count; i++)
            {
                await File.XmlWriter.WriteElementStringAsync(null, Constants.HtmlKeywordTH, null, Columns[i].DataType.TypeNameWithLength);
            }

            await File.XmlWriter.WriteEndElementAsync();   // tr
            
            await File.XmlWriter.WriteEndElementAsync();   // thead

            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTBody, null);
        }

        /// <summary>
        /// Writes the next row into the stream.
        /// </summary>
        /// <param name="values"></param>
        protected override async Task OnWriteNextRowAsync(object[] values)
        {
            await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTR, null);
            
            for (int i = 0; i < Columns.Count; i++)
            {
                if (values[i] == DBNull.Value)
                {
                    // Leave field blank
                    await File.XmlWriter.WriteStartElementAsync(null, Constants.HtmlKeywordTD, null);
                    await File.XmlWriter.WriteEndElementAsync();
                }
                else
                {
                    await File.XmlWriter.WriteElementStringAsync(null, Constants.HtmlKeywordTD, null, ColumnFormatters[i](values[i], "{0}"));
                }
            }
            
            await File.XmlWriter.WriteEndElementAsync();   // tr
        }

        /// <summary>
        /// Writers the resource footer into the stream.
        /// </summary>
        protected override async Task OnWriteFooterAsync()
        {
            await File.XmlWriter.WriteEndElementAsync();   // tbody
            await File.XmlWriter.WriteEndElementAsync();   // table
        }
    }
}
