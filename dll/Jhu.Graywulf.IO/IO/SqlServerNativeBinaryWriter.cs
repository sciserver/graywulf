using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{

    /// <remarks>
    /// bcp native format follows the ODBC standard representation of numbers
    /// http://stackoverflow.com/questions/2099919/what-are-the-binary-storage-formats-for-sqflt8-sqlmoney-and-other-native-sql-da
    /// http://technet.microsoft.com/en-us/library/bb677301.aspx
    /// </remarks>
    public class SqlServerNativeBinaryWriter : IDisposable
    {
        private readonly DateTime epoch0001 = new DateTime(1, 1, 1);
        private readonly DateTime epoch1900 = new DateTime(1900, 1, 1);

        private BinaryWriter writer;
        private Encoding encoding;
        private BitConverterBase bitConverter;
        private byte[] buffer;

        #region Constructors and initializers

        public SqlServerNativeBinaryWriter(Stream baseStream)
        {
            InitializeMembers();

            this.writer = new BinaryWriter(new DetachedStream(baseStream));
        }

        private void InitializeMembers()
        {
            this.writer = null;
            this.encoding = Encoding.Default;
            this.bitConverter = new StraightBitConverter();
            this.buffer = new byte[0x10000];
        }

        public void Dispose()
        {
            this.writer.Dispose();
            this.writer = null;
        }

        #endregion

        public void WriteSqlBit(object value)
        {
            // TODO: verify if works with multiple bits unioned together

            writer.Write((byte)1);
            writer.Write((bool)value ? (byte)1 : (byte)0);
        }

        public void WriteNullableSqlBit(object value)
        {
            // TODO: verify if works with multiple bits unioned together

            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)1);
                writer.Write((bool)value ? (byte)1 : (byte)0);
            }
        }

        public void WriteSqlTinyInt(object value)
        {
            writer.Write((byte)value);
        }

        public void WriteNullableSqlTinyInt(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)1);
                writer.Write((byte)value);
            }
        }

        public void WriteSqlSmallInt(object value)
        {
            writer.Write((short)value);
        }

        public void WriteNullableSqlSmallInt(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)2);
                writer.Write((short)value);
            }
        }

        public void WriteSqlInt(object value)
        {
            writer.Write((int)value);
        }

        public void WriteNullableSqlInt(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                writer.Write((int)value);
            }
        }

        public void WriteSqlBigInt(object value)
        {
            writer.Write((long)value);
        }

        public void WriteNullableSqlBigInt(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                writer.Write((long)value);
            }
        }

        public void WriteSqlReal(object value)
        {
            writer.Write((float)value);
        }

        public void WriteNullableSqlReal(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                writer.Write((float)value);
            }
        }

        public void WriteSqlFloat(object value)
        {
            writer.Write((double)value);
        }

        public void WriteNullableSqlFloat(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                writer.Write((double)value);
            }
        }

        public void WriteSqlDecimal(object value, byte precision, byte scale)
        {
            // SQL Server native format stores decimal numbers on 19 bytes.
            // The first two bytes are precision and scale. Third is the sign,
            // 1 for positive, 0 for negative, followed by 16 bytes for the number.
            // 19 bytes are always enough to represent the highest precision decimals.
            // We assume that the scale of the .Net decimal is equal to the precision of the SQL decimal.
            // To get the sign and scale:
            // bool sign = (parts[3] & 0x80000000) != 0;
            // byte scale = (byte) ((parts[3] >> 16) & 0x7F);
            // http://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx


            var v = (decimal)value;
            int[] bits = decimal.GetBits(v);

            writer.Write((byte)19);
            writer.Write(precision);
            writer.Write(scale);
            writer.Write((byte)(v < 0 ? 0 : 1));
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);
        }

        public void WriteNullableSqlDecimal(object value, byte precision, byte scale)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                WriteSqlDecimal(value, precision, scale);
            }
        }

        public void WriteSqlMoney(object value)
        {
            // SQL Server native format stores money values on eight bytes
            // .Net framework represents decimals on 24 bytes, so we simply ignore
            // the top 8 bytes and also assume that the scale of the .Net decimal
            // is equal to the precision of the SQL money (4)

            var v = (decimal)value;
            int[] bits = decimal.GetBits(v);
            writer.Write(bits[1] * Math.Sign(v));    // TODO: verify sign calculation
            writer.Write(bits[0]);
        }

        public void WriteNullableSqlMoney(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlMoney(value);
            }
        }

        public void WriteSqlSmallMoney(object value)
        {
            // SQL Server native format stores smallmoney values on four bytes
            // .Net framework represents decimals on 24 bytes, so we simply ignore
            // the top 16 bytes and also assume that the scale of the .Net decimal
            // is equal to the precision of the SQL smallmoney (4)

            var v = (decimal)value;
            int[] bits = decimal.GetBits(v);
            writer.Write(bits[0] * Math.Sign(v));    // TODO: verify sign calculation
        }

        public void WriteNullableSqlSmallMoney(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                WriteSqlSmallMoney(value);
            }
        }

        public void WriteSqlChar(object value)
        {
            // Here we assume that value already contains an
            // exact number of characters, so no padding is required

            var str = (string)value;
            var res = encoding.GetBytes(str, 0, str.Length, buffer, 0);
            writer.Write(buffer, 0, res);
        }

        public void WriteNullableSqlChar(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int16)(-1));
            }
            else
            {
                writer.Write((Int16)((string)value).Length);
                WriteSqlChar(value);
            }
        }

        public void WriteSqlVarChar(object value)
        {
            var str = (string)value;
            var res = encoding.GetBytes(str, 0, str.Length, buffer, 0);
            writer.Write((short)res);
            writer.Write(buffer, 0, res);
        }

        public void WriteNullableSqlVarChar(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((short)0x00FF);
            }
            else
            {
                WriteSqlVarChar(value);
            }
        }

        public void WriteSqlText(object value)
        {
            var str = (string)value;

            if (str.Length <= buffer.Length)
            {
                var res = encoding.GetBytes(str, 0, str.Length, buffer, 0);
                writer.Write((Int32)res);
                writer.Write(buffer, 0, res);
            }
            else
            {
                var lbuffer = encoding.GetBytes(str);
                writer.Write((Int32)lbuffer.Length);
                writer.Write(lbuffer);
            }
        }

        public void WriteNullableSqlText(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int32)(-1));
            }
            else
            {
                WriteSqlText(value);
            }
        }

        public void WriteSqlVarCharMax(object value)
        {
            var str = (string)value;

            if (str.Length <= buffer.Length)
            {
                var res = encoding.GetBytes(str, 0, str.Length, buffer, 0);
                writer.Write((Int64)res);
                writer.Write(buffer, 0, res);
            }
            else
            {
                var lbuffer = encoding.GetBytes(str);
                writer.Write((Int64)lbuffer.Length);
                writer.Write(lbuffer);
            }
        }

        public void WriteNullableSqlVarCharMax(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int64)(-1));
            }
            else
            {
                WriteSqlVarCharMax(value);
            }
        }

        public void WriteSqlNChar(object value)
        {
            var str = (string)value;
            var res = Encoding.Unicode.GetBytes(str, 0, str.Length, buffer, 0);
            writer.Write((Int16)res);
            writer.Write(buffer, 0, res);
        }

        public void WriteNullableSqlNChar(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int16)(-1));
            }
            else
            {
                WriteSqlNChar(value);
            }
        }

        public void WriteSqlNText(object value)
        {
            var str = (string)value;
            var res = Encoding.Unicode.GetBytes(str, 0, str.Length, buffer, 0);
            writer.Write((Int32)res);
            writer.Write(buffer, 0, res);
        }

        public void WriteNullableSqlNText(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int32)(-1));
            }
            else
            {
                WriteSqlNText(value);
            }
        }

        public void WriteSqlNVarCharMax(object value)
        {
            var str = (string)value;

            if (str.Length < buffer.Length / 2)
            {
                var res = Encoding.Unicode.GetBytes(str, 0, str.Length, buffer, 0);
                writer.Write((Int64)res);
                writer.Write(buffer, 0, res);
            }
            else
            {
                var lbuffer = Encoding.Unicode.GetBytes(str);
                writer.Write((Int64)lbuffer.Length);
                writer.Write(lbuffer);
            }
        }

        public void WriteNullableSqlNVarCharMax(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int64)(-1));
            }
            else
            {
                WriteSqlNVarCharMax(value);
            }
        }

        public void WriteSqlBinary(object value)
        {
            var bytes = (byte[])value;
            writer.Write((Int16)bytes.Length);
            writer.Write(bytes);
        }

        public void WriteNullableSqlBinary(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int16)(-1));
            }
            else
            {
                WriteSqlBinary(value);
            }
        }

        public void WriteSqlImage(object value)
        {
            var bytes = (byte[])value;
            writer.Write((Int32)bytes.Length);
            writer.Write(bytes);
        }

        public void WriteNullableSqlImage(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int32)(-1));
            }
            else
            {
                WriteSqlImage(value);
            }
        }

        public void WriteSqlVarBinaryMax(object value)
        {
            var bytes = (byte[])value;
            writer.Write((Int64)bytes.Length);
            writer.Write(bytes);
        }

        public void WriteNullableSqlVarBinaryMax(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((Int64)(-1));
            }
            else
            {
                WriteSqlVarBinaryMax(value);
            }
        }

        // ---

        public void WriteSqlDate(object value)
        {
            // SQL Server stores the 'date' type on three bytes only, the
            // number of days since 0001-01-01, and in little-endian byte order

            var dt = (DateTime)value;
            var days = (int)(dt - epoch0001).TotalDays;
            bitConverter.GetBytes(days, buffer, 0);

            writer.Write(buffer, 0, 3);
        }

        public void WriteNullableSqlDate(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)3);
                WriteSqlDate(value);
            }
        }

        public void WriteSqlSmallDateTime(object value)
        {
            // SQL Server stores the 'smalldatetime' type on four bytes,
            // the number of minutes since 1900-01-01 00:00:00.000

            var dt = (DateTime)value;
            var days = (short)(dt.Date - epoch1900).TotalDays;
            var mins = (short)(dt.TimeOfDay).TotalMinutes;

            writer.Write(days);
            writer.Write(mins);
        }

        public void WriteNullableSqlSmallDateTime(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                WriteSqlSmallDateTime(value);
            }
        }

        public void WriteSqlDateTime(object value)
        {
            // SQL Server stores the 'datetime' type on eight bytes.
            // The first four bytes are in the little-endian byte order
            // Time is stored as the number of seconds * 300 + a part of millisenconds
            // rounded to .000, .003 and .007

            var dt = (DateTime)value;
            var days = (int)(dt.Date - epoch1900).TotalDays;
            var secs = (int)((dt.TimeOfDay.Ticks * 3 + 50000) / 100000);     // TODO: figure out millisecond part

            writer.Write(days);
            writer.Write(secs);
        }

        public void WriteNullableSqlDateTime(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlDateTime(value);
            }
        }

        public void WriteSqlDateTime2(object value)
        {
            // SQL Server stores datetime2 values on eight bytes in the native format.
            // The first five bytes represent the time part, the last three are the date
            // part. Date is expressed as the number of days since 0001-01-01.
            // Time is expressed as the number of seconds * 10e7, independently of
            // the actual precision of the column.

            var dt = (DateTime)value;
            var days = (int)(dt - epoch0001).TotalDays;
            var secs = (long)(dt.TimeOfDay.TotalSeconds * 1e7);

            // time part
            bitConverter.GetBytes(secs, buffer, 0);
            writer.Write(buffer, 0, 5);

            // date part
            bitConverter.GetBytes(days, buffer, 0);
            writer.Write(buffer, 0, 3);
        }

        public void WriteNullableSqlDateTime2(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlDateTime2(value);
            }
        }

        public void WriteSqlDateTimeOffset(object value)
        {
            // SQL Server stores datetimeoffset (datetime2 + time zone) values on ten bytes
            // in the native format.
            // The first five bytes represent the time part, the next three are the date
            // part and the last two are the time zone in signed minutes.
            // Date and time is converted to UT and stored such a way.
            // Date is expressed as the number of days since 0001-01-01.
            // Time is expressed as the number of seconds * 10e7, independently of
            // the actual precision of the column.

            var dt = (DateTimeOffset)value;
            var utc = dt.UtcDateTime;
            var days = (int)(utc - epoch0001).TotalDays;
            var secs = (long)(utc.TimeOfDay.TotalSeconds * 1e7);
            var zone = (short)dt.Offset.TotalMinutes;

            // time part
            bitConverter.GetBytes(secs, buffer, 0);
            writer.Write(buffer, 0, 5);

            // date part
            bitConverter.GetBytes(days, buffer, 0);
            writer.Write(buffer, 0, 3);

            // offset part
            writer.Write(zone);
        }

        public void WriteNullableSqlDateTimeOffset(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)10);
                WriteSqlDateTimeOffset(value);
            }
        }

        public void WriteSqlTime(object value)
        {
            // SQL Server native format stores time values on five bytes
            // as the number of seconds * 10e7
            // Precision is always assumed to be 7

            var t = (TimeSpan)value;
            var secs = (long)(t.TotalSeconds * 1e7);

            bitConverter.GetBytes(secs, buffer, 0);
            writer.Write(buffer, 0, 5);
        }

        public void WriteNullableSqlTime(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)5);
                WriteSqlTime(value);
            }
        }

        // ---

        public void WriteSqlTimestamp(object value)
        {
            // SQL Server timestamps are 8-byte binaries

            writer.Write((short)8);
            writer.Write((byte[])value);
        }

        public void WriteNullableSqlTimestamp(object value)
        {
            // SQL Server timestamps are 8-byte binaries

            if (value == DBNull.Value)
            {
                writer.Write((short)0x00FF);
            }
            else
            {
                WriteSqlTimestamp(value);
            }
        }

        public void WriteSqlUniqueIdentifier(object value)
        {
            writer.Write((byte)16);
            writer.Write(((Guid)value).ToByteArray(), 0, 16);
        }

        public void WriteNullableSqlUniqueIdentifier(object value)
        {
            if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                WriteSqlUniqueIdentifier(value);
            }
        }
    }
}
