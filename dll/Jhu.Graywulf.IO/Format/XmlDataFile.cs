using System;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality to read and write XML files.
    /// </summary>
    [Serializable]
    public abstract class XmlDataFile : FormattedDataFileBase, IDisposable
    {
        #region Private member variables

        /// <summary>
        /// Used to compare tags and attribute names. Must be case-sensitive.
        /// </summary>
        public static readonly StringComparer Comparer = StringComparer.InvariantCulture;

        /// <summary>
        /// XmlReader that wraps the stream and used to retreive xml elements.
        /// </summary>
        [NonSerialized]
        private XmlReader inputReader;

        /// <summary>
        /// If true, the input reader is opened by the class and will need to be closed
        /// and disposed when the XML data file is disposed.
        /// </summary>
        [NonSerialized]
        private bool ownsInputReader;

        /// <summary>
        /// XmlWriter that wraps the output stream and used to write xml elements.
        /// </summary>
        [NonSerialized]
        private XmlWriter outputWriter;

        /// <summary>
        /// If true, the output reader is opened by the class and will need to be closed
        /// on disposed when the XML data file is disposed.
        /// </summary>
        [NonSerialized]
        private bool ownsOutputWriter;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the xml reader opened for this XML data file.
        /// </summary>
        public XmlReader XmlReader
        {
            get { return inputReader; }
        }

        /// <summary>
        /// Gets the xml writer opened for this XML data file.
        /// </summary>
        public XmlWriter XmlWriter
        {
            get { return outputWriter; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Initializes an XML data file without opening any underlying stream.
        /// </summary>
        public XmlDataFile()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public XmlDataFile(XmlDataFile old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes an XML data file by automatically opening an underlying stream
        /// identified by an URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        /// <param name="encoding"></param>
        public XmlDataFile(Uri uri, DataFileMode fileMode, Encoding encoding)
            : base(uri, fileMode, encoding, CultureInfo.InvariantCulture)
        {
            InitializeMembers(new StreamingContext());
        }

        public XmlDataFile(Uri uri, DataFileMode fileMode)
            : this(uri, fileMode, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes an XML data file by automatically wrapping and already open binary stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode"></param>
        /// <param name="compression"></param>
        /// <param name="encoding"></param>
        public XmlDataFile(Stream stream, DataFileMode fileMode, Encoding encoding)
            : base(stream, fileMode, encoding, CultureInfo.InvariantCulture)
        {
            InitializeMembers(new StreamingContext());
        }

        public XmlDataFile(Stream stream, DataFileMode fileMode)
            : this(stream, fileMode, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes an XML reader by re-using an already open xml reader.
        /// </summary>
        /// <param name="inputReader"></param>
        /// <param name="encoding"></param>
        public XmlDataFile(XmlReader inputReader, Encoding encoding)
            : base((Stream)null, DataFileMode.Read, encoding, CultureInfo.InvariantCulture)
        {
            InitializeMembers(new StreamingContext());

            this.inputReader = inputReader;
        }

        public XmlDataFile(XmlReader inputReader)
            : this(inputReader, Encoding.UTF8)
        {
            // Overload
        }

        /// <summary>
        /// Initializes an XML data file by re-using an already open xml writer.
        /// </summary>
        /// <param name="outputWriter"></param>
        /// <param name="encoding"></param>
        public XmlDataFile(XmlWriter outputWriter, Encoding encoding)
            : base((Stream)null, DataFileMode.Write, encoding, CultureInfo.InvariantCulture)
        {
            InitializeMembers(new StreamingContext());

            this.outputWriter = outputWriter;
        }

        public XmlDataFile(XmlWriter outputWriter)
            : this(outputWriter, Encoding.UTF8)
        {
            // Overload
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.inputReader = null;
            this.ownsInputReader = false;

            this.outputWriter = null;
            this.ownsOutputWriter = false;
        }

        private void CopyMembers(XmlDataFile old)
        {
            this.inputReader = null;
            this.ownsInputReader = false;

            this.outputWriter = null;
            this.ownsOutputWriter = false;
        }

        public override void Dispose()
        {
            Close();
            base.Dispose();
        }

        #endregion
        #region Stream open and close

        /// <summary>
        /// Starts reading an XML data file from an already open xml reader.
        /// </summary>
        /// <param name="inputReader"></param>
        public virtual void Open(XmlReader inputReader)
        {
            base.Open((Stream)null, DataFileMode.Read);

            this.inputReader = inputReader;

            Open();
        }

        /// <summary>
        /// Starts writing an XML data file into an already open xml writer.
        /// </summary>
        /// <param name="outputWriter"></param>
        public virtual void Open(XmlWriter outputWriter)
        {
            base.Open((Stream)null, DataFileMode.Write);

            this.outputWriter = outputWriter;

            Open();
        }

        /// <summary>
        /// Makes sure that no stream is opened yet.
        /// </summary>
        protected override void EnsureNotOpen()
        {
            if (ownsInputReader && inputReader != null ||
                ownsOutputWriter && outputWriter != null)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// If necessary, opens an XmlReader and wraps the underlying stream in it.
        /// </summary>
        /// <remarks>
        /// This function is called by the infrastructure when starting to read
        /// the file by the FileDataReader
        /// </remarks>
        protected override async Task OpenForReadAsync()
        {
            if (inputReader == null)
            {
                await base.OpenForReadAsync();

                var settings = new XmlReaderSettings()
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                };

                inputReader = XmlReader.Create(new DetachedStream(BaseStream), settings);
                ownsInputReader = true;
            }
        }

        /// <summary>
        /// If necessary, opens an XmlWriter and wraps the underlying stream in it.
        /// </summary>
        protected override async Task OpenForWriteAsync()
        {
            if (outputWriter == null)
            {
                await base.OpenForWriteAsync();

                var settings = new XmlWriterSettings()
                {
                    Indent = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                };

                if (Encoding != null)
                {
                    settings.Encoding = Encoding;
                }

                outputWriter = XmlWriter.Create(new DetachedStream(BaseStream), settings);
                ownsOutputWriter = true;
            }
        }

        /// <summary>
        /// Closes the streams, if they were opened by the object itself.
        /// </summary>
        public override void Close()
        {
            if (ownsInputReader && inputReader != null)
            {
                inputReader.Close();
                inputReader = null;
                ownsInputReader = false;
            }

            if (ownsOutputWriter && outputWriter != null)
            {
                outputWriter.Flush();
                outputWriter.Close();
                outputWriter = null;
                ownsOutputWriter = false;
            }

            base.Close();
        }

        /// <summary>
        /// Gets the state of the underlying data stream.
        /// </summary>
        public override bool IsClosed
        {
            get
            {
                switch (FileMode)
                {
                    case DataFileMode.Read:
                        return inputReader == null;
                    case DataFileMode.Write:
                        return outputWriter == null;
                    case DataFileMode.Unknown:
                        return true;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion
        #region XML utility functions

        public T Deserialize<T>()
        {
            var s = new XmlSerializer(typeof(T));
            return (T)s.Deserialize(XmlReader);
        }

        #endregion
    }
}
