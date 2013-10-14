using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public abstract class TextDataFile : DataFileBase, IDisposable
    {
        #region Parser and formatter delegates

        protected delegate bool ParserDelegate(string value, out object result);
        protected delegate string FormatterDelegate(object value, string format);

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

        [NonSerialized]
        private long lineCounter;
        [NonSerialized]
        private List<string> lineBuffer;
        [NonSerialized]
        private bool lineBufferOn;

        private bool detectEncoding;
        private Encoding encoding;
        private CultureInfo culture;
        private NumberStyles numberStyle;
        private DateTimeStyles dateTimeStyle;

        private bool generateIdentity;
        private int skipLinesCount;
        private bool columnNamesInFirstLine;
        private int rowsToDetectColumns;

        [NonSerialized]
        private ParserDelegate[] columnParsers;
        [NonSerialized]
        private FormatterDelegate[] columnFormatters;

        #endregion
        #region Properties

        public bool DetectEncoding
        {
            get { return detectEncoding; }
            set { detectEncoding = value; }
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public CultureInfo Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        protected long LineCounter
        {
            get { return lineCounter; }
        }

        public bool GenerateIdentity
        {
            get { return generateIdentity; }
            set { generateIdentity = value; }
        }

        public int SkipLinesCount
        {
            get { return skipLinesCount; }
            set { skipLinesCount = value; }
        }

        public bool ColumnNamesInFirstLine
        {
            get { return columnNamesInFirstLine; }
            set { columnNamesInFirstLine = value; }
        }

        public int RowsToDetectColumns
        {
            get { return rowsToDetectColumns; }
            set { rowsToDetectColumns = value; }
        }

        protected ParserDelegate[] ColumnParsers
        {
            get { return columnParsers; }
        }

        protected FormatterDelegate[] ColumnFormatters
        {
            get { return columnFormatters; }
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
        {
            InitializeMembers();

            Open(output, encoding, culture);
        }

        protected TextDataFile(string path, bool detectEncoding)
            : this(path, detectEncoding, null)
        {
            // overload
        }

        protected TextDataFile(string path, bool detectEncoding, CultureInfo culture)
            : base(path, DataFileMode.Read)
        {
            InitializeMembers();

            this.detectEncoding = detectEncoding;
            this.encoding = null;

            this.culture = culture;
        }

        protected TextDataFile(string path, DataFileMode fileMode)
            : this(path, fileMode, null, null)
        {
            // overload
        }

        protected TextDataFile(string path, DataFileMode fileMode, CultureInfo culture)
            : this(path, fileMode, null, culture)
        {
            // overload
        }

        protected TextDataFile(string path, DataFileMode fileMode, Encoding encoding)
            : this(path, fileMode, encoding, null)
        {
            // overload
        }

        protected TextDataFile(string path, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(path, fileMode)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            InitializeMembers();

            this.detectEncoding = false;
            this.encoding = encoding;
            this.culture = culture;
        }

        private void InitializeMembers()
        {
            this.inputReader = null;
            this.ownsInputReader = false;

            this.outputWriter = null;
            this.ownsOutputWriter = false;

            this.lineCounter = 0;
            this.lineBuffer = null;
            this.lineBufferOn = false;

            this.detectEncoding = true;
            this.encoding = null;
            this.culture = null;
            this.numberStyle = NumberStyles.Float;
            this.dateTimeStyle = DateTimeStyles.None;

            this.generateIdentity = true;
            this.skipLinesCount = 0;
            this.columnNamesInFirstLine = true;
            this.rowsToDetectColumns = 1000;
        }

        public override void Dispose()
        {
            Close();

            lineBuffer = null;

            base.Dispose();
        }

        #endregion

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
            this.culture = culture;
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
            this.culture = culture;
            this.encoding = encoding;
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
                if (encoding == null)
                {
                    inputReader = new StreamReader(base.Stream);
                }
                else
                {
                    inputReader = new StreamReader(base.Stream, encoding);
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
                if (encoding == null)
                {
                    outputWriter = new StreamWriter(base.Stream);
                }
                else
                {
                    outputWriter = new StreamWriter(base.Stream, encoding);
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

        #region Buffer logic

        protected void StartLineBuffer()
        {
            // Line buffer can only be turned on at the beginning
            if (lineCounter > 0)
            {
                throw new InvalidOperationException();
            }

            if (lineBuffer != null)
            {
                throw new InvalidOperationException();
            }

            lineBuffer = new List<string>();
            lineBufferOn = true;
        }

        protected void StopLineBuffer()
        {
            lineBufferOn = false;
        }

        protected void RewindLineBuffer()
        {
            if (lineBuffer == null && lineCounter > 0)
            {
                throw new InvalidOperationException();
            }

            lineCounter = 0;
        }

        /// <summary>
        /// Reads the next line from the input stream.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <returns></returns>
        /// 
        protected string ReadLine()
        {
            // See if there are lines in the buffer (prefetched for column detection)
            // If so, use lines in buffer, otherwise read from the stream
            if (lineBuffer != null && lineCounter < lineBuffer.Count)
            {
                return lineBuffer[(int)lineCounter++];
            }
            else
            {
                string line = inputReader.ReadLine();

                // Store in buffer if it's turned on
                if (lineBufferOn)
                {
                    lineBuffer.Add(line);
                }

                lineCounter++;

                if (line == null && ownsInputReader)
                {
                    Close();
                }

                return line;
            }
        }

        protected void WriteLine(string line)
        {
            outputWriter.WriteLine(line);
        }

        #endregion

        public void DetectColumns(int rowCount)
        {
            if (IsClosed)
            {
                OpenForRead();
            }

            Columns.Clear();
            OnDetectColumns(rowCount);

            if (generateIdentity)
            {
                Columns.Insert(0, DataFileColumn.Identity);
            }
        }

        protected abstract void OnDetectColumns(int rowCount);

        /// <summary>
        /// Detects columns from a set of values
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="useNames"></param>
        /// <param name="cols"></param>
        /// <param name="colRanks"></param>
        protected void DetectColumnsFromParts(string[] parts, bool useNames, out DataFileColumn[] cols, out int[] colRanks)
        {
            cols = new DataFileColumn[parts.Length];
            colRanks = new int[parts.Length];

            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = new DataFileColumn();

                if (useNames)
                {
                    cols[i].Name = parts[i].Trim();
                }
                else
                {
                    cols[i].Name = String.Format("Col{0}", i);
                }
            }
        }

        /// <summary>
        /// Detect column types from a set of values
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="cols"></param>
        /// <param name="colranks"></param>
        protected void DetectColumnTypes(string[] parts, DataFileColumn[] cols, int[] colranks)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Type type;
                int size, rank;
                if (!GetBestColumnTypeEstimate(parts[i], out type, out size, out rank))
                {
                    cols[i].IsNullable = true;
                }

                if (cols[i].DataType == null || colranks[i] < rank)
                {
                    cols[i].DataType = DataType.Create(type);
                    cols[i].DataType.Size = (short)size;
                }

                if (cols[i].DataType.Size < size)
                {
                    cols[i].DataType.Size = (short)size;
                }
            }
        }

        protected override void OnColumnsCreated()
        {
            base.OnColumnsCreated();

            if ((FileMode & DataFileMode.Read) != 0)
            {
                InitializeColumnParsers();
            }

            if ((FileMode & DataFileMode.Write) != 0)
            {
                InitializeColumnFormatters();
            }
        }

        private void InitializeColumnParsers()
        {
            columnParsers = new ParserDelegate[Columns.Count];

            for (int i = 0; i < columnParsers.Length; i++)
            {
                columnParsers[i] = GetParserDelegate(Columns[i]);
            }
        }

        private void InitializeColumnFormatters()
        {
            columnFormatters = new FormatterDelegate[Columns.Count];

            for (int i = 0; i < columnFormatters.Length; i++)
            {
                columnFormatters[i] = GetFormatterDelegate(Columns[i]);
            }
        }

        protected virtual ParserDelegate GetParserDelegate(DataFileColumn column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(string s, out object p)
                {
                    SByte v;
                    var r = SByte.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(string s, out object p)
                {
                    Int16 v;
                    var r = Int16.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(string s, out object p)
                {
                    Int32 v;
                    var r = Int32.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(string s, out object p)
                {
                    Int64 v;
                    var r = Int64.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(string s, out object p)
                {
                    Byte v;
                    var r = Byte.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(string s, out object p)
                {
                    UInt16 v;
                    var r = UInt16.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(string s, out object p)
                {
                    UInt32 v;
                    var r = UInt32.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(string s, out object p)
                {
                    UInt64 v;
                    var r = UInt64.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(bool))
            {
                // *** TODO: parse numbers!
                return delegate(string s, out object p)
                {
                    bool v;
                    var r = bool.TryParse(s, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(float))
            {
                return delegate(string s, out object p)
                {
                    float v;
                    var r = float.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(double))
            {
                return delegate(string s, out object p)
                {
                    double v;
                    var r = double.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(decimal))
            {
                return delegate(string s, out object p)
                {
                    decimal v;
                    var r = decimal.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(char))
            {
                return delegate(string s, out object p)
                {
                    if (s.Length == 1)
                    {
                        p = s[0];
                        return true;
                    }
                    else
                    {
                        p = null;
                        return false;
                    }
                };
            }
            else if (t == typeof(string))
            {
                return delegate(string s, out object p)
                {
                    p = s;
                    return true;
                };
            }
            else if (t == typeof(DateTime))
            {
                return delegate(string s, out object p)
                {
                    DateTime v;
                    var r = DateTime.TryParse(s, culture, dateTimeStyle, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Guid))
            {
                return delegate(string s, out object p)
                {
                    Guid v;
                    var r = Guid.TryParse(s, out v);
                    p = v;
                    return r;
                };
            }

            throw new NotImplementedException();
        }

        protected virtual FormatterDelegate GetFormatterDelegate(DataFileColumn column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (SByte)o);
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int16)o);
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int32)o);
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int64)o);
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Byte)o);
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt16)o);
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt32)o);
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt64)o);
                };
            }
            else if (t == typeof(bool))
            {
                // TODO: use numbers?
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (bool)o);
                };
            }
            else if (t == typeof(float))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (float)o);
                };
            }
            else if (t == typeof(double))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (double)o);
                };
            }
            else if (t == typeof(decimal))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (decimal)o);
                };
            }
            else if (t == typeof(char))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (string)o);
                };
            }
            else if (t == typeof(string))
            {
                return delegate(object o, string f)
                {
                    return (string)o;
                };
            }
            else if (t == typeof(DateTime))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (DateTime)o);
                };
            }
            else if (t == typeof(Guid))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Guid)o);
                };
            }

            throw new NotImplementedException();
        }

        protected bool GetBestColumnTypeEstimate(string value, out Type type, out int size, out int rank)
        {
            if (String.IsNullOrEmpty(value))
            {
                type = null;
                size = 0;
                rank = 0;
                return false;
            }

            // Try from the simplest to the most complex syntax

            // Only assume Int32 or Int64 for integers. ID's might start from low numbers and the first 1000
            // rows would suggest an ID is only Int16 while it is Int32.

            // double and float are indistinguishable based on their string representations
            // while the number of decimal digits could be used, standard .net functions don't provide
            // this functionality, so we always assume double by default

            rank = 0;

            /*
            Byte bytev;
            if (Byte.TryParse(value, numberStyle, culture, out bytev))
            {
                type = typeof(Byte);
                size = 0;
                return true;
            }
            rank++;

            /*UInt16 uint16v;
            if (UInt16.TryParse(value, numberStyle, culture, out uint16v))
            {
                type = typeof(UInt16);
                size = 0;
                return true;
            }
            rank++;

            UInt32 uint32v;
            if (UInt32.TryParse(value, numberStyle, culture, out uint32v))
            {
                type = typeof(UInt32);
                size = 0;
                return true;
            }
            rank++;

            UInt64 uint64v;
            if (UInt64.TryParse(value, numberStyle, culture, out uint64v))
            {
                type = typeof(UInt64);
                size = 0;
                return true;
            }
            rank++;*/

            /*
            SByte sbytev;
            if (SByte.TryParse(value, numberStyle, culture, out sbytev))
            {
                type = typeof(SByte);
                size = 0;
                return true;
            }
            rank++;*/

            /*
            Int16 int16v;
            if (Int16.TryParse(value, numberStyle, culture, out int16v))
            {
                type = typeof(Int16);
                size = 0;
                return true;
            }
            rank++;*/

            Int32 int32v;
            if (Int32.TryParse(value, numberStyle, culture, out int32v))
            {
                type = typeof(Int32);
                size = 0;
                return true;
            }
            rank++;

            Int64 int64v;
            if (Int64.TryParse(value, numberStyle, culture, out int64v))
            {
                type = typeof(Int64);
                size = 0;
                return true;
            }
            rank++;

            /*
            float floatv;
            if (float.TryParse(value, numberStyle, culture, out floatv))
            {
                type = typeof(float);
                size = 0;
                return true;
            }
            rank++;*/

            /*
            decimal decimalv;
            if (decimal.TryParse(value, numberStyle, culture, out decimalv))
            {
                type = typeof(decimal);
                size = 0;
                return true;
            }
            rank++;*/

            double doublev;
            if (double.TryParse(value, numberStyle, culture, out doublev))
            {
                type = typeof(double);
                size = 0;
                return true;
            }
            rank++;

            Guid guidv;
            if (Guid.TryParse(value, out guidv))
            {
                type = typeof(Guid);
                size = 0;
                return true;
            }
            rank++;

            DateTime datetimev;
            if (DateTime.TryParse(value, culture, dateTimeStyle, out datetimev))
            {
                type = typeof(DateTime);
                size = 0;
                return true;
            }
            rank++;

            if (value.Length == 1)
            {
                type = typeof(char);
                size = 0;
                return true;
            }
            rank++;

            bool boolv;
            if (bool.TryParse(value, out boolv))
            {
                type = typeof(bool);
                size = 0;
                return true;
            }
            rank++;

            // Last case: it's a string
            type = typeof(string);
            size = value.Length;
            return true;
        }

    }

}
