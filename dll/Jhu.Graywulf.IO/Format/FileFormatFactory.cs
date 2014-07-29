using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality to return a list of supported file formats
    /// and figure out file format from extensions.
    /// </summary>
    public class FileFormatFactory
    {
        #region Static members

        public static FileFormatFactory Create(string typename)
        {
            Type type = null;

            if (typename != null)
            {
                type = Type.GetType(typename);
            }

            // If config is incorrect, fall back to known types.
            if (type == null)
            {
                type = typeof(FileFormatFactory);
            }

            return (FileFormatFactory)Activator.CreateInstance(type, true);
        }

        #endregion
        #region Private member variables

        private Dictionary<Type, FileFormatDescription> fileFormats;
        private Dictionary<string, Type> fileFormatsByExtension;
        private Dictionary<string, Type> fileFormatsByMimeType;
        private bool isFileFormatsLoaded;

        #endregion
        #region Constructors and initializers

        protected FileFormatFactory()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.fileFormats = new Dictionary<Type, FileFormatDescription>();
            this.fileFormatsByExtension = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
            this.fileFormatsByMimeType = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
            this.isFileFormatsLoaded = false;
        }

        #endregion

        #region File formats

        private void LoadFileFormats()
        {
            foreach (var mapping in OnFileFormatsLoading())
            {
                if (!fileFormats.ContainsKey(mapping.Type))
                {
                    fileFormats.Add(mapping.Type, LoadFileFormat(mapping.Type));
                }

                if (!fileFormatsByExtension.ContainsKey(mapping.Extension))
                {
                    fileFormatsByExtension.Add(mapping.Extension, mapping.Type);
                }

                if (!fileFormatsByMimeType.ContainsKey(mapping.MimeType))
                {
                    fileFormatsByExtension.Add(mapping.MimeType, mapping.Type);
                }
            }

            isFileFormatsLoaded = true;
        }

        /// <summary>
        /// When overriden in derived classes, collects supported file types in
        /// a collection.
        /// </summary>
        /// <param name="fileTypes"></param>
        protected virtual IEnumerable<FileFormatMapping> OnFileFormatsLoading()
        {
            yield return new FileFormatMapping
            {
                Extension = Constants.FileExtensionCsv,
                MimeType = Constants.MimeTypeCsv,
                Type = typeof(DelimitedTextDataFile)
            };

            yield return new FileFormatMapping
            {
                Extension = Constants.FileExtensionBcp,
                MimeType = Constants.MimeTypeBcp,
                Type = typeof(SqlServerNativeDataFile)
            };

            yield return new FileFormatMapping
            {
                Extension = Constants.FileExtensionHtml,
                MimeType = Constants.MimeTypeHtml,
                Type = typeof(XHtmlDataFile)
            };
        }

        /// <summary>
        /// Initializes a file object descriptios based on file type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private FileFormatDescription LoadFileFormat(Type type)
        {
            var f = (DataFileBase)Activator.CreateInstance(type, true);

            var fd = f.Description;
            fd.Type = type;

            return fd;
        }

        private void EnsureFileFormatsLoaded()
        {
            if (!isFileFormatsLoaded)
            {
                LoadFileFormats();
            }
        }

        #endregion
        #region File format description access functions

        public FileFormatDescription GetFileFormat(Uri uri, out string filename, out string extension, out DataFileCompression compression)
        {
            GetExtensionWithoutCompression(uri, out filename, out extension, out compression);
            return GetFileFormatFromExtension(extension);
        }

        public FileFormatDescription GetFileFormat(Type type)
        {
            EnsureFileFormatsLoaded();
            return fileFormats[type];
        }

        public FileFormatDescription GetFileFormatFromExtension(string extension)
        {
            EnsureFileFormatsLoaded();
            return fileFormats[fileFormatsByExtension[extension]];
        }

        public FileFormatDescription GetFileFormatFromMimeType(string mimeType)
        {
            EnsureFileFormatsLoaded();
            return fileFormats[fileFormatsByMimeType[mimeType]];
        }

        public IEnumerable<FileFormatDescription> EnumerateFileFormatDescriptions()
        {
            EnsureFileFormatsLoaded();
            return fileFormats.Values;
        }

        #endregion

        /// <summary>
        /// Returns the file extension by stripping of the extension of the
        /// compressed file, if any.
        /// </summary>
        protected void GetExtensionWithoutCompression(Uri uri, out string filename, out string extension, out DataFileCompression compressionMethod)
        {
            var path = Util.UriConverter.ToFilePath(uri);    // This isn't always a file path, so it's the safest to do now
            extension = Path.GetExtension(path);

            if (Jhu.Graywulf.IO.Constants.CompressionExtensions.ContainsKey(extension))
            {
                filename = Path.GetFileNameWithoutExtension(path);
                extension = Path.GetExtension(path);
                compressionMethod = Jhu.Graywulf.IO.Constants.CompressionExtensions[extension];
            }
            else
            {
                filename = Path.GetFileName(path);
                extension = Path.GetExtension(path);
                compressionMethod = DataFileCompression.None;
            }
        }

        /// <summary>
        /// Returns a file object based on the format description.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public DataFileBase CreateFile(FileFormatDescription format)
        {
            var c = format.Type.GetConstructor(Type.EmptyTypes);
            var f = (DataFileBase)c.Invoke(null);

            return f;
        }

        public DataFileBase CreateFile(Uri uri)
        {
            string filename, extension;
            DataFileCompression compression;

            var ffd = GetFileFormat(uri, out filename, out extension, out compression);
            var format = CreateFile(ffd);

            format.Uri = uri;

            return format;
        }

        public DataFileBase CreateFile(string filename)
        {
            return CreateFile(Util.UriConverter.FromFilePath(filename));
        }
    }
}
