using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Net;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.IO
{
    /// <summary>
    /// Implements logic to open streams to read and write URIs.
    /// </summary>
    /// <remarks>
    /// Class is inheritable to implement additional network protocols
    /// but compression methods can only be handled internally.
    /// </remarks>
    public class StreamFactory
    {
        #region Static members

        public static IOConfiguration Configuration
        {
            get
            {
                return (IOConfiguration)ConfigurationManager.GetSection("jhu.graywulf/io");
            }
        }

        /// <summary>
        /// Creates a new factory class derived from StreamFactory
        /// but using the actual class listed in the config files.
        /// </summary>
        /// <returns></returns>
        public static StreamFactory Create(string typename)
        {
            Type type = null;

            if (typename != null)
            {
                type = Type.GetType(typename);
            }

            // Use config file
            if (Configuration.StreamFactory != null)
            {
                type = Type.GetType(Configuration.StreamFactory);
            }

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(StreamFactory);
            }

            return (StreamFactory)Activator.CreateInstance(type, true);
        }

        #endregion
        #region Private member variables

        private Uri uri;
        private Credentials credentials;
        private DataFileMode mode;
        private DataFileCompression compression;
        private DataFileArchival archival;
        private int bufferSize;
        private FileOptions options;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the URI to open
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Gets or sets the object holding credentials to authenticate
        /// file access.
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// Gets or sets read/write mode.
        /// </summary>
        public DataFileMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Gets or sets whether the file should be compressed/decompressed
        /// </summary>
        public DataFileCompression Compression
        {
            get { return compression; }
            set { compression = value; }
        }

        /// <summary>
        /// Gets or sets whether the file in an archive (tar, zip, directory)
        /// </summary>
        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        public FileOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion
        #region Constructors and initialzers

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        protected StreamFactory()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializes private member variables.
        /// </summary>
        private void InitializeMembers()
        {
            this.uri = null;
            this.credentials = null;
            this.mode = DataFileMode.Read;
            this.compression = DataFileCompression.Automatic;
            this.archival = DataFileArchival.Automatic;
            this.bufferSize = 0x10000;  // 64k
            this.options = FileOptions.None;
        }

        #endregion
        #region Public methods to open a stream

        /// <summary>
        /// Opens a file identified by a URI for read or write.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <remarks>
        /// If not set explicitly through properties, compression and archival settings
        /// are figured out automatically from the file extension.
        /// </remarks>
        public async Task<Stream> OpenAsync(Uri uri, Credentials credentials, DataFileMode mode)
        {
            this.uri = uri;
            this.credentials = credentials;
            this.mode = mode;

            return await OpenAsync();
        }

        public async Task<Stream> OpenAsync(Uri uri, Credentials credentials, DataFileMode mode, DataFileCompression compression)
        {
            this.uri = uri;
            this.credentials = credentials;
            this.mode = mode;
            this.compression = compression;

            return await OpenAsync();
        }

        /// <summary>
        /// Opens a file identified by a URI for read or write.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mode"></param>
        /// <param name="compression"></param>
        /// <param name="archival"></param>
        /// <returns></returns>
        public async Task<Stream> OpenAsync(Uri uri, Credentials credentials, DataFileMode mode, DataFileCompression compression, DataFileArchival archival)
        {
            this.uri = uri;
            this.credentials = credentials;
            this.mode = mode;
            this.compression = compression;
            this.archival = archival;

            return await OpenAsync();
        }

        public Stream Open(Stream stream, DataFileMode mode, DataFileCompression compression, DataFileArchival archival)
        {
            this.mode = mode;
            this.compression = compression;
            this.archival = archival;

            return Open(stream);
        }

        /// <summary>
        /// Opens a file with parameters determined by the factory class properties.
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> OpenAsync()
        {
            switch (mode)
            {
                case DataFileMode.Read:
                    return await OpenForReadAsync();
                case DataFileMode.Write:
                    return await OpenForWriteAsync();
                default:
                    throw new NotImplementedException();
            }
        }

        public Stream Open(Stream stream)
        {
            switch (mode)
            {
                case DataFileMode.Read:
                    return WrapForRead(stream);
                case DataFileMode.Write:
                    return WrapForWrite(stream);
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        #region Stream handlers

        /// <summary>
        /// Opens a stream for reading a file. Stream is automatically wrapped into
        /// a decompressor and archive reader, if necessary.
        /// </summary>
        /// <returns></returns>
        private async Task<Stream> OpenForReadAsync()
        {
            var stream = await OpenBaseStreamForRead();

            if (stream == null)
            {
                throw new FileFormatException("Unknown protocol.");      // TODO
            }

            return WrapForRead(stream);
        }

        private Stream WrapForRead(Stream stream)
        {
            // Check if compressed and wrap in compressed stream reader
            stream = WrapCompressedStreamForRead(stream);

            // Check if archive and wrap in archive stream reader
            stream = WrapArchiveStreamForRead(stream);

            return stream;
        }

        /// <summary>
        /// Opens a stream for writing a file. Stream is automatically wrapped into
        /// an archive writer and compressior, if necessary.
        /// </summary>
        /// <returns></returns>
        private async Task<Stream> OpenForWriteAsync()
        {
            var stream = await OpenBaseStreamForWrite();

            if (stream == null)
            {
                throw new FileFormatException("Unknown protocol.");      // TODO
            }

            return WrapForWrite(stream);
        }

        private Stream WrapForWrite(Stream stream)
        {
            // Check if compressed and wrap in compressed stream reader
            stream = WrapCompressedStreamForWrite(stream);

            // Check if archive and wrap in archive stream reader
            stream = WrapArchiveStreamForWrite(stream);

            return stream;
        }

        /// <summary>
        /// Opens the base stream for reading a file using the protocol specified by the URI.
        /// </summary>
        /// <remarks>
        /// This function can be overloaded in inherited classes to
        /// support special protocols.
        /// </remarks>
        protected virtual async Task<Stream> OpenBaseStreamForRead()
        {
            // If URI is relative it must be a file
            if (!uri.IsAbsoluteUri || uri.IsFile)
            {
                return OpenFileStream();
            }
            else
            {
                switch (uri.Scheme.ToLowerInvariant())
                {
                    case Constants.UriSchemeHttp:
                    case Constants.UriSchemeHttps:
                        return await OpenHttpStreamAsync();
                    case Constants.UriSchemeFtp:
                        return await OpenFtpStreamAsync();
                    default:
                        // If the stream format is unknown, return null, so that a derived class will be
                        // able to handle it
                        return null;
                }
            }
        }

        /// <summary>
        /// Opens the base stream for writing a file using a protocol specified by the URI.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<Stream> OpenBaseStreamForWrite()
        {
            if (!uri.IsAbsoluteUri || uri.IsFile)
            {
                return OpenFileStream();
            }
            else
            {
                switch (uri.Scheme.ToLowerInvariant())
                {
                    case Constants.UriSchemeHttp:
                    case Constants.UriSchemeHttps:
                        return await OpenHttpStreamAsync();
                    case Constants.UriSchemeFtp:
                        return await OpenFtpStreamAsync();
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Wraps a stream into a decompressor.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        private Stream WrapCompressedStreamForRead(Stream baseStream)
        {
            var cm = GetCompressionMethod();

            switch (cm)
            {
                case DataFileCompression.None:
                    return baseStream;
                case DataFileCompression.GZip:
                    return new ICSharpCode.SharpZipLib.GZip.GZipInputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileCompression.BZip2:
                    return new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileCompression.Zip:
                    return new SharpZipLibWrapper.ZipInputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Wraps a stream into a compressor.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        private Stream WrapCompressedStreamForWrite(Stream baseStream)
        {
            var cm = GetCompressionMethod();

            switch (cm)
            {
                case DataFileCompression.None:
                    return baseStream;
                case DataFileCompression.Automatic:
                case DataFileCompression.GZip:
                    return new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileCompression.BZip2:
                    return new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileCompression.Zip:
                    return new SharpZipLibWrapper.ZipOutputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Wraps a stream into an archive reader.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        private Stream WrapArchiveStreamForRead(Stream baseStream)
        {
            var am = GetArchivalMethod();

            switch (am)
            {
                case DataFileArchival.None:
                    return baseStream;
                case DataFileArchival.Tar:
                    return new SharpZipLibWrapper.TarInputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileArchival.Zip:
                    // because zip might have already handled by the compression
                    // wrapper, we need to make a distinction here
                    if (baseStream is SharpZipLibWrapper.ZipInputStream)
                    {
                        return baseStream;
                    }
                    else
                    {
                        return new SharpZipLibWrapper.ZipInputStream(baseStream)
                        {
                            IsStreamOwner = true
                        };
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Wraps a stream into an archive writer.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        private Stream WrapArchiveStreamForWrite(Stream baseStream)
        {
            var am = GetArchivalMethod();

            switch (am)
            {
                case DataFileArchival.Automatic:
                case DataFileArchival.None:
                    return baseStream;
                case DataFileArchival.Tar:
                    return new SharpZipLibWrapper.TarOutputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
                case DataFileArchival.Zip:
                    // because zip might have already handled by the compression
                    // wrapper, we need to make a distinction here
                    if (baseStream is SharpZipLibWrapper.ZipOutputStream)
                    {
                        return baseStream;
                    }
                    else
                    {
                        return new SharpZipLibWrapper.ZipOutputStream(baseStream)
                        {
                            IsStreamOwner = true
                        };
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        #region Specialized stream open functions

        /// <summary>
        /// Opens a local or UNC file for read or write.
        /// </summary>
        /// <returns></returns>
        protected Stream OpenFileStream()
        {
            var path = Util.UriConverter.ToFilePath(uri);

            switch (mode)
            {
                case DataFileMode.Read:
                    return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, options);
                case DataFileMode.Write:
                    // Create directory first
                    var dir = Path.GetDirectoryName(path);
                    if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, options);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Opens a HTTP stream for read.
        /// </summary>
        /// <returns></returns>
        protected async Task<Stream> OpenHttpStreamAsync()
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);

            req.AllowAutoRedirect = false;
            req.KeepAlive = false;

            if (credentials != null)
            {
                req.UseDefaultCredentials = false;
                req.Credentials = credentials.GetNetworkCredentials();
                req.Headers = credentials.GetWebHeaders();
            }
            else
            {
                req.UseDefaultCredentials = true;
            }

            if (mode == DataFileMode.Read)
            {
                // Read mode is simple, just send request
                // and return response stream 

                req.Method = "GET";

                var res = await req.GetResponseAsync();
                return res.GetResponseStream();
            }
            else if (mode == DataFileMode.Write)
            {
                // In write mode, we get a stream and write data to it.
                // The stream eventually gets closed by the caller, this is
                // when the request is finished and the HTTP respose is
                // read from the server.

                req.Method = "PUT";
                req.AllowWriteStreamBuffering = false;
                req.SendChunked = true;

                // TODO: req.ContentType = 
                
                return await req.GetRequestStreamAsync();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Opens an ftp stream for read.
        /// </summary>
        /// <returns></returns>
        protected async Task<Stream> OpenFtpStreamAsync()
        {
            // TODO: add authentication
            // TODO: add write logic: how to write FTP

            var req = FtpWebRequest.Create(uri);

            if (credentials != null)
            {
                req.Credentials = credentials.GetNetworkCredentials();
            }
            else
            {
                // TODO: what are the default credentials for FTP?
                //req.UseDefaultCredentials = true;
            }

            var res = await req.GetResponseAsync();
            return res.GetResponseStream();
        }

        #endregion
        #region Uri verification

        /// <summary>
        /// Returns true if the URI is safe for general user access.
        /// </summary>
        /// <returns></returns>
        /// <remarks>As most operations run under a service account, user access
        /// to the file system is needs to be controlled manually. This function
        /// returns false for all URIs targeting the file system.</remarks>
        public virtual bool IsUriSchemeSafe(Uri uri)
        {
            if (uri.IsFile || !uri.IsAbsoluteUri)
            {
                return false;
            }

            return true;
        }

        public virtual bool IsUriSchemeSupported(Uri uri)
        {
            var c = StringComparer.InvariantCultureIgnoreCase;

            if (c.Compare(uri.Scheme, Uri.UriSchemeFile) == 0 ||
                c.Compare(uri.Scheme, Uri.UriSchemeHttp) == 0 ||
                c.Compare(uri.Scheme, Uri.UriSchemeHttps) == 0 ||
                c.Compare(uri.Scheme, Uri.UriSchemeFtp) == 0)
            {
                return true;
            }

            return false;
        }

        #endregion
        #region URI manipulation functions

        public static void GetFileExtensions(Uri uri, out string path, out string filename, out string extension, out DataFileArchival archivalMethod, out DataFileCompression compressionMethod)
        {
            GetFileExtensions(Util.UriConverter.GetPath(uri), out path, out filename, out extension, out archivalMethod, out compressionMethod);
        }

        /// <summary>
        /// Returns the file extension by stripping of the extension of the
        /// compressed file, if any.
        /// </summary>
        public static void GetFileExtensions(string uri, out string path, out string filename, out string extension, out DataFileArchival archivalMethod, out DataFileCompression compressionMethod)
        {
            // TODO: We could use mime type here if stream comes from HTTP
            path = uri; ;
            filename = Path.GetFileName(path);
            extension = null;
            archivalMethod = DataFileArchival.None;
            compressionMethod = DataFileCompression.None;

            // Handle at most three extensions, ie. csv.tar.gz
            var extensions = new string[3];
            for (int i = 0; i < extensions.Length; i++)
            {
                extensions[i] = Path.GetExtension(filename);
                filename = Path.GetFileNameWithoutExtension(filename);

                // Archival method is determined from any of the extensions
                // TODO: handle the ambigous case of specifying more than one extension, iw. tar.zip
                if (!String.IsNullOrWhiteSpace(extensions[i]) &&
                    Constants.ArchivalExtensions.ContainsKey(extensions[i]))
                {
                    archivalMethod = Constants.ArchivalExtensions[extensions[i]];
                }
            }
            
            // The first extension determines the compression method
            int j = 0;
            filename = Path.GetFileName(path);
            if (!String.IsNullOrWhiteSpace(extensions[j]) && Constants.CompressionExtensions.ContainsKey(extensions[j]))
            {
                compressionMethod = Constants.CompressionExtensions[extensions[j]];
                filename = Path.GetFileNameWithoutExtension(filename);
                j++;
            }

            // The next extension can be the archival method, i.e. tar.gz
            if (!String.IsNullOrWhiteSpace(extensions[j]) && Constants.ArchivalExtensions.ContainsKey(extensions[j]))
            {
                filename = Path.GetFileNameWithoutExtension(filename);
                j++;
            }

            // The last extension is the actual file format extension
            extension = extensions[j];
            filename = Path.GetFileNameWithoutExtension(filename);
            path = (Path.GetDirectoryName(path) ?? "/").Replace("\\", "/");
        }

        protected DataFileArchival GetArchivalMethod()
        {
            DataFileArchival am;

            if (uri != null && archival == DataFileArchival.Automatic)
            {
                am = GetArchivalMethod(uri);
            }
            else
            {
                am = archival;
            }

            return am;
        }

        /// <summary>
        /// Returnes the archival method used within this file.
        /// </summary>
        /// <returns></returns>
        public virtual DataFileArchival GetArchivalMethod(Uri uri)
        {
            string path, filename, extension;
            DataFileArchival am;
            DataFileCompression cm;

            GetFileExtensions(uri, out path, out filename, out extension, out am, out cm);
            return am;
        }

        private DataFileCompression GetCompressionMethod()
        {
            DataFileCompression cm;

            if (uri != null && compression == DataFileCompression.Automatic)
            {
                cm = GetCompressionMethod(uri);
            }
            else
            {
                cm = compression;
            }

            return cm;
        }

        /// <summary>
        /// If compression is set to automatic, figures out compression
        /// method from the file extension
        /// </summary>
        /// <returns></returns>
        public virtual DataFileCompression GetCompressionMethod(Uri uri)
        {
            string path, filename, extension;
            DataFileArchival am;
            DataFileCompression cm;

            GetFileExtensions(uri, out path, out filename, out extension, out am, out cm);
            return cm;
        }
        
        public static string CombineFileExtensions(string path, string filename, string extension, DataFileArchival archivalMethod, DataFileCompression compressionMethod)
        {
            var uri = Path.Combine(path, filename).Replace("\\", "/");
            uri += extension;

            // Only append tar here, zip is appened at next step
            if (archivalMethod == DataFileArchival.Tar)
            {
                uri += Constants.FileExtensionTar;
            }

            uri += Constants.CompressionExtensions[compressionMethod];

            return uri;
        }

        #endregion
    }
}
