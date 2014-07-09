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

        // TODO: add logic to handle user credentials

        private Uri uri;
        private DataFileMode mode;
        private DataFileCompression compression;
        private DataFileArchival archival;

        private string userName;
        private string password;

        /// <summary>
        /// Gets or sets the URI to open
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
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

        /// <summary>
        /// Gets or sets the username to access the URI
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the password to access the URI
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

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
            this.mode = DataFileMode.Read;
            this.compression = DataFileCompression.Automatic;
            this.archival = DataFileArchival.Automatic;

            this.userName = null;
            this.password = null;
        }

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
        public Stream Open(Uri uri, DataFileMode mode)
        {
            this.uri = uri;
            this.mode = mode;

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
        public Stream Open(Uri uri, DataFileMode mode, DataFileCompression compression, DataFileArchival archival)
        {
            this.uri = uri;
            this.mode = mode;
            this.compression = compression;
            this.archival = archival;

            return Open();
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

            // Check if compressed and wrap in compressed stream reader
            stream = WrapCompressedStreamForWrite(stream);

            // Check if archive and wrap in archive stream reader
            stream = WrapArchiveStreamForWrite(stream);

            return stream;
        }

        /// <summary>
        /// Opens the base stream for reading a file using the protocol specified by the URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="mode"></param>
        /// <param name="compression"></param>
        /// <returns></returns>
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
            var cm = GetCompressionMethod(Util.UriConverter.ToFileName(uri));

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
            var cm = GetCompressionMethod(Util.UriConverter.ToFileName(uri));

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
            var am = GetArchivalMethod(Util.UriConverter.ToFileName(uri));

            switch (am)
            {
                case DataFileArchival.None:
                    return baseStream;
                case DataFileArchival.Tar:
                    return new SharpZipLibWrapper.TarInputStream(baseStream)
                    {
                        IsStreamOwner = true
                    };
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
            var am = GetArchivalMethod(Util.UriConverter.ToFileName(uri));

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
                    return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                case DataFileMode.Write:
                    // Create directory first
                    var dir = Path.GetDirectoryName(path);
                    if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
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
            req.Credentials = GetCredentials();
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
            req.Credentials = GetCredentials();
            var res = req.GetResponse();

            return res.GetResponseStream();
        }

        #endregion
        #region Utility functions

        /// <summary>
        /// Gets username and password as network credentials for HTTP, FTP etc.
        /// </summary>
        /// <returns></returns>
        private ICredentials GetCredentials()
        {
            // TODO: Hopefully credentials from URIs are read automatically, test
            if (userName != null)
            {
                return new NetworkCredential(userName, password);
            }
            else
            {
                return new NetworkCredential("anonymous", "anonymous@");
            }
        }

        /// <summary>
        /// If compression is set to automatic, figures out compression
        /// method from the file extension
        /// </summary>
        /// <returns></returns>
        private DataFileCompression GetCompressionMethod(string path)
        {
            // TODO: We could use mime type here if stream comes from HTTP

            // Open compressed stream, if necessary
            var cm = compression;

            if (cm == DataFileCompression.Automatic)
            {
                var extension = Path.GetExtension(path).ToLowerInvariant();

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
            }

            return cm;
        }

        /// <summary>
        /// If archival method is set to automatic, figures it out from
        /// file extension.
        /// </summary>
        /// <returns></returns>
        private DataFileArchival GetArchivalMethod(string path)
        {
            var am = archival;

            if (am == DataFileArchival.Automatic)
            {
                // Look for second extension
                // TODO: this doesn't work with non-gzipped tar files but
                // whoever would use that?

                // Zip files
                var extension = Path.GetExtension(Path.GetFileNameWithoutExtension(path)).ToLowerInvariant();

                switch (extension)
                {
                    case Constants.FileExtensionTar:
                        am = DataFileArchival.Tar;
                        break;
                    default:
                        am = DataFileArchival.None;
                        break;
                }
            }

            return am;
        }

        #endregion
    }
}
