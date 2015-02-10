using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    [DataContract(Namespace="")]
    public class SqlServerNativeDataFileBlock : DataFileBlockBase, ICloneable
    {
        private delegate void BinaryColumnWriterDelegate(SqlServerNativeBinaryWriter w, object value, DataType type);

        [NonSerialized]
        private BinaryColumnWriterDelegate[] columnWriters;

        [IgnoreDataMember]
        private SqlServerNativeDataFile File
        {
            get { return (SqlServerNativeDataFile)file; }
        }

        protected SqlServerNativeDataFileBlock()
        {
            InitializeMembers();
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
            // Nothing to do here
        }

        protected override void OnWriteNextRow(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                columnWriters[i](File.NativeWriter, values[i], Columns[i].DataType);
            }
        }

        protected override void OnWriteFooter()
        {
            // Native files are always stored in an archive as they consist of
            // multiple files

            if (File.GenerateSqlScripts)
            {
                WriteCreateScript();
                WriteBulkInsertScript();
            }
        }

        private void WriteTextFileEntry(string extension, string text)
        {
            var filename = Util.UriConverter.ToFileName(File.Uri);
            var dir = Path.GetDirectoryName(filename);

            // Strip the extension of the archive
            filename = Path.GetFileNameWithoutExtension(filename);
            // Strip the extension of the file format
            filename = Path.GetFileNameWithoutExtension(filename);
            // Append to directory and add custom extension
            filename = Path.Combine(dir, filename += extension);

            var buffer = Encoding.UTF8.GetBytes(text);

            File.CreateArchiveEntry(filename, buffer.Length);
            File.BaseStream.Write(buffer, 0, buffer.Length);
            
            // Normally, this entry would need to be closed here but
            // it will get closed automatically when the next entry
            // is added so the following line is commented out
            // File.CloseArchiveEntry();
        }

        private void WriteCreateScript()
        {
            var sql = new StringBuilder();

            // TODO: somehow bring table name here, probably via smartdatareader/smartcommand
            sql.AppendLine(String.Format("CREATE TABLE [{0}]", "$tablename"));
            sql.AppendLine("(");

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    sql.AppendLine(",");
                }

                sql.AppendFormat("\t[{0}] {1} {2}NULL",
                    Columns[i].Name,
                    Columns[i].DataType.NameWithLength,
                    Columns[i].DataType.IsNullable ? "" : "NOT ");
            }

            sql.AppendLine();
            sql.AppendLine(")");

            WriteTextFileEntry("_create.sql", sql.ToString());
        }

        private void WriteBulkInsertScript()
        {
            var filename = Util.UriConverter.ToFileName(File.Uri);
            // Strip the extension of the archive
            filename = Path.GetFileNameWithoutExtension(filename);

            var sql = new StringBuilder();

            sql.AppendLine(String.Format("BULK INSERT [{0}]", "$tablename"));
            sql.AppendLine(String.Format("FROM '{0}.bcp'", filename));
            sql.AppendLine("WITH (DATAFILETYPE = 'native', TABLOCK)");

            WriteTextFileEntry("_insert.sql", sql.ToString());
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
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlBit(v);
                            };
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlTinyInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlSmallInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.Int:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlBigInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.Real:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlReal(v);
                            };
                            break;
                        case System.Data.SqlDbType.Float:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlFloat(v);
                            };
                            break;
                        case System.Data.SqlDbType.Binary:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlBinary(v);
                            };
                            break;
                        case System.Data.SqlDbType.VarBinary:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlBinary(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlVarBinaryMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.Image:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlImage(v);
                            };
                            break;
                        case System.Data.SqlDbType.Char:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlChar(v);
                            };
                            break;
                        case System.Data.SqlDbType.VarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlVarChar(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlVarCharMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.Text:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlText(v);
                            };
                            break;
                        case System.Data.SqlDbType.NChar:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlNChar(v);
                            };
                            break;
                        case System.Data.SqlDbType.NVarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlNChar(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlNVarCharMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.NText:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlNText(v);
                            };
                            break;
                        case System.Data.SqlDbType.Date:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlDate(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlDateTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime2:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlDateTime2(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTimeOffset:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlDateTimeOffset(v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallDateTime:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlSmallDateTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.Time:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.Timestamp:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlTimestamp(v);
                            };
                            break;
                        case System.Data.SqlDbType.Decimal:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlDecimal(v, t.Precision, t.Scale);
                            };
                            break;
                        case System.Data.SqlDbType.SmallMoney:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlSmallMoney(v);
                            };
                            break;
                        case System.Data.SqlDbType.Money:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlMoney(v);
                            };
                            break;
                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteSqlUniqueIdentifier(v);
                            };
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
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlBit(v);
                            };
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlTinyInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlSmallInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.Int:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlBigInt(v);
                            };
                            break;
                        case System.Data.SqlDbType.Real:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlReal(v);
                            };
                            break;
                        case System.Data.SqlDbType.Float:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlFloat(v);
                            };
                            break;
                        case System.Data.SqlDbType.Binary:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlBinary(v);
                            };
                            break;
                        case System.Data.SqlDbType.VarBinary:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlBinary(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlVarBinaryMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.Image:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlImage(v);
                            };
                            break;
                        case System.Data.SqlDbType.Char:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlChar(v);
                            };
                            break;
                        case System.Data.SqlDbType.VarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlVarChar(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlVarCharMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.Text:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlText(v);
                            };
                            break;
                        case System.Data.SqlDbType.NChar:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlNChar(v);
                            };
                            break;
                        case System.Data.SqlDbType.NVarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlNChar(v);
                            };
                            }
                            else
                            {
                                columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlNVarCharMax(v);
                            };
                            }
                            break;
                        case System.Data.SqlDbType.NText:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlNText(v);
                            };
                            break;
                        case System.Data.SqlDbType.Date:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlDate(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlDateTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime2:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlDateTime2(v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTimeOffset:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlDateTimeOffset(v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallDateTime:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlSmallDateTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.Time:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlTime(v);
                            };
                            break;
                        case System.Data.SqlDbType.Timestamp:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlTimestamp(v);
                            };
                            break;
                        case System.Data.SqlDbType.Decimal:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlDecimal(v, t.Precision, t.Scale);
                            };
                            break;
                        case System.Data.SqlDbType.SmallMoney:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlSmallMoney(v);
                            };
                            break;
                        case System.Data.SqlDbType.Money:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlMoney(v);
                            };
                            break;
                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnWriters[i] = delegate(SqlServerNativeBinaryWriter w, object v, DataType t)
                            {
                                w.WriteNullableSqlUniqueIdentifier(v);
                            };
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

        #endregion
    }
}
