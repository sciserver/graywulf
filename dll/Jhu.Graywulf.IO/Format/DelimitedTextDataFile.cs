using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Runtime.Serialization;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DelimitedTextDataFile : TextDataFileBase, IDisposable, ICloneable
    {
        [NonSerialized]
        private bool isFirstBlock;

        [NonSerialized]
        private char comment;

        [NonSerialized]
        private char quote;

        [NonSerialized]
        private string columnSeparators;

        #region Properties

        [DataMember]
        public char Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        [DataMember]
        public char Quote
        {
            get { return quote; }
            set { quote = value; }
        }

        [DataMember]
        public string ColumnSeparators
        {
            get { return columnSeparators; }
            set { columnSeparators = value; }
        }

        #endregion
        #region Constructors and initializers

        public DelimitedTextDataFile()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public DelimitedTextDataFile(DelimitedTextDataFile old)
            : base(old)
        {
            CopyMembers(old);
        }

        public DelimitedTextDataFile(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode, encoding, culture)
        {
            InitializeMembers(new StreamingContext());

            Open();
        }

        public DelimitedTextDataFile(Uri uri, DataFileMode fileMode)
            : this(uri, fileMode, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public DelimitedTextDataFile(Stream stream, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(stream, fileMode, encoding, culture)
        {
            InitializeMembers(new StreamingContext());

            Open();
        }

        public DelimitedTextDataFile(Stream stream, DataFileMode fileMode)
            : this(stream, fileMode, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public DelimitedTextDataFile(TextReader inputReader, Encoding encoding, CultureInfo culture)
            : base(inputReader, encoding, culture)
        {
            InitializeMembers(new StreamingContext());

            Open();
        }

        public DelimitedTextDataFile(TextReader inputReader)
            : this(inputReader, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public DelimitedTextDataFile(TextWriter outputWriter, Encoding encoding, CultureInfo culture)
            : base(outputWriter, encoding, culture)
        {
            InitializeMembers(new StreamingContext());
        }

        public DelimitedTextDataFile(TextWriter outputWriter)
            : this(outputWriter, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            Description = new FileFormatDescription()
            {
                DisplayName = FileFormatNames.CsvFile,
                MimeType = Constants.MimeTypeCsv,
                Extension = Constants.FileExtensionCsv,
                CanRead = true,
                CanWrite = true,
                CanDetectColumnNames = true,
                CanHoldMultipleDatasets = false,
                RequiresArchive = false,
                IsCompressed = false,
                KnowsRecordCount = false,
                RequiresRecordCount = false,
            };

            this.isFirstBlock = true;

            this.comment = '#';
            this.quote = '"';
            this.columnSeparators = ",";
        }

        private void CopyMembers(DelimitedTextDataFile old)
        {
            this.isFirstBlock = true;

            this.comment = old.comment;
            this.quote = old.quote;
            this.columnSeparators = old.columnSeparators;
        }

        public override object Clone()
        {
            return new DelimitedTextDataFile(this);
        }

        #endregion
        #region Stream open and close

        public override void Open(TextReader inputReader)
        {
            base.Open(inputReader);

            Open();
        }

        public override void Open(TextWriter outputWriter)
        {
            base.Open(outputWriter);

            Open();
        }

        #endregion
        #region DataFileBase overloads

        /// <summary>
        /// Advanced the file to the next block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        protected override DataFileBlockBase OnReadNextBlock(DataFileBlockBase block)
        {
            // Only allow calling this function once
            if (!isFirstBlock)
            {
                return null;
            }

            isFirstBlock = false;

            // Create a new block object, if necessary
            return block ?? new DelimitedTextDataFileBlock(this);
        }

        protected internal override void OnReadFooter()
        {
            // No footer in csv files
        }

        /// <summary>
        /// Initializes writing the next block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        /// If next block
        protected override DataFileBlockBase OnCreateNextBlock(DataFileBlockBase block)
        {
            if (!isFirstBlock)
            {
                throw new InvalidOperationException();
                // CSV files can contain a single file block only
            }

            return block ?? new DelimitedTextDataFileBlock(this);
        }

        #endregion
    }
}
