using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;
using System.Xml;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public class CsvFile : TextDataFile, IDisposable
    {
        [NonSerialized]
        private bool nextResultsCalled;

        private char comment;
        private char quote;
        private char separator;

        #region Properties

        public override FileFormatDescription Description
        {
            get
            {
                return new FileFormatDescription()
                    {
                        DisplayName = FileFormatNames.Jhu_Graywulf_Format_CsvFile,
                        DefaultExtension = Constants.FileExtensionCsv,
                        CanRead = true,
                        CanWrite = true,
                        CanDetectColumnNames = true,
                        MultipleDatasets = false,
                        IsCompressed = false,
                    };
            }
        }

        public char Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public char Quote
        {
            get { return quote; }
            set { quote = value; }
        }

        public char Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        #endregion
        #region Constructors and initializers

        protected CsvFile()
            : base()
        {
            InitializeMembers();
        }

        public CsvFile(Uri uri, DataFileMode fileMode, CompressionMethod compression, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode, compression, encoding, culture)
        {
            InitializeMembers();

            Open();
        }

        public CsvFile(Uri uri, DataFileMode fileMode)
            :this(uri, fileMode, CompressionMethod.Automatic, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public CsvFile(Stream stream, DataFileMode fileMode, CompressionMethod compression, Encoding encoding, CultureInfo culture)
            : base(stream, fileMode, compression, encoding, culture)
        {
            InitializeMembers();

            Open();
        }

        public CsvFile(Stream stream, DataFileMode fileMode)
            :this(stream, fileMode, CompressionMethod.None, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public CsvFile(TextReader inputReader, Encoding encoding, CultureInfo culture)
            : base(inputReader, encoding, culture)
        {
            InitializeMembers();
        }

        public CsvFile(TextReader inputReader)
            : this(inputReader, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        public CsvFile(TextWriter outputWriter, Encoding encoding, CultureInfo culture)
            : base(outputWriter, encoding, culture)
        {
            InitializeMembers();
        }

        public CsvFile(TextWriter outputWriter)
            : this(outputWriter, Encoding.ASCII, CultureInfo.InvariantCulture)
        {
            // Overload
        }

        private void InitializeMembers()
        {
            this.nextResultsCalled = false;

            this.comment = '#';
            this.quote = '"';
            this.separator = ',';
        }

        #endregion
        #region Stream open and close

        public override void Open(Stream stream, DataFileMode fileMode)
        {
            base.Open(stream, fileMode);

            Open();
        }

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
            if (nextResultsCalled)
            {
                return null;
            }

            nextResultsCalled = true;

            // Create a new block object, if necessary
            return block ?? new CsvFileBlock(this);
        }

        protected internal override void OnReadFooter()
        {
            // No footer in csv files
        }

        protected override DataFileBlockBase OnWriteNextBlock(DataFileBlockBase block, IDataReader dr)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
