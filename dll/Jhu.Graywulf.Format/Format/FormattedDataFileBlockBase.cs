using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Types;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    public abstract class FormattedDataFileBlockBase : DataFileBlockBase, ICloneable
    {
        protected delegate bool ParserDelegate(string value, out object result);
        protected delegate string FormatterDelegate(object value, string format);

        [NonSerialized]
        private ParserDelegate[] columnParsers;

        [NonSerialized]
        private FormatterDelegate[] columnFormatters;

        protected ParserDelegate[] ColumnParsers
        {
            get { return columnParsers; }
        }

        protected FormatterDelegate[] ColumnFormatters
        {
            get { return columnFormatters; }
        }

        private FormattedDataFileBase File
        {
            get { return (FormattedDataFileBase)file; }
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
        /// <param name="cols"></param>
        /// <param name="colranks"></param>
        protected void DetectColumnTypes(string[] parts, Column[] cols, int[] colranks)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Type type;
                int size, rank;
                if (!GetBestColumnTypeEstimate(parts[i], out type, out size, out rank))
                {
                    cols[i].DataType.IsNullable = true;
                }

                if (cols[i].DataType == null || colranks[i] < rank)
                {
                    cols[i].DataType = DataType.Create(type, (short)size);
                }

                // Make column longer if necessary
                if (cols[i].DataType.HasLength && cols[i].DataType.Length < size)
                {
                    cols[i].DataType.Length = (short)size;
                }
            }
        }

        #endregion
        #region Read functions

        protected abstract bool ReadNextRowParts(out string[] parts, bool skipComments);

        protected internal override bool OnReadNextRow(object[] values)
        {
            string[] parts;

            if (ReadNextRowParts(out parts, true))
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
            else if (t == typeof(Guid))
            {
                return delegate(object o, string f)
                {
                    return String.Format(File.Culture, f, (Guid)o);
                };
            }

            throw new NotImplementedException();
        }

        private bool GetBestColumnTypeEstimate(string value, out Type type, out int size, out int rank)
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

            Int32 int32v;
            if (Int32.TryParse(value, File.NumberStyle, File.Culture, out int32v))
            {
                type = typeof(Int32);
                size = 0;
                return true;
            }
            rank++;

            Int64 int64v;
            if (Int64.TryParse(value, File.NumberStyle, File.Culture, out int64v))
            {
                type = typeof(Int64);
                size = 0;
                return true;
            }
            rank++;

            double doublev;
            if (double.TryParse(value, File.NumberStyle, File.Culture, out doublev))
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
            if (DateTime.TryParse(value, File.Culture, File.DateTimeStyle, out datetimev))
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

#endregion
    }
}
