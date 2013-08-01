using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public class CsvFile : TextDataFile, IDisposable
    {
        [NonSerialized]
        private bool nextResultsCalled;
        [NonSerialized]
        private long rowCounter;

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

        public CsvFile(string path, DataFileMode fileMode)
            : base(path, fileMode)
        {
            InitializeMembers();
        }

        public CsvFile(string path, bool detectEncoding)
            : this(path, detectEncoding, null)
        {
            // overload
        }

        public CsvFile(string path, CultureInfo culture)
            : this(path, true, culture)
        {
            // overload
        }

        public CsvFile(string path, bool detectEncoding, CultureInfo culture)
            : base(path, detectEncoding, culture)
        {
            InitializeMembers();
        }

        public CsvFile(string path, DataFileMode fileMode, Encoding encoding)
            : this(path, fileMode, encoding, null)
        {
            // overload
        }

        public CsvFile(string path, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(path, fileMode, encoding, culture)
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

        private void SkipLines()
        {
            for (int i = 0; i < SkipLinesCount; i++)
            {
                ReadLine();
            }
        }

        protected override void OnDetectColumns(int rowCount)
        {
            if (LineCounter > 0)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            // Buffering is needed to detect columns automatically
            StartLineBuffer();


            string[] parts;

            DataFileColumn[] cols = null;      // detected columns
            int[] colranks = null;

            if (ColumnNamesInFirstLine)
            {
                parts = ReadLine().TrimStart(comment).Split(separator);
                DetectColumnsFromParts(parts, true, out cols, out colranks);
            }

            // Try to figure out the type of columns from the first n rows
            SkipLines();

            // TODO: add logic to figure out column names

            // Try to read some rows to detect
            int q = 0;
            while (q < rowCount && GetNextLineParts(out parts))
            {
                if (q == 0 && cols == null)
                {
                    DetectColumnsFromParts(parts, false, out cols, out colranks);
                }

                if (cols.Length != parts.Length)
                {
                    throw new FileFormatException();    // TODO
                }

                DetectColumnTypes(parts, cols, colranks);

                q++;
            }

            this.Columns.Clear();
            this.Columns.AddRange(cols);

            // Rewind stream
            RewindLineBuffer();
        }

        private void DetectColumnsFromParts(string[] parts, bool useNames, out DataFileColumn[] cols, out int[] colRanks)
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

        private void DetectColumnTypes(string[] parts, DataFileColumn[] cols, int[] colranks)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Type type;
                int size, rank;
                if (!GetBestColumnTypeEstimate(parts[i], out type, out size, out rank))
                {
                    cols[i].AllowNull = true;
                }

                if (cols[i].Type == null || colranks[i] < rank)
                {
                    cols[i].Type = type;
                    cols[i].Size = size;
                }

                if (cols[i].Size < size)
                {
                    cols[i].Size = size;
                }
            }
        }

        protected override bool OnNextResult()
        {
            // Only allow calling this function once
            if (nextResultsCalled)
            {
                return false;
            }

            nextResultsCalled = true;
            rowCounter = 0;

            // if no columns are set, try to detect them from the file
            if (Columns.Count == 0)
            {
                DetectColumns(RowsToDetectColumns);
            }

            return true;
        }

        protected override bool OnRead(object[] values)
        {
            if (rowCounter == 0)
            {
                StopLineBuffer();
                SkipLines();
            }

            string[] parts;
            var res = GetNextLineParts(out parts);

            if (!res)
            {
                return false;
            }

            // Now parse the parts
            int pi = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].IsIdentity && GenerateIdentity)
                {
                    // IDs are 1 based (like in SQL server)
                    values[i] = rowCounter + 1;     // TODO: might need to convert to a smaller type?
                }
                else
                {
                    if (!ColumnParsers[i](parts[pi], out values[i]))
                    {
                        throw new FormatException();    // TODO: add logic to skip exceptions
                    }

                    pi++;
                }
            }

            // TODO: add logic to handle nulls

            rowCounter++;

            return true;
        }

        #endregion

        /// <summary>
        /// Returns the next line in the file braked up into parts
        /// along separators.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function support quoted strings.
        /// </remarks>
        private bool GetNextLineParts(out string[] parts)
        {
            string line;
            List<string> res = new List<string>();

            int ci = 0;
            bool inquote = false;
            string part = String.Empty;

            while (true)
            {
                if ((line = ReadLine()) == null)
                {
                    parts = null;
                    return false;
                }

                // skip empty lines
                if (!inquote && line.Length == 0)
                {
                    continue;
                }

                // skip comments
                if (!inquote && line[0] == comment)
                {
                    continue;
                }

                // Loop over characters in line
                while (ci <= line.Length)
                {
                    if (inquote && ci == line.Length)   // Inside a quote and end of line reached
                    {
                        line = ReadLine();

                        if (line == null)
                        {
                            throw new Exception();  // TODO: unexpected end of line
                        }

                        ci = 0;
                    }
                    if (!inquote && ci == line.Length || line[ci] == separator)
                    {
                        res.Add(part);
                        part = String.Empty;

                        if (ci == line.Length)  // Outside a quote and end of line reached
                        {
                            parts = res.ToArray();
                            return true;
                        }
                    }
                    else if (line[ci] == quote)     // quote
                    {
                        if (!inquote)
                        {
                            inquote = true;
                        }
                        else
                        {
                            if (ci < line.Length - 1 && line[ci + 1] == quote)
                            {
                                // double quote
                                part += quote;
                                ci++;
                            }

                            inquote = false;
                        }
                    }
                    else
                    {
                        part += line[ci];
                    }

                    ci++;
                }
            }
        }

        #region Writer functions

        protected override void OnWriteHeader()
        {
            var line = new StringBuilder();

            line.Append(comment);

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    line.Append(separator);
                }

                line.Append(Columns[i].Name.Replace(separator, '_'));    // TODO
            }

            WriteLine(line.ToString());
        }

        protected override void OnWrite(object[] values)
        {
            var line = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    line.Append(separator);
                }

                line.Append(ColumnFormatters[i](values[i], Columns[i].Format));
            }

            WriteLine(line.ToString());
        }

        protected override void OnWriteFooter()
        {
            // nothing to do here
        }

        #endregion

        protected override FormatterDelegate GetFormatterDelegate(DataFileColumn column)
        {
            if (column.Type == typeof(string))
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
    }
}
