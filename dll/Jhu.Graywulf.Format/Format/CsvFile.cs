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
                        DefaultExtension = ".csv",
                        CanCompress = true,
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

        public CsvFile()
            :base()
        {
            InitializeMembers();
        }

        public CsvFile(TextReader input)
            : this(input, null)
        {
            // overload
        }

        public CsvFile(TextReader input, CultureInfo culture)
            : base(input, culture)
        {
            InitializeMembers();
        }

        public CsvFile(TextWriter output)
            : this(output, null, null)
        {
            // overload
        }

        public CsvFile(TextWriter output, CultureInfo culture)
            : this(output, null, culture)
        {
            // overload
        }

        public CsvFile(TextWriter output, Encoding encoding)
            : this(output, encoding, null)
        {
            // overload
        }

        public CsvFile(TextWriter output, Encoding encoding, CultureInfo culture)
            : base(output, encoding, culture)
        {
            InitializeMembers();
        }

        public CsvFile(Uri uri, DataFileMode fileMode)
            : base(uri, fileMode)
        {
            InitializeMembers();
        }

        public CsvFile(Uri uri, bool detectEncoding)
            : this(uri, detectEncoding, null)
        {
            // overload
        }

        public CsvFile(Uri uri, CultureInfo culture)
            : this(uri, true, culture)
        {
            // overload
        }

        public CsvFile(Uri uri, bool detectEncoding, CultureInfo culture)
            : base(uri, detectEncoding, culture)
        {
            InitializeMembers();
        }

        public CsvFile(Uri uri, DataFileMode fileMode, Encoding encoding)
            : this(uri, fileMode, encoding, null)
        {
            // overload
        }

        public CsvFile(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode, encoding, culture)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.nextResultsCalled = false;

            this.comment = '#';
            this.quote = '"';
            this.separator = ',';
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

#if false

        protected override FormatterDelegate GetFormatterDelegate(DataFileColumn column)
        {
            if (column.DataType.Type == typeof(string))
            {
                return delegate(object o, string f)
                {
                    return String.Format(Culture, "{0}{1}{0}", quote, ((string)o).Replace(quote.ToString(), quote.ToString() + quote.ToString()));
                };
            }
            else
            {
                return base.GetFormatterDelegate(column);
            }
        }
#endif
    }
}
