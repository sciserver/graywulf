using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Jhu.Graywulf.Types;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    public class SqlServerNativeDataFileBlock : DataFileBlockBase, ICloneable
    {
        private delegate void BinaryColumnWriterDelegate(BinaryWriter w, object value, DataType type);

        private BinaryColumnWriterDelegate[] columnWriters;

        private SqlServerNativeDataFile File
        {
            get { return (SqlServerNativeDataFile)file; }
        }

        public SqlServerNativeDataFileBlock(SqlServerNativeDataFile file)
            : base(file)
        {
            InitializeMembers();
        }

        public SqlServerNativeDataFileBlock(SqlServerNativeDataFileBlock old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(SqlServerNativeDataFileBlock old)
        {
        }

        public override object Clone()
        {
            return new SqlServerNativeDataFileBlock(this);
        }

        protected override void OnColumnsCreated()
        {
            switch (File.FileMode)
            {
                case DataFileMode.Read:
                    throw new NotImplementedException();
                case DataFileMode.Write:
                    CreateColumnWriters();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected internal override void OnReadHeader()
        {
        }

        protected internal override bool OnReadNextRow(object[] values)
        {
            return false;
        }

        protected internal override void OnReadToFinish()
        {

        }

        protected internal override void OnReadFooter()
        {

        }

        protected override void OnWriteHeader()
        {
            // Native files are always stored in an archive as they consist of
            // multiple files

            WriteCreateScript();
            WriteBulkInsertScript();

            // Create entry for data
            File.CreateArchiveEntry("table.dat", 0);
        }

        protected override void OnWriteNextRow(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                columnWriters[i](File.OutputWriter, values[i], Columns[i].DataType);
            }
        }

        protected override void OnWriteFooter()
        {
            // Nothing to do here
        }

        private void WriteTextFileEntry(string filename, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);

            File.CreateArchiveEntry(filename, buffer.Length);
            File.BaseStream.Write(buffer, 0, buffer.Length);
        }

        private void WriteCreateScript()
        {
            var sql = new StringBuilder();

            sql.AppendLine(String.Format("CREATE TABLE [{0}] (", "newtable"));

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    sql.AppendLine(",");
                }

                sql.AppendFormat("[{0}] {1} {2}NULL",
                    Columns[i].Name,
                    Columns[i].DataType.NameWithLength,
                    Columns[i].DataType.IsNullable ? "" : "NOT ");
            }

            sql.AppendLine(")");

            WriteTextFileEntry("create.sql", sql.ToString());
        }

        private void WriteBulkInsertScript()
        {
            var sql = new StringBuilder();

            sql.AppendLine(String.Format("BULK INSERT [{0}]", "newtable"));
            sql.AppendLine(String.Format("FROM '{0}'", "table.dat"));
            sql.AppendLine("WITH (DATAFILETYPE = 'native', TABLOCK)");

            WriteTextFileEntry("insert.sql", sql.ToString());
        }

        #region Value writer functions

        /// <summary>
        /// Creates column writer delegates for fast dispatching
        /// </summary>
        private void CreateColumnWriters()
        {
            columnWriters = new BinaryColumnWriterDelegate[Columns.Count];

            for (int i = 0; i < columnWriters.Length; i++)
            {
                if (!Columns[i].DataType.IsNullable)
                {
                    switch (Columns[i].DataType.SqlDbType.Value)
                    {
                        case System.Data.SqlDbType.Bit:
                            columnWriters[i] = WriteBit;
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnWriters[i] = WriteTinyInt;
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnWriters[i] = WriteSmallInt;
                            break;
                        case System.Data.SqlDbType.Int:
                            columnWriters[i] = WriteInt;
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnWriters[i] = WriteBigInt;
                            break;
                        case System.Data.SqlDbType.Real:
                            columnWriters[i] = WriteReal;
                            break;
                        case System.Data.SqlDbType.Float:
                            columnWriters[i] = WriteFloat;
                            break;

                        case System.Data.SqlDbType.Image:
                        case System.Data.SqlDbType.Binary:
                        case System.Data.SqlDbType.VarBinary:
                        case System.Data.SqlDbType.Text:
                        case System.Data.SqlDbType.Char:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NVarChar:

                        case System.Data.SqlDbType.Date:
                        case System.Data.SqlDbType.DateTime:
                        case System.Data.SqlDbType.DateTime2:
                        case System.Data.SqlDbType.DateTimeOffset:
                        case System.Data.SqlDbType.SmallDateTime:
                        case System.Data.SqlDbType.Time:

                        case System.Data.SqlDbType.Timestamp:

                        case System.Data.SqlDbType.Decimal:

                        case System.Data.SqlDbType.SmallMoney:
                            throw new NotImplementedException();
                        case System.Data.SqlDbType.Money:
                            columnWriters[i] = WriteMoney;
                            break;

                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnWriters[i] = WriteUniqueIdentifier;
                            break;

                        case System.Data.SqlDbType.Variant:
                        case System.Data.SqlDbType.Xml:

                        case System.Data.SqlDbType.Structured:
                        case System.Data.SqlDbType.Udt:
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    switch (Columns[i].DataType.SqlDbType.Value)
                    {
                        case System.Data.SqlDbType.Bit:
                            columnWriters[i] = WriteNullableBit;
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnWriters[i] = WriteNullableTinyInt;
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnWriters[i] = WriteNullableSmallInt;
                            break;
                        case System.Data.SqlDbType.Int:
                            columnWriters[i] = WriteNullableInt;
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnWriters[i] = WriteNullableBigInt;
                            break;
                        case System.Data.SqlDbType.Real:
                            columnWriters[i] = WriteNullableReal;
                            break;
                        case System.Data.SqlDbType.Float:
                            columnWriters[i] = WriteNullableFloat;
                            break;

                        case System.Data.SqlDbType.Image:
                        case System.Data.SqlDbType.Binary:
                        case System.Data.SqlDbType.VarBinary:
                        case System.Data.SqlDbType.Text:
                        case System.Data.SqlDbType.Char:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NVarChar:

                        case System.Data.SqlDbType.Date:
                        case System.Data.SqlDbType.DateTime:
                        case System.Data.SqlDbType.DateTime2:
                        case System.Data.SqlDbType.DateTimeOffset:
                        case System.Data.SqlDbType.SmallDateTime:
                        case System.Data.SqlDbType.Time:

                        case System.Data.SqlDbType.Timestamp:

                        case System.Data.SqlDbType.Decimal:

                        case System.Data.SqlDbType.SmallMoney:
                            throw new NotImplementedException();

                        case System.Data.SqlDbType.Money:
                            columnWriters[i] = WriteNullableMoney;
                            break;

                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnWriters[i] = WriteNullableUniqueIdentifier;
                            break;

                        case System.Data.SqlDbType.Variant:
                        case System.Data.SqlDbType.Xml:

                        case System.Data.SqlDbType.Structured:
                        case System.Data.SqlDbType.Udt:
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private void WriteBit(BinaryWriter w, object value, DataType type)
        {
            // TODO: verify if works with multiple bits unioned together

            w.Write((byte)1);
            w.Write((bool)value ? (byte)1 : (byte)0);
        }

        private void WriteNullableBit(BinaryWriter w, object value, DataType type)
        {
            // TODO: verify if works with multiple bits unioned together

            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)1);
                w.Write((bool)value ? (byte)1 : (byte)0);
            }
        }

        private void WriteTinyInt(BinaryWriter w, object value, DataType type)
        {
            w.Write((byte)value);
        }

        private void WriteNullableTinyInt(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)1);
                w.Write((byte)value);
            }
        }

        private void WriteSmallInt(BinaryWriter w, object value, DataType type)
        {
            w.Write((short)value);
        }

        private void WriteNullableSmallInt(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)2);
                w.Write((short)value);
            }
        }

        private void WriteInt(BinaryWriter w, object value, DataType type)
        {
            w.Write((int)value);
        }

        private void WriteNullableInt(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)4);
                w.Write((int)value);
            }
        }

        private void WriteBigInt(BinaryWriter w, object value, DataType type)
        {
            w.Write((long)value);
        }

        private void WriteNullableBigInt(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)8);
                w.Write((long)value);
            }
        }

        private void WriteReal(BinaryWriter w, object value, DataType type)
        {
            w.Write((float)value);
        }

        private void WriteNullableReal(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)4);
                w.Write((float)value);
            }
        }

        private void WriteFloat(BinaryWriter w, object value, DataType type)
        {
            w.Write((double)value);
        }

        private void WriteNullableFloat(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)4);
                w.Write((float)value);
            }
        }

        private void WriteMoney(BinaryWriter w, object value, DataType type)
        {
            // TODO: how to store money in native binary???
            w.Write((double)(decimal)value);
        }

        private void WriteNullableMoney(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                // TODO: how to store money in native binary???
                w.Write((byte)1);
                w.Write((double)(decimal)value);
            }
        }

        private void WriteUniqueIdentifier(BinaryWriter w, object value, DataType type)
        {
            w.Write((byte)16);       // TODO: Test valid value
            w.Write(((Guid)value).ToByteArray(), 0, 16);
        }

        private void WriteNullableUniqueIdentifier(BinaryWriter w, object value, DataType type)
        {
            if (value == DBNull.Value)
            {
                w.Write((byte)0xFF);
            }
            else
            {
                w.Write((byte)16);       // TODO: Test valid value
                w.Write(((Guid)value).ToByteArray(), 0, 16);
            }
        }

#if false
        public void WriteDateTime(DateTime dateTime)
        {
            if (binary)
            {
                int date = (dateTime - new DateTime(1900, 1, 1)).Days;
                int time = (int)((dateTime.TimeOfDay).TotalSeconds * 300);
                outputBinary.Write(date);
                outputBinary.Write(time);
            }
            else
            {
                WriteFieldEnd();
                row.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}", dateTime);
            }
        }

        /*
        public void WriteNullableDateTime(DateTime dateTime)
        {
            if (binary)
            {
                outputBinary.Write((byte)8);
            }
            WriteDateTime(dateTime);
        }*/

        public void WriteVarChar(string value, int maxlength)
        {
            if (binary)
            {
                if (value != null)
                {
                    int len = Math.Min(value.Length, maxlength);
                    byte[] bytes = Encoding.Unicode.GetBytes(value.Substring(0, len));

                    outputBinary.Write((ushort)bytes.Length);
                    outputBinary.Write(bytes);
                }
                else
                {
                    outputBinary.Write((ushort)0xFFFF);
                }
            }
            else
            {
                if (value != null)
                {
                    WriteFieldEnd();
                    row.Append(value.Substring(0, Math.Min(value.Length, maxlength)));
                }
                else
                {
                    WriteFieldEnd();
                }
            }
        }

        public void WriteChar(string value, int length)
        {
            if (binary)
            {
                if (value != null)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(value.PadRight(length));
                    outputBinary.Write(bytes, 0, 2 * length);
                }
                else
                {
                    outputBinary.Write((ushort)0xFFFF);
                }
            }
            else
            {
                if (value != null)
                {
                    WriteFieldEnd();
                    row.Append(value.Substring(0, Math.Min(value.Length, length)));
                }
                else
                {
                    WriteFieldEnd();
                }
            }
        }

        public void WriteNullableChar(string value, int length)
        {
            if (binary)
            {
                if (value == null)
                {
                    outputBinary.Write((ushort)0xFFFF);
                }
                else
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(value.PadRight(length));
                    outputBinary.Write((ushort)bytes.Length);
                    outputBinary.Write(bytes);
                }
            }
            else
            {
                if (value != null)
                {
                    WriteFieldEnd();
                    row.Append(value.Substring(0, Math.Min(value.Length, length)));
                }
                else
                {
                    WriteFieldEnd();
                }
            }
        }

#endif

        #endregion
    }
}
