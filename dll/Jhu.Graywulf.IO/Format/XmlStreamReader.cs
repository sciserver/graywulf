using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;

namespace Jhu.Graywulf.Format
{
    public class XmlStream : Stream
    {
        private long position;
        private byte[] buffer;
        private int length;
        private int offset;
        private XmlReader reader;
        private XmlWriter writer;

        public override bool CanRead
        {
            get { return reader != null; }
        }

        public override bool CanWrite
        {
            get { return writer != null; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return position; }
            set { throw new InvalidOperationException(); }
        }

        public XmlStream()
        {
            InitializeMembers();
        }

        public XmlStream(XmlReader reader)
        {
            InitializeMembers();
            Open(reader);
        }

        public XmlStream(XmlWriter writer)
        {
            InitializeMembers();
            Open(writer);
        }

        private void InitializeMembers()
        {
            this.reader = null;
            this.writer = null;
            this.position = 0;        }

        private void EnsureNotOpen()
        {
            if (reader != null || writer != null)
            {
                throw new InvalidOperationException();
            }
        }

        private void InitializeBuffer()
        {
            this.buffer = new byte[0x10000];  // 64k
            this.length = 0;
            this.offset = 0;
        }

        public void Open(XmlReader reader)
        {
            EnsureNotOpen();
            InitializeBuffer();
            this.reader = reader;
        }

        public void Open(XmlWriter writer)
        {
            EnsureNotOpen();
            InitializeBuffer();
            this.writer = writer;
        }

        public override int Read(byte[] dest, int destoffset, int count)
        {
            int cnt = 0;
            int pos = 0;

            while (count > 0)
            {
                if (offset >= length)
                {
                    // No more bytes in the buffer, try to read more
                    length = reader.ReadContentAsBase64(buffer, 0, buffer.Length);
                }

                if (length == 0)
                {
                    // No more bytes to read
                    break;
                }

                if (offset + count < length)
                {
                    // There's enough bytes in the buffer
                    cnt = count;
                }
                else
                {
                    // There's not enough bytes in the buffer
                    cnt = length - offset;
                }

                Buffer.BlockCopy(buffer, offset, dest, destoffset, cnt);
                offset += cnt;
                pos += cnt;
                destoffset += cnt;
                count -= cnt;
            }

            return pos;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void Flush()
        {
            if (writer != null)
            {
                writer.Flush();
            }
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (writer != null)
            {
                await writer.FlushAsync();
            }
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }
    }
}
