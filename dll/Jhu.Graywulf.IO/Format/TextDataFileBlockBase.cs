using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Types;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    public abstract class TextDataFileBlockBase : FormattedDataFileBlockBase, ICloneable
    {
        [NonSerialized]
        private BufferedTextReader textBuffer;

        protected BufferedTextReader TextBuffer
        {
            get { return textBuffer; }
        }

        private TextDataFileBase File
        {
            get { return (TextDataFileBase)file; }
        }

        #region Constructors and initializers

        public TextDataFileBlockBase(TextDataFileBase file)
            : base(file)
        {
            InitializeMembers();
        }

        public TextDataFileBlockBase(TextDataFileBlockBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.textBuffer = new BufferedTextReader(File.TextReader);
        }

        private void CopyMembers(TextDataFileBlockBase old)
        {
            this.textBuffer = new BufferedTextReader(File.TextReader);
        }

        #endregion
        #region Column functions

        /// <summary>
        /// Detects columns from a set of values.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="useNames"></param>
        /// <param name="cols"></param>
        /// <param name="colRanks"></param>
        /// <remarks>
        /// If it is the first line and useName is true, it will use them as column names,
        /// otherwise parts are only counted, columns are created for each and automatically generated
        /// names are used.
        /// </remarks>
        protected void DetectColumnsFromParts(string[] parts, bool useNames, out Column[] cols, out int[] colRanks)
        {
            cols = new Column[parts.Length];
            colRanks = new int[parts.Length];

            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = new Column();

                if (useNames)
                {
                    cols[i].Name = parts[i].Trim();
                }
                else
                {
                    cols[i].Name = String.Format("Col{0}", i);  // *** TODO: use constants
                }
            }
        }

        #endregion
        #region Read functions

        protected internal override void OnReadHeader()
        {
            // Make sure it's the first line
            if (TextBuffer.LineCounter > 0)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (File.AutoDetectColumns)
            {
                // Buffering is needed to detect columns automatically
                TextBuffer.StartLineBuffer();

                string[] parts;

                Column[] cols = null;      // detected columns
                int[] colranks = null;

                // If column names are in the first line, use them to generate names
                if (File.ColumnNamesInFirstLine)
                {
                    ReadNextRowParts(out parts, false);
                    DetectColumnsFromParts(parts, true, out cols, out colranks);
                }

                TextBuffer.SkipLines(File.SkipLinesCount);

                // Try to figure out the type of columns from the first n rows
                // Try to read some rows to detect
                int q = 0;
                while (q < File.AutoDetectColumnsCount && ReadNextRowParts(out parts, true))
                {
                    if (q == 0 && cols == null)
                    {
                        DetectColumnsFromParts(parts, false, out cols, out colranks);
                    }

                    if (cols.Length != parts.Length)
                    {
                        throw new FileFormatException();    // TODO
                    }

                    DetectColumnTypes(parts, cols, colranks);

                    q++;
                }

                // Rewind stream
                TextBuffer.RewindLineBuffer();
                TextBuffer.StopLineBuffer();

                CreateColumns(cols);
            }
            else
            {
                CreateColumns(Columns.ToArray());
            }

            // Skip the first few lines of the file
            TextBuffer.SkipLines(File.SkipLinesCount);
        }

        protected internal override void OnReadFooter()
        {
            // No footer in text files
        }

        protected internal override void OnReadToFinish()
        {
            // No need to read to the end if no more blocks
        }

        #endregion
        #region Write functions

        protected override void OnWriteFooter()
        {
            // No footer in text files
        }

        #endregion
    }
}
