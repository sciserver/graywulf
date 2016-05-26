using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Jhu.Graywulf.IO.Tasks
{
    public partial class AsyncFileCopy : Jhu.Graywulf.Tasks.CancelableTask, IDisposable
    {
        private string source;
        private string destination;
        private int blockSize;
        private bool unbuffered;

        private byte[][] buffer;
        private EventWaitHandle[] events;
        private long bytes;
        private FileStream sourceStream;
        private FileStream destinationStream;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        public bool Unbuffered
        {
            get { return unbuffered; }
            set { unbuffered = value; }
        }

        public AsyncFileCopy()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
            this.blockSize = 0x100000;    // 1M
            this.unbuffered = true;
            this.buffer = null;
            this.events = null;
            this.sourceStream = null;
            this.destinationStream = null;
        }

        public void Dispose()
        {
            buffer = null;

            if (sourceStream != null)
            {
                sourceStream.Dispose();
            }

            if (destinationStream != null)
            {
                destinationStream.Dispose();
            }

            if (events != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    events[i].Dispose();
                }
            }
        }

        protected override void OnExecute()
        {
            sourceStream = null;
            destinationStream = null;

            ExecuteNet4_5();
        }

        private void ExecuteNet4_5()
        {
            try
            {
                sourceStream = OpenStream(source, FileMode.Open, FileAccess.Read, FileShare.None, unbuffered, true, true, blockSize);
                destinationStream = OpenStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, unbuffered, true, true, blockSize);

                var t = sourceStream.CopyToAsync(destinationStream, blockSize);

                t.Wait();

                // TODO: implement cancelation
            }
            finally
            {
                Dispose();
            }
        }

        private void ExecutePreNet4_5()
        {
            try
            {
                // TODO: in .Net 4.5, this is implemented by the framework, use
                // sourceStream.CopyToAsync() instead

                buffer = new byte[2][];
                events = new EventWaitHandle[2];

                for (int i = 0; i < 2; i++)
                {
                    buffer[i] = new byte[blockSize];
                    events[i] = new EventWaitHandle(true, EventResetMode.ManualReset);
                }

                sourceStream = OpenStream(source, FileMode.Open, FileAccess.Read, FileShare.None, unbuffered, true, true, blockSize);
                destinationStream = OpenStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, unbuffered, true, true, blockSize);

                bytes = sourceStream.Length;

                // Overlapped copy of most of the file
                if (bytes >= blockSize)
                {
                    int slot = 0;

                    while (!IsCanceled && bytes >= blockSize)
                    {
                        events[slot].WaitOne();
                        events[slot].Reset();

                        bytes -= sourceStream.Read(buffer[slot], 0, blockSize);
                        destinationStream.BeginWrite(buffer[slot], 0, blockSize, OnWriteComplete, slot);

                        slot = (slot + 1) % 2;
                    }

                    for (int i = 0; i < events.Length; i++)
                    {
                        events[i].WaitOne();
                    }
                }

                // Write last portion of file
                if (bytes > 0)
                {
                    sourceStream.Read(buffer[0], 0, (int)bytes);
                    destinationStream.Write(buffer[0], 0, (int)bytes);
                }

                sourceStream.Close();
                destinationStream.Close();
            }
            finally
            {
                Dispose();
            }
        }

        private void OnWriteComplete(IAsyncResult ar)
        {
            destinationStream.EndWrite(ar);
            int slot = (int)ar.AsyncState;

            events[slot].Set();
        }

        protected FileStream OpenStream(string path, FileMode mode, FileAccess acc, FileShare share, bool unbuffered, bool sequential, bool async, int blockSize)
        {
            FileStream stream = null;
            int flags = 0;

            if (unbuffered) flags |= Native.FILE_FLAG_NO_BUFFERING;
            if (sequential) flags |= Native.FILE_FLAG_SEQUENTIAL_SCAN;
            if (async) flags |= Native.FILE_FLAG_OVERLAPPED;

            SafeFileHandle handle = Native.CreateFile(path, (int)acc, share, IntPtr.Zero, mode, flags, IntPtr.Zero);

            if (!handle.IsInvalid)
            {
                stream = new FileStream(handle, acc, blockSize, async);
            }
            else
            {
                stream = null;
                handle = null;
            }

            return stream;
        }
    }
}
