using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    [DataContract(Namespace="")]
    public abstract class FormattedDataFileBlockBase : DataFileBlockBase, ICloneable
    {
        protected delegate bool ParserDelegate(string value, out object result);
        protected delegate string FormatterDelegate(object value, string format);

        [NonSerialized]
        private ParserDelegate[] columnParsers;

        [NonSerialized]
        private FormatterDelegate[] columnFormatters;

        [IgnoreDataMember]
        protected ParserDelegate[] ColumnParsers
        {
            get { return columnParsers; }
        }

        [IgnoreDataMember]
        protected FormatterDelegate[] ColumnFormatters
        {
            get { return columnFormatters; }
        }

        [IgnoreDataMember]
        private FormattedDataFileBase File
        {
            get { return (FormattedDataFileBase)file; }
        }

        protected FormattedDataFileBlockBase()
        {
            InitializeMembers();
        }

        public FormattedDataFileBlockBase(FormattedDataFileBase file)
            : base(file)
        {
            InitializeMembers();
        }

        public FormattedDataFileBlockBase(FormattedDataFileBlockBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(FormattedDataFileBlockBase old)
        {
        }

        #region Column functions

        protected override void OnColumnsCreated()
        {
            if ((file.FileMode & DataFileMode.Read) != 0)
            {
                InitializeColumnParsers();
            }

            if ((file.FileMode & DataFileMode.Write) != 0)
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

        /// <summary>
        /// Detect column types from a set of values
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="columns"></param>
        /// <param name="columnTypePredence"></param>
        protected void DetectColumnTypes(IList<string> parts, Column[] columns, int[] columnTypePredence)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                Type type;
                int size, precedence;
                if (!GetBestColumnTypeEstimate(parts[i], out type, out size, out precedence))
                {
                    columns[i].DataType.IsNullable = true;
                }

                if (columns[i].DataType == null || columnTypePredence[i] < precedence)
                {
                    columns[i].DataType = DataType.Create(type, (short)size);
                    columnTypePredence[i] = precedence;
                }

                // Make column longer if necessary
                if (columns[i].DataType.HasLength && columns[i].DataType.Length < size)
                {
                    columns[i].DataType.Length = (short)size;
                }
            }
        }

        #endregion
        #region Read functions

        protected abstract Task<bool> OnReadNextRowPartsAsync(List<string> parts, bool skipComments);

        protected internal override async Task<bool> OnReadNextRowAsync(object[] values)
        {
            var parts = new List<string>(Columns.Count);
            var hasNextRow = await OnReadNextRowPartsAsync(parts, true);

            if (hasNextRow)
            {
                // Now parse the parts
                int pi = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (!Columns[i].IsIdentity)
                    {
                        if (parts[pi] == null && Columns[i].DataType.IsNullable)
                        {
                            values[i] = null;
                        }
                        else if (parts[pi] == null)
                        {
                            throw new ArgumentNullException();  // *** TODO
                        }
                        else if (!ColumnParsers[i](parts[pi], out values[i]))
                        {
                            throw new FormatException();    // TODO: add logic to skip exceptions
                        }

                        pi++;
                    }
                    else
                    {
                        values[i] = null;   // Identity value will be filled in by the data reader
                    }
                }

                // TODO: add logic to handle nulls

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        #region Parsers, formatter and column recognition

        private ParserDelegate GetParserDelegate(Column column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(string s, out object p)
                {
                    SByte v;
                    var r = SByte.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(string s, out object p)
                {
                    Int16 v;
                    var r = Int16.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(string s, out object p)
                {
                    Int32 v;
                    var r = Int32.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(string s, out object p)
                {
                    Int64 v;
                    var r = Int64.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(string s, out object p)
                {
                    Byte v;
                    var r = Byte.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(string s, out object p)
                {
                    UInt16 v;
                    var r = UInt16.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(string s, out object p)
                {
                    UInt32 v;
                    var r = UInt32.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(string s, out object p)
                {
                    UInt64 v;
                    var r = UInt64.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Boolean))
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
            else if (t == typeof(Single))
            {
                return delegate(string s, out object p)
                {
                    float v;
                    var r = float.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Double))
            {
                return delegate(string s, out object p)
                {
                    double v;
                    var r = double.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Decimal))
            {
                return delegate(string s, out object p)
                {
                    decimal v;
                    var r = decimal.TryParse(s, File.NumberStyle, File.Culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Char))
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
            else if (t == typeof(String))
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
                    var r = DateTime.TryParse(s, File.Culture, File.DateTimeStyle, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(DateTimeOffset))
            {
                return delegate(string s, out object p)
                {
                    DateTimeOffset v;
                    var r = DateTimeOffset.TryParse(s, File.Culture, File.DateTimeStyle, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(TimeSpan))
            {
                return delegate(string s, out object p)
                {
                    TimeSpan v;
                    var r = TimeSpan.TryParse(s, File.Culture, out v);
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
            else if (t == typeof(Byte[]))
            {
                // Assume hex representation
                return delegate(string s, out object p)
                {
                    int start;
                    s = s.Trim();

                    if (s.StartsWith("0x"))
                    {
                        if (s.Length < 4 || s.Length % 2 != 0)
                        {
                            p = null;
                            return false;
                        }

                        start = 2;
                    }
                    else
                    {
                        if (s.Length < 2 || s.Length % 2 != 0)
                        {
                            p = null;
                            return false;
                        }

                        start = 0;
                    }

                    var count = (s.Length - start) / 2;
                    var buffer = new Byte[count];

                    // TODO: This could be further optimized by parsing ulong values instead of bytes...
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        // TODO: This one here can also be optimized, for example, not to call substring
                        buffer[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
                    }

                    p = buffer;
                    return true;
                };
            }

            throw new NotImplementedException();
        }

        protected virtual FormatterDelegate GetFormatterDelegate(Column column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (SByte)o);
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Int16)o);
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Int32)o);
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Int64)o);
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Byte)o);
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (UInt16)o);
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (UInt32)o);
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (UInt64)o);
                };
            }
            else if (t == typeof(Boolean))
            {
                // TODO: use numbers?
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (bool)o);
                };
            }
            else if (t == typeof(Single))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (float)o);
                };
            }
            else if (t == typeof(Double))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (double)o);
                };
            }
            else if (t == typeof(Decimal))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (decimal)o);
                };
            }
            else if (t == typeof(Char))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (string)o);
                };
            }
            else if (t == typeof(String))
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
                    return String.Format(File.Culture, f, (DateTime)o);
                };
            }
            else if (t == typeof(DateTimeOffset))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (DateTimeOffset)o);
                };
            }
            else if (t == typeof(TimeSpan))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (TimeSpan)o);
                };
            }
            else if (t == typeof(Guid))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Guid)o);
                };
            }
            else if (t == typeof(byte[]))
            {
                return delegate(object o, string f)
                {
                    var buffer = (byte[])o;

                    if (buffer.Length == 0)
                    {
                        return null;    // *** TODO: test if returning null's OK, otherwise return ""
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        sb.Append("0x");

                        for (int i = 0; i < buffer.Length; i++)
                        {
                            sb.AppendFormat("{0:X}", buffer[i]);
                        }

                        return sb.ToString();
                    }
                };
            }

            throw new NotImplementedException();
        }

        private bool GetBestColumnTypeEstimate(string value, out Type type, out int size, out int precedence)
        {
            if (String.IsNullOrEmpty(value))
            {
                type = null;
                size = 0;
                precedence = 0;
                return false;
            }

            // Try from the simplest to the most complex syntax

            // Only assume Int32 or Int64 for integers. ID's might start from low numbers and the first 1000
            // rows would suggest an ID is only Int16 while it is Int32.

            // double and float are indistinguishable based on their string representations
            // while the number of decimal digits could be used, standard .net functions don't provide
            // this functionality, so we always assume double by default

            // We calculate a precedence value (a kind of type complexity value). Types with
            // higher precedence can store types with lower precedence. Any type can be stored
            // in strings.

            precedence = 0;

            Int32 int32v;
            if (Int32.TryParse(value, File.NumberStyle, File.Culture, out int32v))
            {
                type = typeof(Int32);
                size = 0;
                return true;
            }
            precedence++;

            Int64 int64v;
            if (Int64.TryParse(value, File.NumberStyle, File.Culture, out int64v))
            {
                type = typeof(Int64);
                size = 0;
                return true;
            }
            precedence++;

            double doublev;
            if (double.TryParse(value, File.NumberStyle, File.Culture, out doublev))
            {
                type = typeof(double);
                size = 0;
                return true;
            }
            precedence++;

            Guid guidv;
            if (Guid.TryParse(value, out guidv))
            {
                type = typeof(Guid);
                size = 0;
                return true;
            }
            precedence++;

            DateTime datetimev;
            if (DateTime.TryParse(value, File.Culture, File.DateTimeStyle, out datetimev))
            {
                type = typeof(DateTime);
                size = 0;
                return true;
            }
            precedence++;

            if (value.Length == 1)
            {
                type = typeof(char);
                size = 0;
                return true;
            }
            precedence++;

            bool boolv;
            if (bool.TryParse(value, out boolv))
            {
                type = typeof(bool);
                size = 0;
                return true;
            }
            precedence++;

            // TODO: check if it's a hex literal that can be parsed into byte[]

            // Last case: it's a string
            type = typeof(string);
            size = value.Length;
            return true;
        }

#endregion
    }
}
