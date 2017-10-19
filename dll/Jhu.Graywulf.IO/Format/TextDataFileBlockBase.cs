using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    [DataContract(Namespace="")]
    public abstract class TextDataFileBlockBase : FormattedDataFileBlockBase, ICloneable
    {
        [IgnoreDataMember]
        private TextDataFileBase File
        {
            get { return (TextDataFileBase)file; }
        }

        #region Constructors and initializers

        protected TextDataFileBlockBase()
        {
            InitializeMembers();
        }

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
        }

        private void CopyMembers(TextDataFileBlockBase old)
        {
        }

        #endregion
        #region Column functions

        /// <summary>
        /// Detects columns from a set of values.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="useNames"></param>
        /// <param name="columns"></param>
        /// <param name="columnTypePrecedence"></param>
        /// <remarks>
        /// If it is the first line and useName is true, it will use them as column names,
        /// otherwise parts are only counted, columns are created for each and automatically generated
        /// names are used.
        /// </remarks>
        protected void DetectColumnsFromParts(string[] parts, bool useNames, out Column[] columns, out int[] columnTypePrecedence)
        {
            columns = new Column[parts.Length];
            columnTypePrecedence = new int[parts.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new Column();

                if (useNames)
                {
                    columns[i].Name = parts[i].Trim();
                }
                else
                {
                    columns[i].Name = String.Format("Col{0}", i);  // *** TODO: use constants
                }
            }
        }

        #endregion
        #region Read functions

        protected internal override void OnReadHeader()
        {
            // Make sure it's the first line
            if (File.BufferedReader.LineCounter > 0)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (File.AutoDetectColumns)
            {
                // Buffering is needed to detect columns automatically
                File.BufferedReader.StartLineBuffer();

                string[] parts;
                Column[] columns = null;      // detected columns
                int[] columnTypePrecedence = null;

                // If column names are in the first line, use them to generate names
                if (File.ColumnNamesInFirstLine)
                {
                    OnReadNextRowParts(out parts, false);
                    DetectColumnsFromParts(parts, true, out columns, out columnTypePrecedence);
                }

                File.BufferedReader.SkipLines(File.SkipLinesCount);

                // Try to figure out the type of columns from the first n rows
                // Try to read some rows to detect
                int q = 0;
                while (q < File.AutoDetectColumnsCount && OnReadNextRowParts(out parts, true))
                {
                    if (q == 0 && columns == null)
                    {
                        DetectColumnsFromParts(parts, false, out columns, out columnTypePrecedence);
                    }

                    if (columns.Length != parts.Length)
                    {
                        throw new FileFormatException();    // TODO
                    }

                    DetectColumnTypes(parts, columns, columnTypePrecedence);

                    q++;
                }

                // Rewind stream
                File.BufferedReader.RewindLineBuffer();
                File.BufferedReader.StopLineBuffer();

                // Now the stream is rewound but we skip the very first line
                // if it contains the column names
                if (File.ColumnNamesInFirstLine)
                {
                    OnReadNextRowParts(out parts, false);
                }

                CreateColumns(columns);
            }
            else
            {
                CreateColumns(Columns.ToArray());
            }

            // Skip the first few lines of the file
            File.BufferedReader.SkipLines(File.SkipLinesCount);
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
