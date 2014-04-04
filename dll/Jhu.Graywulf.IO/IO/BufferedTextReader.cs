using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.IO
{
    /// <summary>
    /// Implements a wrapper around TextReader that stores a given number of lines in memory
    /// and allows rewinding to the beginning to the buffer even if the underlying file does
    /// not support seeking (i.e. network streams)
    /// </summary>
    public class BufferedTextReader
    {
        /// <summary>
        /// Reference to the text reader wrapping the input stream.
        /// </summary>
        private TextReader baseReader;

        /// <summary>
        /// Keeps track of the current line's number
        /// </summary>
        private long lineCounter;

        /// <summary>
        /// If buffering is turned on, lines are saved into this buffer
        /// to support rewind of files wrapping non-seekable streams.
        /// </summary>
        private List<string> lineBuffer;

        /// <summary>
        /// Marks if line buffering is turned on
        /// </summary>
        private bool lineBufferOn;

        /// <summary>
        /// Gets the number of the current line
        /// </summary>
        public long LineCounter
        {
            get { return lineCounter; }
        }

        /// <summary>
        /// Creates a new object by wrapping a TextReader
        /// </summary>
        /// <param name="baseReader"></param>
        public BufferedTextReader(TextReader baseReader)
        {
            InitializeMembers();

            this.baseReader = baseReader;
        }

        /// <summary>
        /// Initializes member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.baseReader = null;

            this.lineCounter = 0;
            this.lineBuffer = null;
            this.lineBufferOn = false;
        }

        #region Buffer implementation

        /// <summary>
        /// Reads the next line from the input stream.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <returns></returns>
        /// 
        public string ReadLine()
        {
            // TODO: make sure line length is limited, so no invalid
            // file is read into memory entirely

            // See if there are lines in the buffer (prefetched for column detection)
            // If so, use lines in buffer, otherwise read from the stream
            if (lineBuffer != null && lineCounter < lineBuffer.Count)
            {
                return lineBuffer[(int)lineCounter++];
            }
            else
            {
                string line = baseReader.ReadLine();

                // Store in buffer if it's turned on
                if (lineBufferOn)
                {
                    lineBuffer.Add(line);
                }

                lineCounter++;

                return line;
            }
        }

        /// <summary>
        /// Skips a given number of lines.
        /// </summary>
        /// <param name="count"></param>
        public void SkipLines(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReadLine();
            }
        }

        /// <summary>
        /// Turns line buffering on
        /// </summary>
        public void StartLineBuffer()
        {
            // Line buffer can only be turned on at the beginning
            if (lineCounter > 0)
            {
                throw new InvalidOperationException();
            }

            if (lineBuffer != null)
            {
                throw new InvalidOperationException();
            }

            lineBuffer = new List<string>();
            lineBufferOn = true;
        }

        /// <summary>
        /// Turns line buffering off.
        /// </summary>
        public void StopLineBuffer()
        {
            lineBufferOn = false;
        }

        /// <summary>
        /// Rewinds line buffer to the beginning of the file
        /// </summary>
        public void RewindLineBuffer()
        {
            if (lineBuffer == null && lineCounter > 0)
            {
                throw new InvalidOperationException();
            }

            lineCounter = 0;
        }

        #endregion

    }
}
