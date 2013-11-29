using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Jhu.Graywulf.Format
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
        private Uri uri;
        private DataFileMode mode;
        private DataFileCompression compression;
        private DataFileArchival archival;

        private string userName;
        private string password;

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public DataFileMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public DataFileCompression Compression
        {
            get { return compression; }
            set { compression = value; }
        }

        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public StreamFactory()
        {
            InitializeMembers();
        }

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

        public Stream Open(Uri uri, DataFileMode mode)
        {
            this.uri = uri;
            this.mode = mode;

            return Open();
        }

        public Stream Open(Uri uri, DataFileMode mode, DataFileCompression compression, DataFileArchival archival)
        {
            this.uri = uri;
            this.mode = mode;
            this.compression = compression;
            this.archival = archival;

            return Open();
        }

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

        private Stream OpenForRead()
        {
            var stream = OpenBaseStreamForRead();
            
            // Check if compressed and wrap in compressed stream reader
            stream = WrapCompressedStreamForRead(stream);

            // Check if archive and wrap in archive stream reader
            stream = WrapArchiveStreamForRead(stream);

            return stream;
        }

        private Stream OpenForWrite()
        {
            var stream = OpenBaseStreamForWrite();

            // Check if compressed and wrap in compressed stream reader
            stream = WrapCompressedStreamForWrite(stream);

            // Check if archive and wrap in archive stream reader
            stream = WrapArchiveStreamForWrite(stream);

            return stream;
        }

        /// <summary>
        /// 
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

        private Stream WrapCompressedStreamForRead(Stream baseStream)
        {
            var cm = GetCompressionMethod(GetPathFromUri());

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

        private Stream WrapCompressedStreamForWrite(Stream baseStream)
        {
            var cm = GetCompressionMethod(GetPathFromUri());

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

        private Stream WrapArchiveStreamForRead(Stream baseStream)
        {
            var am = GetArchivalMethod(GetPathFromUri());

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

        private Stream WrapArchiveStreamForWrite(Stream baseStream)
        {
            var am = GetArchivalMethod(GetPathFromUri());

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

        protected Stream OpenFileStream()
        {
            var path = GetPathFromUri();

            switch (mode)
            {
                case DataFileMode.Read:
                    return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                case DataFileMode.Write:
                    return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                default:
                    throw new NotImplementedException();
            }
        }

        protected Stream OpenHttpStream()
        {
            // TODO: add authentication

            var req = WebRequest.Create(uri);
            req.Credentials = GetCredentials();
            var res = req.GetResponse();
            
            return res.GetResponseStream();
        }

        protected Stream OpenFtpStream()
        {
            // TODO: add authentication

            var req = FtpWebRequest.Create(uri);
            req.Credentials = GetCredentials();
            var res = req.GetResponse();

            return res.GetResponseStream();
        }

        #endregion
        #region Utility functions

        private string GetPathFromUri()
        {
            return uri.IsAbsoluteUri ? uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : uri.ToString();
        }

        private ICredentials GetCredentials()
        {
            // TODO: Hopefully credentials from URIs are read automatically, test
            if (userName != null)
            {
                return new NetworkCredential(userName, password);
            }
            else
            {
                return new NetworkCredential();
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
                // TODO: this doesn't work with non-zipped tar files but
                // whoever would use that?
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
