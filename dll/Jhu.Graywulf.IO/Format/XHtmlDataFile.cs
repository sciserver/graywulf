using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality to read and write VOTables.
    /// </summary>
    [Serializable]
    public class XHtmlDataFile : XmlDataFile, IDisposable, ICloneable
    {
        #region Constructors and initializers

        /// <summary>
        /// Initializes a VOTable object without opening any underlying stream.
        /// </summary>
        public XHtmlDataFile()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public XHtmlDataFile(XHtmlDataFile old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes a VOTable object by automatically opening an underlying stream
        /// identified by an URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        /// <param name="encoding"></param>
        public XHtmlDataFile(Uri uri, DataFileMode fileMode, Encoding encoding)
            : base(uri, fileMode, encoding)
        {
            InitializeMembers(new StreamingContext());

            Open();
        }

        public XHtmlDataFile(Uri uri, DataFileMode fileMode)
            : this(uri, fileMode, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes a VOTable object by automatically wrapping and already open binary stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        /// <param name="encoding"></param>
        public XHtmlDataFile(Stream stream, DataFileMode fileMode, Encoding encoding)
            : base(stream, fileMode, encoding)
        {
            InitializeMembers(new StreamingContext());

            Open();
        }

        public XHtmlDataFile(Stream stream, DataFileMode fileMode)
            : this(stream, fileMode, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes a VOTable object by re-using an already open xml reader.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <param name="encoding"></param>
        public XHtmlDataFile(XmlReader inputReader, Encoding encoding)
            : base(inputReader, encoding)
        {
            InitializeMembers(new StreamingContext());
        }

        public XHtmlDataFile(XmlReader inputReader)
            : this(inputReader, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes a VOTable object by re-using an already open xml writer.
        /// </summary>
        /// <param name="outputWriter"></param>
        /// <param name="encoding"></param>
        public XHtmlDataFile(XmlWriter outputWriter, Encoding encoding)
            : base(outputWriter, encoding)
        {
            InitializeMembers(new StreamingContext());
        }

        public XHtmlDataFile(XmlWriter outputWriter)
            : this(outputWriter, Encoding.UTF8)
        {
            // Overload
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            Description = new FileFormatDescription()
            {
                DisplayName = FileFormatNames.XHtmlDataFile,
                MimeType = Constants.MimeTypeHtml,
                Extension = Constants.FileExtensionHtml,
                CanRead = false,
                CanWrite = true,
                CanDetectColumnNames = false,
                CanHoldMultipleDatasets = true,
                RequiresArchive = false,
                IsCompressed = false,
                KnowsRecordCount = false,
                RequiresRecordCount = false,
            };
        }

        private void CopyMembers(XHtmlDataFile old)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override object Clone()
        {
            return new XHtmlDataFile(this);
        }

        #endregion

        protected override void OnBlockAppended(DataFileBlockBase block)
        {
            throw new NotImplementedException();
        }

        protected internal override void OnReadHeader()
        {
            throw new NotImplementedException();
        }

        protected override DataFileBlockBase OnReadNextBlock(DataFileBlockBase block)
        {
            throw new NotImplementedException();
        }

        protected internal override void OnReadFooter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the file header but stops before the block header.
        /// </summary>
        /// <remarks>
        /// Writes from VOTABLE until the RESOURCE tag.
        /// </remarks>
        protected override void OnWriteHeader()
        {
            XmlWriter.WriteStartElement(Constants.HtmlKeywordHtml);
            XmlWriter.WriteStartElement(Constants.HtmlKeywordBody);
            XmlWriter.WriteStartElement(Constants.HtmlKeywordHead);

            XmlWriter.WriteElementString(Constants.HtmlKeywordStyle, XHtmlDataFileResources.XHtmlDataFileStyle);

            XmlWriter.WriteEndElement();    // head
        }

        /// <summary>
        /// Initializes writing of the next block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override DataFileBlockBase OnCreateNextBlock(DataFileBlockBase block)
        {
            return block ?? new XHtmlDataFileTable(this);
        }

        /// <summary>
        /// Writes the file footer.
        /// </summary>
        /// <remarks>
        /// Writes tags after the last RESOURCE tag till the closing VOTABLE
        /// </remarks>
        protected override void OnWriteFooter()
        {
            XmlWriter.WriteEndElement();    // body
            XmlWriter.WriteEndElement();    // html
        }
    }
}
