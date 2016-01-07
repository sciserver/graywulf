using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{

    /// <summary>
    /// Implements functions to write SQL Server native binary files for bulk import data.
    /// </summary>
    /// <remarks>
    /// The BCP native format follows the ODBC standard representation of numbers.
    /// http://stackoverflow.com/questions/2099919/what-are-the-binary-storage-formats-for-sqflt8-sqlmoney-and-other-native-sql-da
    /// http://technet.microsoft.com/en-us/library/bb677301.aspx
    /// </remarks>
    public class SqlServerNativeBinaryReader : IDisposable
    {
        #region Private constants

        private readonly DateTime epoch0001 = new DateTime(1, 1, 1);
        private readonly DateTime epoch1900 = new DateTime(1900, 1, 1);

        #endregion
        #region Private member variables

        private BinaryReader reader;
        private Encoding encoding;
        private byte[] buffer;

        #endregion
        #region Constructors and initializers

        public SqlServerNativeBinaryReader(Stream baseStream)
        {
            InitializeMembers();

            this.reader = new BinaryReader(new DetachedStream(baseStream));
        }

        private void InitializeMembers()
        {
            this.reader = null;
            this.encoding = Encoding.Default;
            this.buffer = new byte[0x10000];
        }

        public void Dispose()
        {
            this.reader.Dispose();
            this.reader = null;
        }

        #endregion

        public void ReadSqlBit(out object value)
        {
            // TODO: verify if works with multiple bits unioned together

            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadByte() != 0;
            }
        }

        public void ReadNullableSqlBit(out object value)
        {
            // TODO: verify if works with multiple bits unioned together

            ReadSqlBit(out value);
        }

        public void ReadSqlTinyInt(out object value)
        {
            value = reader.ReadByte();
        }

        public void ReadNullableSqlTinyInt(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadByte();
            }
        }

        public void ReadSqlSmallInt(out object value)
        {
            value = reader.ReadInt16();
        }

        public void ReadNullableSqlSmallInt(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadInt16();
            }
        }

        public void ReadSqlInt(out object value)
        {
            value = reader.ReadInt32();
        }

        public void ReadNullableSqlInt(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadInt32();
            }
        }

        public void ReadSqlBigInt(out object value)
        {
            value = reader.ReadInt64();
        }

        public void ReadNullableSqlBigInt(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadInt64();
            }
        }

        public void ReadSqlReal(out object value)
        {
            value = reader.ReadSingle();
        }

        public void ReadNullableSqlReal(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadSingle();
            }
        }

        public void ReadSqlFloat(out object value)
        {
            value = reader.ReadDouble();
        }

        public void ReadNullableSqlFloat(out object value)
        {
            var size = reader.ReadByte();

            if (size == 0xFF)
            {
                value = DBNull.Value;
            }
            else
            {
                value = reader.ReadDouble();
            }
        }

        public void ReadSqlDecimal(out object value, byte precision, byte scale)
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

            /*
            var v = (decimal)value;
            int[] bits = decimal.GetBits(v);

            writer.Write((byte)19);
            writer.Write(precision);
            writer.Write(scale);
            writer.Write((byte)(v < 0 ? 0 : 1));
            writer.Write(bits[0]);
            writer.Write(bits[1]);
            writer.Write(bits[2]);
            writer.Write(bits[3]);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlDecimal(out object value, byte precision, byte scale)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                WriteSqlDecimal(value, precision, scale);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlMoney(out object value)
        {
            // SQL Server native format stores money values on eight bytes
            // .Net framework represents decimals on 24 bytes, so we simply ignore
            // the top 8 bytes and also assume that the scale of the .Net decimal
            // is equal to the precision of the SQL money (4)

            /*var v = (decimal)value;
            int[] bits = decimal.GetBits(v);
            writer.Write(bits[1] * Math.Sign(v));    // TODO: verify sign calculation
            writer.Write(bits[0]);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlMoney(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlMoney(value);
            }
             * */

            throw new NotImplementedException();
        }

        public void ReadSqlSmallMoney(out object value)
        {
            // SQL Server native format stores smallmoney values on four bytes
            // .Net framework represents decimals on 24 bytes, so we simply ignore
            // the top 16 bytes and also assume that the scale of the .Net decimal
            // is equal to the precision of the SQL smallmoney (4)

            /*var v = (decimal)value;
            int[] bits = decimal.GetBits(v);
            writer.Write(bits[0] * Math.Sign(v));    // TODO: verify sign calculation
             * */

            throw new NotImplementedException();
        }

        public void ReadNullableSqlSmallMoney(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                WriteSqlSmallMoney(value);
            }
             * */

            throw new NotImplementedException();
        }

        public void ReadSqlChar(int size, out object value)
        {
            if (size == 0)
            {
                value = String.Empty;
            }
            else
            {
                reader.Read(buffer, 0, size);
                value = encoding.GetString(buffer, 0, size);
            }
        }

        public void ReadNullableSqlChar(out object value)
        {
            ReadSqlVarChar(out value);
        }

        public void ReadSqlVarChar(out object value)
        {
            var size = reader.ReadInt16();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                ReadSqlChar(size, out value);
            }
        }

        public void ReadNullableSqlVarChar(out object value)
        {
            ReadSqlVarChar(out value);
        }

        public void ReadSqlText(out object value)
        {
            byte[] lbuffer;

            var size = reader.ReadInt32();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                if (size <= buffer.Length)
                {
                    lbuffer = buffer;
                }
                else
                {
                    lbuffer = new byte[size];
                }

                reader.Read(lbuffer, 0, size);
                value = encoding.GetString(lbuffer, 0, size);
            }
        }

        public void ReadNullableSqlText(out object value)
        {
            ReadSqlText(out value);
        }

        public void ReadSqlVarCharMax(out object value)
        {
            byte[] lbuffer;

            var size = reader.ReadInt64();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                if (size <= buffer.Length)
                {
                    lbuffer = buffer;
                }
                else
                {
                    lbuffer = new byte[size];
                }

                reader.Read(lbuffer, 0, (int)size);
                value = encoding.GetString(lbuffer, 0, (int)size);
            }
        }

        public void ReadNullableSqlVarCharMax(out object value)
        {
            ReadSqlVarCharMax(out value);
        }

        public void ReadSqlNChar(int size, out object value)
        {
            if (size == 0)
            {
                value = String.Empty;
            }
            else
            {
                reader.Read(buffer, 0, size);
                value = Encoding.Unicode.GetString(buffer, 0, size);
            }
        }

        public void ReadNullableSqlNChar(out object value)
        {
            ReadSqlNVarChar(out value);
        }

        public void ReadSqlNVarChar(out object value)
        {
            var size = reader.ReadInt16();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                ReadSqlNChar(size, out value);
            }
        }

        public void ReadNullableSqlNVarChar(out object value)
        {
            ReadSqlNVarChar(out value);
        }

        public void ReadSqlNText(out object value)
        {
            byte[] lbuffer;

            var size = reader.ReadInt32();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                if (size <= buffer.Length)
                {
                    lbuffer = buffer;
                }
                else
                {
                    lbuffer = new byte[size];
                }

                reader.Read(lbuffer, 0, (int)size);
                value = Encoding.Unicode.GetString(lbuffer, 0, (int)size);
            }
        }

        public void ReadNullableSqlNText(out object value)
        {
            ReadSqlNText(out value);
        }

        public void ReadSqlNVarCharMax(out object value)
        {
            byte[] lbuffer;

            var size = reader.ReadInt64();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                if (size <= buffer.Length)
                {
                    lbuffer = buffer;
                }
                else
                {
                    lbuffer = new byte[size];
                }

                reader.Read(lbuffer, 0, (int)size);
                value = Encoding.Unicode.GetString(lbuffer, 0, (int)size);
            }
        }

        public void ReadNullableSqlNVarCharMax(out object value)
        {
            ReadSqlVarCharMax(out value);
        }

        public void ReadSqlBinary(int size, out object value)
        {
            var bytes = new byte[size];
            reader.Read(bytes, 0, size);
            value = bytes;
        }

        public void ReadNullableSqlBinary(out object value)
        {
            ReadSqlVarBinary(out value);
        }

        public void ReadSqlVarBinary(out object value)
        {
            var size = reader.ReadInt16();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                ReadSqlBinary(size, out value);
            }
        }

        public void ReadSqlImage(out object value)
        {
            var size = reader.ReadInt32();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                var bytes = new byte[size];
                reader.Read(bytes, 0, size);
                value = bytes;
            }
        }

        public void ReadNullableSqlImage(out object value)
        {
            ReadSqlImage(out value);
        }

        public void ReadSqlVarBinaryMax(out object value)
        {
            var size = reader.ReadInt64();

            if (size == -1)
            {
                value = DBNull.Value;
            }
            else
            {
                var bytes = new byte[size];
                reader.Read(bytes, 0, (int)size);       // *** TODO
                value = bytes;
            }
        }

        public void ReadNullableSqlVarBinaryMax(out object value)
        {
            ReadSqlVarBinaryMax(out value);
        }

        // ---

        public void ReadSqlDate(out object value)
        {
            // SQL Server stores the 'date' type on three bytes only, the
            // number of days since 0001-01-01, and in little-endian byte order

            /*var dt = (DateTime)value;
            var days = (int)(dt - epoch0001).TotalDays;
            
            writer.Write(BitConverter.GetBytes(days), 0, 3);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlDate(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)3);
                WriteSqlDate(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlSmallDateTime(out object value)
        {
            // SQL Server stores the 'smalldatetime' type on four bytes,
            // the number of minutes since 1900-01-01 00:00:00.000

            /*var dt = (DateTime)value;
            var days = (short)(dt.Date - epoch1900).TotalDays;
            var mins = (short)(dt.TimeOfDay).TotalMinutes;

            writer.Write(days);
            writer.Write(mins);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlSmallDateTime(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)4);
                WriteSqlSmallDateTime(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlDateTime(out object value)
        {
            // SQL Server stores the 'datetime' type on eight bytes.
            // The first four bytes are in the little-endian byte order
            // Time is stored as the number of seconds * 300 + a part of millisenconds
            // rounded to .000, .003 and .007

            /*var dt = (DateTime)value;
            var days = (int)(dt.Date - epoch1900).TotalDays;
            var secs = (int)((dt.TimeOfDay.Ticks * 3 + 50000) / 100000);     // TODO: figure out millisecond part

            writer.Write(days);
            writer.Write(secs);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlDateTime(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlDateTime(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlDateTime2(out object value)
        {
            // SQL Server stores datetime2 values on eight bytes in the native format.
            // The first five bytes represent the time part, the last three are the date
            // part. Date is expressed as the number of days since 0001-01-01.
            // Time is expressed as the number of seconds * 10e7, independently of
            // the actual precision of the column.

            /*var dt = (DateTime)value;
            var days = (int)(dt - epoch0001).TotalDays;
            var secs = (long)(dt.TimeOfDay.TotalSeconds * 1e7);

            // time part
            writer.Write(BitConverter.GetBytes(secs), 0, 5);

            // date part
            writer.Write(BitConverter.GetBytes(days), 0, 3);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlDateTime2(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)8);
                WriteSqlDateTime2(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlDateTimeOffset(out object value)
        {
            // SQL Server stores datetimeoffset (datetime2 + time zone) values on ten bytes
            // in the native format.
            // The first five bytes represent the time part, the next three are the date
            // part and the last two are the time zone in signed minutes.
            // Date and time is converted to UT and stored such a way.
            // Date is expressed as the number of days since 0001-01-01.
            // Time is expressed as the number of seconds * 10e7, independently of
            // the actual precision of the column.

            /*var dt = (DateTimeOffset)value;
            var utc = dt.UtcDateTime;
            var days = (int)(utc - epoch0001).TotalDays;
            var secs = (long)(utc.TimeOfDay.TotalSeconds * 1e7);
            var zone = (short)dt.Offset.TotalMinutes;

            // time part
            writer.Write(BitConverter.GetBytes(secs), 0, 5);

            // date part
            writer.Write(BitConverter.GetBytes(days), 0, 3);

            // offset part
            writer.Write(zone);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlDateTimeOffset(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)10);
                WriteSqlDateTimeOffset(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlTime(out object value)
        {
            // SQL Server native format stores time values on five bytes
            // as the number of seconds * 10e7
            // Precision is always assumed to be 7

            /*var t = (TimeSpan)value;
            var secs = (long)(t.TotalSeconds * 1e7);

            writer.Write(BitConverter.GetBytes(secs), 0, 5);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlTime(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                writer.Write((byte)5);
                WriteSqlTime(value);
            }
             * */

            throw new NotImplementedException();
        }

        // ---

        public void ReadSqlTimestamp(out object value)
        {
            // SQL Server timestamps are 8-byte binaries

            /*writer.Write((short)8);
            writer.Write((byte[])value);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlTimestamp(out object value)
        {
            // SQL Server timestamps are 8-byte binaries

            /*if (value == DBNull.Value)
            {
                writer.Write((short)0x00FF);
            }
            else
            {
                WriteSqlTimestamp(value);
            }*/

            throw new NotImplementedException();
        }

        public void ReadSqlUniqueIdentifier(out object value)
        {
            /*writer.Write((byte)16);
            writer.Write(((Guid)value).ToByteArray(), 0, 16);*/

            throw new NotImplementedException();
        }

        public void ReadNullableSqlUniqueIdentifier(out object value)
        {
            /*if (value == DBNull.Value)
            {
                writer.Write((byte)0xFF);
            }
            else
            {
                WriteSqlUniqueIdentifier(value);
            }*/

            throw new NotImplementedException();
        }
    }
}
