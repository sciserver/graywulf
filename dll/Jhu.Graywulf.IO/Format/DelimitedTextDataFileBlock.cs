using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Format
{
    public class DelimitedTextDataFileBlock : TextDataFileBlockBase, ICloneable
    {
        private DelimitedTextDataFile File
        {
            get { return (DelimitedTextDataFile)file; }
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
        protected override bool ReadNextRowParts(out string[] parts, bool skipComments)
        {
            string line;
            List<string> res = new List<string>();

            int ci = 0;
            bool inquote = false;
            string part = String.Empty;

            while (true)
            {
                line = TextBuffer.ReadLine();

                if (line == null)
                {
                    parts = null;
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
                        line = TextBuffer.ReadLine();

                        if (line == null)
                        {
                            throw new Exception();  // TODO: unexpected end of line
                        }

                        ci = 0;
                    }
                    if (!inquote && ci == line.Length || line[ci] == File.Separator)
                    {
                        res.Add(part);
                        part = String.Empty;

                        if (ci == line.Length)  // Outside a quote and end of line reached
                        {
                            parts = res.ToArray();
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

        protected override void OnWriteHeader()
        {            
            var line = new StringBuilder();

            line.Append(File.Comment);

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    line.Append(File.Separator);
                }

                line.Append(Columns[i].Name.Replace(File.Separator, '_'));    // TODO
            }

            File.TextWriter.WriteLine(line.ToString());
        }

        protected override void OnWriteNextRow(object[] values)
        {
            var line = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    line.Append(File.Separator);
                }

                line.Append(ColumnFormatters[i](values[i], Columns[i].Metadata.Format));
            }

            File.TextWriter.WriteLine(line.ToString());
        }

    }
}
