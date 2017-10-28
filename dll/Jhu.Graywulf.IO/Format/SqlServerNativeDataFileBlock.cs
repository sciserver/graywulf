using System;
using System.Threading.Tasks;
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
        private delegate void BinaryColumnReaderDelegate(SqlServerNativeBinaryReader r, out object value, DataType type);

        [NonSerialized]
        private BinaryColumnReaderDelegate[] columnReaders;

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
                    CreateColumnReaders();
                    break;
                case DataFileMode.Write:
                    CreateColumnWriters();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected internal override Task OnReadHeaderAsync()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        protected internal override async Task<bool> OnReadNextRowAsync(object[] values)
        {
            try
            {

                for (int i = 0; i < values.Length; i++)
                {
                    columnReaders[i](File.NativeReader, out values[i], Columns[i].DataType);
                }

                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        protected internal override Task OnReadToFinishAsync()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        protected internal override Task OnReadFooterAsync()
        {
            // Nothing to do here
            return Task.CompletedTask;
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
            var filename = Util.UriConverter.ToFilePath(File.Uri);
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
                    Columns[i].DataType.TypeNameWithLength,
                    Columns[i].DataType.IsNullable ? "" : "NOT ");
            }

            sql.AppendLine();
            sql.AppendLine(")");

            WriteTextFileEntry("_create.sql", sql.ToString());
        }

        private void WriteBulkInsertScript()
        {
            var filename = Util.UriConverter.ToFilePath(File.Uri);
            // Strip the extension of the archive
            filename = Path.GetFileNameWithoutExtension(filename);

            var sql = new StringBuilder();

            sql.AppendLine(String.Format("BULK INSERT [{0}]", "$tablename"));
            sql.AppendLine(String.Format("FROM '{0}.bcp'", filename));
            sql.AppendLine("WITH (DATAFILETYPE = 'native', TABLOCK)");

            WriteTextFileEntry("_insert.sql", sql.ToString());
        }

        #region Value formatter functions

        /// <summary>
        /// Creates column writer delegates for fast dispatching
        /// </summary>
        private void CreateColumnReaders()
        {
            columnReaders = new BinaryColumnReaderDelegate[Columns.Count];

            for (int i = 0; i < columnReaders.Length; i++)
            {
                if (!Columns[i].DataType.IsNullable)
                {
                    switch (Columns[i].DataType.SqlDbType.Value)
                    {
                        case System.Data.SqlDbType.Bit:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlBit(out v);
                            };
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlTinyInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlSmallInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Int:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlBigInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Real:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlReal(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Float:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlFloat(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Binary:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlBinary(t.Length, out v);
                            };
                            break;
                        case System.Data.SqlDbType.VarBinary:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlVarBinary(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlVarBinaryMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.Image:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlImage(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Char:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlChar(t.Length, out v);
                            };
                            break;
                        case System.Data.SqlDbType.VarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlVarChar(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlVarCharMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.Text:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlText(out v);
                            };
                            break;
                        case System.Data.SqlDbType.NChar:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlNChar(t.Length, out v);
                            };
                            break;
                        case System.Data.SqlDbType.NVarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlNVarChar(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadSqlNVarCharMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.NText:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlNText(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Date:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlDate(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlDateTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime2:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlDateTime2(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTimeOffset:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlDateTimeOffset(out v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallDateTime:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlSmallDateTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Time:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Timestamp:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlTimestamp(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Decimal:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlDecimal(out v, t.Precision, t.Scale);
                            };
                            break;
                        case System.Data.SqlDbType.SmallMoney:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlSmallMoney(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Money:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlMoney(out v);
                            };
                            break;
                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadSqlUniqueIdentifier(out v);
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
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlBit(out v);
                            };
                            break;
                        case System.Data.SqlDbType.TinyInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlTinyInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlSmallInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Int:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.BigInt:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlBigInt(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Real:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlReal(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Float:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlFloat(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Binary:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlBinary(out v);
                            };
                            break;
                        case System.Data.SqlDbType.VarBinary:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlBinary(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlVarBinaryMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.Image:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlImage(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Char:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlChar(out v);
                            };
                            break;
                        case System.Data.SqlDbType.VarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlVarChar(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlVarCharMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.Text:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlText(out v);
                            };
                            break;
                        case System.Data.SqlDbType.NChar:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlNChar(out v);
                            };
                            break;
                        case System.Data.SqlDbType.NVarChar:
                            if (!Columns[i].DataType.IsMaxLength)
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlNChar(out v);
                                };
                            }
                            else
                            {
                                columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                                {
                                    r.ReadNullableSqlNVarCharMax(out v);
                                };
                            }
                            break;
                        case System.Data.SqlDbType.NText:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlNText(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Date:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlDate(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlDateTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTime2:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlDateTime2(out v);
                            };
                            break;
                        case System.Data.SqlDbType.DateTimeOffset:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlDateTimeOffset(out v);
                            };
                            break;
                        case System.Data.SqlDbType.SmallDateTime:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlSmallDateTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Time:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlTime(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Timestamp:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlTimestamp(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Decimal:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlDecimal(out v, t.Precision, t.Scale);
                            };
                            break;
                        case System.Data.SqlDbType.SmallMoney:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlSmallMoney(out v);
                            };
                            break;
                        case System.Data.SqlDbType.Money:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlMoney(out v);
                            };
                            break;
                        case System.Data.SqlDbType.UniqueIdentifier:
                            columnReaders[i] = delegate(SqlServerNativeBinaryReader r, out object v, DataType t)
                            {
                                r.ReadNullableSqlUniqueIdentifier(out v);
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
