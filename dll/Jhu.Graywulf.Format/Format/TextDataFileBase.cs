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
    public abstract class TextDataFileBase : FormattedDataFileBase, IDisposable
    {
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

        protected internal TextWriter TextWriter
        {
            get { return outputWriter; }
        }

        #endregion
        #region Constructors and initializers

        protected TextDataFileBase()
        {
            InitializeMembers();
        }

        protected TextDataFileBase(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode, encoding, culture)
        {
            InitializeMembers();
        }

        protected TextDataFileBase(Stream stream, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(stream, fileMode, encoding, culture)
        {
            InitializeMembers();
        }

        protected TextDataFileBase(TextReader inputReader, Encoding encoding, CultureInfo culture)
            : base((Stream)null, DataFileMode.Read, encoding, culture)
        {
            InitializeMembers();

            this.inputReader = inputReader;
        }

        protected TextDataFileBase(TextWriter outputWriter, Encoding encoding, CultureInfo culture)
            : base((Stream)null, DataFileMode.Write, encoding, culture)
        {
            InitializeMembers();

            this.outputWriter = outputWriter;
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

        public virtual void Open(TextReader inputReader)
        {
            OpenStream(null, DataFileMode.Read);

            this.inputReader = inputReader;
        }

        public virtual void Open(TextWriter outputWriter)
        {
            OpenStream(null, DataFileMode.Write);

            this.outputWriter = outputWriter;
        }

        protected override void EnsureNotOpen()
        {
            if ((ownsInputReader && inputReader != null) ||
                (ownsOutputWriter && outputWriter != null))
            {
                throw new InvalidOperationException();
            }
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
                base.OpenForRead();

                // Open text reader
                // Wrap underlying stream, so it doesn't get disposed automatically
                if (base.Encoding == null)
                {
                    inputReader = new StreamReader(new DetachedStream( base.BaseStream));
                }
                else
                {
                    inputReader = new StreamReader(new DetachedStream(base.BaseStream), base.Encoding);
                }

                this.ownsInputReader = true;
            }
        }

        protected override void OpenForWrite()
        {
            if (outputWriter == null)
            {
                base.OpenForWrite();

                // Open TextWriter
                // Wrap underlying stream, so it doesn't get disposed automatically
                if (base.Encoding == null)
                {
                    outputWriter = new StreamWriter(new DetachedStream(base.BaseStream));
                }
                else
                {
                    outputWriter = new StreamWriter(new DetachedStream(base.BaseStream), base.Encoding);
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
            }

            inputReader = null;
            ownsInputReader = false;

            if (ownsOutputWriter && outputWriter != null)
            {
                outputWriter.Flush();
                outputWriter.Close();
                outputWriter.Dispose();
            }

            outputWriter = null;
            ownsOutputWriter = false;

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
            /* TODO: maybe remove
            // Make sure only one block is added
            if (Blocks.Count > 1)
            {
                throw new InvalidOperationException("Text files must consist of a single block.");
            }*/
        }

        protected override void OnWriteHeader()
        {
            // Header is written by the first block
        }

        protected override void OnWriteFooter()
        {
            // No footer in text files
        }
    }
}
