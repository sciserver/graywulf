using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Format
{
    public abstract class FormattedDataFileBlock : DataFileBlockBase
    {
        [NonSerialized]
        private FormattedDataFile.ParserDelegate[] columnParsers;

        [NonSerialized]
        private FormattedDataFile.FormatterDelegate[] columnFormatters;

        public FormattedDataFile.ParserDelegate[] ColumnParsers
        {
            get { return columnParsers; }
        }

        public FormattedDataFile.FormatterDelegate[] ColumnFormatters
        {
            get { return columnFormatters; }
        }

        private FormattedDataFile File
        {
            get { return (FormattedDataFile)file; }
        }

        public FormattedDataFileBlock(FormattedDataFile file)
            : base(file)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
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
            columnParsers = new FormattedDataFile.ParserDelegate[Columns.Count];

            for (int i = 0; i < columnParsers.Length; i++)
            {
                columnParsers[i] = File.GetParserDelegate(Columns[i]);
            }
        }

        private void InitializeColumnFormatters()
        {
            columnFormatters = new FormattedDataFile.FormatterDelegate[Columns.Count];

            for (int i = 0; i < columnFormatters.Length; i++)
            {
                columnFormatters[i] = File.GetFormatterDelegate(Columns[i]);
            }
        }

        /// <summary>
        /// Detect column types from a set of values
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="cols"></param>
        /// <param name="colranks"></param>
        protected void DetectColumnTypes(string[] parts, DataFileColumn[] cols, int[] colranks)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Type type;
                int size, rank;
                if (!File.GetBestColumnTypeEstimate(parts[i], out type, out size, out rank))
                {
                    cols[i].IsNullable = true;
                }

                if (cols[i].DataType == null || colranks[i] < rank)
                {
                    cols[i].DataType = DataType.Create(type, (short)size);
                }

                // Make column longer if necessary
                if (cols[i].DataType.HasSize && cols[i].DataType.Size < size)
                {
                    cols[i].DataType.Size = (short)size;
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
                        if (parts[pi] == null && Columns[i].IsNullable)
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
    }
}
