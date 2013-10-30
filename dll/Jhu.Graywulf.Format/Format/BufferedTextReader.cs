using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements a wrapper around TextReader that stores a given number of lines in memory
    /// and allows rewinding to the beginning to the buffer even if the underlying file does
    /// not support seeking (i.e. network streams)
    /// </summary>
    public class BufferedTextReader
    {
        private TextReader baseReader;

        private long lineCounter;
        private List<string> lineBuffer;
        private bool lineBufferOn;

        public long LineCounter
        {
            get { return lineCounter; }
        }

        public BufferedTextReader(TextReader baseReader)
        {
            InitializeMembers();

            this.baseReader = baseReader;
        }

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

        public void StopLineBuffer()
        {
            lineBufferOn = false;
        }

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
