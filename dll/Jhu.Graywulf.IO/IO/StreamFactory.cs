using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(StreamFactory);
            }

            return (StreamFactory)Activator.CreateInstance(type, true);
        }

        #endregion

        private Uri uri;
        private Credentials credentials;
        private DataFileMode mode;
        private DataFileCompression compression;
        private DataFileArchival archival;
        private int bufferSize;
        private FileOptions options;

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
        public Stream Open(Uri uri, Credentials credentials, DataFileMode mode)
        {
            this.uri = uri;
            this.mode = mode;

            return Open();
        }

        public Stream Open(Uri uri, Credentials credentials, DataFileMode mode, DataFileCompression compression)
        {
            this.uri = uri;
            this.mode = mode;
            this.compression = compression;

            return Open();
        }

        /// <summary>
        /// Opens a file identified by a URI for read or write.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mode"></param>
        /// <param name="compression"></param>
        /// <param name="archival"></param>
        /// <returns></returns>
        public Stream Open(Uri uri, Credentials credentials, DataFileMode mode, DataFileCompression compression, DataFileArchival archival)
        {
            this.uri = uri;
            this.mode = mode;
            this.compression = compression;
            this.archival = archival;

            return Open();
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
        public Stream Open()
        {
            switch (mode)
            {
                case DataFileMode.Read:
                    return OpenForRead();
                case DataFileMode.Write:
                    return OpenForWrite();
                default:
                    throw new NotImplementedException();
            }
        }

        public Stream Open(Stream stream)
        {
            switch (mode)
            {
                case DataFileMode.Read:
                    return OpenForRead(stream);
                case DataFileMode.Write:
                    return OpenForWrite(stream);
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
        private Stream OpenForRead()
        {
            var stream = OpenBaseStreamForRead();

            if (stream == null)
            {
                throw new FileFormatException("Unknown protocol.");      // TODO
            }

            return OpenForRead(stream);
        }

        private Stream OpenForRead(Stream stream)
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
        private Stream OpenForWrite()
        {
            var stream = OpenBaseStreamForWrite();

            if (stream == null)
            {
                throw new FileFormatException("Unknown protocol.");      // TODO
            }

            return OpenForWrite(stream);
        }

        private Stream OpenForWrite(Stream stream)
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
        protected virtual Stream OpenBaseStreamForRead()
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
                        return OpenHttpStream();
                    case Constants.UriSchemeFtp:
                        return OpenFtpStream();
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
        protected virtual Stream OpenBaseStreamForWrite()
        {
            if (!uri.IsAbsoluteUri || uri.IsFile)
            {
                return OpenFileStream();
            }
            else
            {
                switch (uri.Scheme.ToLowerInvariant())
                {
                    case Constants.UriSchemeFtp:
                        return OpenFtpStream();
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
        protected Stream OpenHttpStream()
        {
            // TODO: add authentication

            var req = WebRequest.Create(uri);

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

            var res = req.GetResponse();

            return res.GetResponseStream();
        }

        /// <summary>
        /// Opens an ftp stream for read.
        /// </summary>
        /// <returns></returns>
        protected Stream OpenFtpStream()
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

            var res = req.GetResponse();

            return res.GetResponseStream();
        }

        #endregion
        #region Utility functions

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
            // TODO: We could use mime type here if stream comes from HTTP

            var path = Util.UriConverter.ToFileName(uri);

            var extension = Path.GetExtension(path).ToLowerInvariant();
            DataFileCompression cm;

            switch (extension)
            {
                case Constants.FileExtensionGz:
                    cm = DataFileCompression.GZip;
                    break;
                case Constants.FileExtensionBz2:
                    cm = DataFileCompression.BZip2;
                    break;
                case Constants.FileExtensionZip:
                    cm = DataFileCompression.Zip;
                    break;
                default:
                    cm = DataFileCompression.None;
                    break;
            }

            return cm;
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
            var path = Util.UriConverter.ToFileName(uri);
            DataFileArchival am;

            var firstExtension = Path.GetExtension(path);
            var secondExtension = Path.GetExtension(Path.GetFileNameWithoutExtension(path));

            if (StringComparer.InvariantCultureIgnoreCase.Compare(firstExtension, Constants.FileExtensionZip) == 0)
            {
                am = DataFileArchival.Zip;
            }
            else if (
                StringComparer.InvariantCultureIgnoreCase.Compare(firstExtension, Constants.FileExtensionTar) == 0 ||
                StringComparer.InvariantCultureIgnoreCase.Compare(secondExtension, Constants.FileExtensionTar) == 0)
            {
                am = DataFileArchival.Tar;
            }
            else
            {
                am = DataFileArchival.None;
            }

            return am;
        }

        #endregion
    }
}
