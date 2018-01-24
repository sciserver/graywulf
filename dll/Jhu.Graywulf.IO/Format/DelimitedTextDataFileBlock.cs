using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    [DataContract(Namespace="")]
    public class DelimitedTextDataFileBlock : TextDataFileBlockBase, ICloneable
    {
        [IgnoreDataMember]
        private DelimitedTextDataFile File
        {
            get { return (DelimitedTextDataFile)file; }
        }

        #region Constructors and initializers

        protected DelimitedTextDataFileBlock()
        {
            InitializeMembers();
        }

        public DelimitedTextDataFileBlock(DelimitedTextDataFile file)
            : base(file)
        {
            InitializeMembers();
        }

        public DelimitedTextDataFileBlock(DelimitedTextDataFileBlock old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(DelimitedTextDataFileBlock old)
        {
        }

        public override object Clone()
        {
            return new DelimitedTextDataFileBlock(this);
        }

        #endregion
        #region Column functions

        /// <summary>
        /// Returns a delegate for formatting the column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>
        /// Characters and strings have to be quoted as may contain the separator character
        /// </remarks>
        protected override FormattedDataFileBlockBase.FormatterDelegate GetFormatterDelegate(Column column)
        {
            var t = column.DataType.Type;

            if (t == typeof(Char))
            {
                return delegate(object o, string f)
                {
                    return File.Quote + ((string)o).Replace(File.Quote.ToString(), File.Quote.ToString() + File.Quote) + File.Quote;
                };
            }
            else if (t == typeof(String))
            {
                return delegate(object o, string f)
                {
                    // TODO: maybe do some optimization here
                    return File.Quote + ((string)o).Replace(File.Quote.ToString(), File.Quote.ToString() + File.Quote) + File.Quote;
                };
            }
            else
            {
                return base.GetFormatterDelegate(column);
            }
        }

        #endregion

        private string EscapeColumnName(string name)
        {
            for (int i = 0; i < File.ColumnSeparators.Length; i ++)
            {
                name.Replace(File.ColumnSeparators[i], '_');
            }

            return name;
        }

        /// <summary>
        /// Returns the next line in the file broken up into parts
        /// along separators.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function supports quoted strings.
        /// </remarks>
        protected override async Task<bool> OnReadNextRowPartsAsync(List<string> parts, bool skipComments)
        {
            string line;
            int ci = 0;
            bool inquote = false;
            string part = String.Empty;

            parts.Clear();

            while (true)
            {
                line = await File.BufferedReader.ReadLineAsync();

                if (line == null)
                {
                    return false;
                }

                if (!skipComments)
                {
                    line = line.TrimStart(File.Comment);
                }

                // skip empty lines
                if (!inquote && line.Length == 0)
                {
                    continue;
                }

                // skip comments
                if (!inquote && skipComments && line[0] == File.Comment)
                {
                    continue;
                }

                // Loop over characters in line
                while (ci <= line.Length)
                {
                    if (inquote && ci == line.Length)   // Inside a quote and end of line reached
                    {
                        line = await File.BufferedReader.ReadLineAsync();

                        if (line == null)
                        {
                            throw new Exception();  // TODO: unexpected end of line
                        }

                        ci = 0;
                    }
                    if (!inquote && ci == line.Length || File.ColumnSeparators.Contains(line[ci]))
                    {
                        parts.Add(part);
                        part = String.Empty;

                        if (ci == line.Length)  // Outside a quote and end of line reached
                        {
                            return true;
                        }
                    }
                    else if (line[ci] == File.Quote)     // quote
                    {
                        if (!inquote)
                        {
                            inquote = true;
                        }
                        else
                        {
                            if (ci < line.Length - 1 && line[ci + 1] == File.Quote)
                            {
                                // double quote
                                part += File.Quote;
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

        protected override async Task OnWriteHeaderAsync()
        {         
            var line = new StringBuilder();

            line.Append(File.Comment);

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    // Always use the first column separator
                    line.Append(File.ColumnSeparators[0]);
                }

                line.Append(EscapeColumnName(Columns[i].Name));
            }

            await File.TextWriter.WriteLineAsync(line.ToString());
        }

        protected override async Task OnWriteNextRowAsync(object[] values)
        {
            var line = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    // Allways use the first column separator
                    line.Append(File.ColumnSeparators[0]);
                }

                if (values[i] == DBNull.Value)
                {
                    switch (File.NullStyle)
                    {
                        case NullStyles.Empty:
                            break;
                        case NullStyles.NullText:
                            line.Append("NULL");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    line.Append(ColumnFormatters[i](values[i], Columns[i].Metadata.Format));
                }
            }

            await File.TextWriter.WriteLineAsync(line.ToString());
        }

    }
}
