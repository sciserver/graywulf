using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public abstract class TextDataFile : FormattedDataFile, IDisposable
    {
        #region Parser and formatter delegates

        #endregion
        #region Member variables

        [NonSerialized]
        private TextReader inputReader;
        [NonSerialized]
        private bool ownsInputReader;

        [NonSerialized]
        private TextWriter outputWriter;
        [NonSerialized]
        private bool ownsOutputWriter;

        private int skipLinesCount;
        private bool autoDetectColumns;
        private bool columnNamesInFirstLine;
        private int autoDetectColumnsCount;

        #endregion
        #region Properties

        public int SkipLinesCount
        {
            get { return skipLinesCount; }
            set { skipLinesCount = value; }
        }

        public bool AutoDetectColumns
        {
            get { return autoDetectColumns; }
            set { autoDetectColumns = value; }
        }

        public bool ColumnNamesInFirstLine
        {
            get { return columnNamesInFirstLine; }
            set { columnNamesInFirstLine = value; }
        }

        public int AutoDetectColumnsCount
        {
            get { return autoDetectColumnsCount; }
            set { autoDetectColumnsCount = value; }
        }

        protected internal TextReader TextReader
        {
            get { return inputReader; }
        }

        #endregion
        #region Constructors and initializers

        protected TextDataFile()
        {
            InitializeMembers();
        }

        protected TextDataFile(TextReader input)
            : this(input, null)
        {
            // overload
        }

        protected TextDataFile(TextReader input, CultureInfo culture)
            : base(input,culture)
        {
            InitializeMembers();

            Open(input, culture);
        }

        protected TextDataFile(TextWriter output)
            : this(output, null, null)
        {
            // overload
        }

        protected TextDataFile(TextWriter output, CultureInfo culture)
            : this(output, null, culture)
        {
            // overload
        }

        protected TextDataFile(TextWriter output, Encoding encoding)
            : this(output, encoding, null)
        {
            // overload
        }

        protected TextDataFile(TextWriter output, Encoding encoding, CultureInfo culture)
            :base(output,encoding,culture)
        {
            InitializeMembers();

            Open(output, encoding, culture);
        }

        protected TextDataFile(Uri uri, bool detectEncoding)
            : this(uri, detectEncoding, null)
        {
            // overload
        }

        /// <summary>
        /// New constructor with formattedData
        /// </summary>
        /// <param name="path"></param>
        /// <param name="detectEncoding"></param>
        /// <param name="culture"></param>
        protected TextDataFile(Uri uri, bool detectEncoding, CultureInfo culture)
            : base( uri, detectEncoding, culture)
        {
            InitializeMembers();           
        }
        protected TextDataFile(Uri uri, DataFileMode fileMode)
            : this(uri, fileMode, null, null)
        {
            // overload
        }

        protected TextDataFile(Uri uri, DataFileMode fileMode, CultureInfo culture)
            : this(uri, fileMode, null, culture)
        {
            // overload
        }

        protected TextDataFile(Uri uri, DataFileMode fileMode, Encoding encoding)
            : this(uri, fileMode, encoding, null)
        {
            // overload
        }

        protected TextDataFile(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode, encoding,culture)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.inputReader = null;
            this.ownsInputReader = false;

            this.outputWriter = null;
            this.ownsOutputWriter = false;

            this.skipLinesCount = 0;
            this.columnNamesInFirstLine = true;
            this.autoDetectColumnsCount = 1000;
        }

        public override void Dispose()
        {
            Close();
            base.Dispose();
        }

        #endregion
        #region Stream open and close

        protected override void EnsureNotOpen()
        {
            if (ownsInputReader && inputReader != null ||
                ownsOutputWriter && outputWriter != null)
            {
                throw new InvalidOperationException();
            }
        }

        public void Open(TextReader input)
        {
            Open(input, null);
        }

        public void Open(TextReader input, CultureInfo culture)
        {
            EnsureNotOpen();

            this.FileMode = DataFileMode.Read;

            this.inputReader = input;
            this.ownsInputReader = false;
            this.Culture = culture;
        }

        public void Open(TextWriter output)
        {
            Open(output, null, null);
        }

        public void Open(TextWriter output, Encoding encoding)
        {
            Open(output, encoding, null);
        }

        public void Open(TextWriter output, CultureInfo culture)
        {
            Open(output, null, culture);
        }

        public void Open(TextWriter output, Encoding encoding, CultureInfo culture)
        {
            EnsureNotOpen();

            this.FileMode = DataFileMode.Write;

            this.outputWriter = output;
            this.ownsOutputWriter = false;
            this.Culture = culture;
            this.Encoding = encoding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This function is called by the infrastructure when starting to read
        /// the file by the FileDataReader
        /// </remarks>
        protected override void OpenForRead()
        {
            if (inputReader == null)
            {
                // No open text reader yet
                base.OpenForRead();

                // Open text reader
                if (base.Encoding == null)
                {
                    inputReader = new StreamReader(base.Stream);
                }
                else
                {
                    inputReader = new StreamReader(base.Stream, base.Encoding);
                }

                this.ownsInputReader = true;
            }
        }

        protected override void OpenForWrite()
        {
            if (outputWriter == null)
            {
                // No open TextWriter yet
                base.OpenForWrite();

                // Open TextWriter
                if (base.Encoding == null)
                {
                    outputWriter = new StreamWriter(base.Stream);
                }
                else
                {
                    outputWriter = new StreamWriter(base.Stream, base.Encoding);
                }

                ownsOutputWriter = true;
            }
        }

        public override void Close()
        {
            if (ownsInputReader && inputReader != null)
            {
                inputReader.Close();
                inputReader.Dispose();
                inputReader = null;
                ownsInputReader = false;
            }

            if (ownsOutputWriter && outputWriter != null)
            {
                outputWriter.Close();
                outputWriter.Dispose();
                outputWriter = null;
                ownsOutputWriter = false;
            }

            base.Close();
        }

        public override bool IsClosed
        {
            get
            {
                switch (FileMode)
                {
                    case DataFileMode.Read:
                        return inputReader == null;
                    case DataFileMode.Write:
                        return outputWriter == null;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        protected internal override void OnReadHeader()
        {
            // Do nothing in case of text files,
            // header will be read by the block
        }

        protected override void OnBlockAppended(DataFileBlockBase block)
        {
            // Make sure only one block is added
            if (Blocks.Count > 1)
            {
                throw new InvalidOperationException("Text files must consist of a single block.");
            }
        }
    }
}
