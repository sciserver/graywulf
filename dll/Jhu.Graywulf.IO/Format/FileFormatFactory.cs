using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Net.Http.Headers;
using Jhu.Graywulf.Components;
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

        private List<DataFileBase> prototypes;
        private Dictionary<string, int> filesByExtension;
        private Dictionary<string, int> filesByMimeType;
        private bool isFileFormatsLoaded;

        #endregion
        #region Constructors and initializers

        protected FileFormatFactory()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.prototypes = new List<DataFileBase>();
            this.filesByExtension = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this.filesByMimeType = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this.isFileFormatsLoaded = false;
        }

        #endregion

        #region File formats

        private void LoadFileFormats()
        {
            prototypes.Clear();
            filesByExtension.Clear();
            filesByMimeType.Clear();

            foreach (var file in OnFilesLoading())
            {
                prototypes.Add(file);

                var idx = prototypes.Count - 1;

                if (!filesByExtension.ContainsKey(file.Description.Extension))
                {
                    filesByExtension.Add(file.Description.Extension, idx);
                }

                if (!filesByMimeType.ContainsKey(file.Description.MimeType))
                {
                    filesByMimeType.Add(file.Description.MimeType, idx);
                }
            }

            isFileFormatsLoaded = true;
        }

        /// <summary>
        /// When overriden in derived classes, collects supported file types in
        /// a collection.
        /// </summary>
        /// <param name="fileTypes"></param>
        protected virtual IEnumerable<DataFileBase> OnFilesLoading()
        {
            DataFileBase file;

            // -- Plain TXT file
            file = new DelimitedTextDataFile()
            {
                ColumnSeparators = "\t ",
            };
            file.Description.MimeType = Constants.MimeTypePlainText;
            file.Description.Extension = Constants.FileExtensionTxt;
            file.Description.DisplayName = FileFormatNames.AsciiFile;
            yield return file;

            // -- CSV file
            file = new DelimitedTextDataFile()
            {
                ColumnSeparators = ",",
            };
            file.Description.MimeType = Constants.MimeTypeCsv;
            file.Description.Extension = Constants.FileExtensionCsv;
            file.Description.DisplayName = FileFormatNames.CsvFile;
            yield return file;

            // -- XML file
            yield return new XHtmlDataFile();

            // -- SQL Server BCP file
            yield return new SqlServerNativeDataFile();
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

        public bool IsMimeTypeSupported(string mimeType)
        {
            return filesByMimeType.ContainsKey(mimeType);
        }

        public bool IsExtensionSupported(string extension)
        {
            return filesByExtension.ContainsKey(extension);
        }

        // TODO: this is only used by web form
        // delete when web form is changed to accept archives
        public DataFileBase CreateFile(Uri uri, out string filename, out string extension, out DataFileCompression compression)
        {
            DataFileBase file;
            if (!TryCreateFile(uri, out filename, out extension, out compression, out file))
            {
                throw CreateFileFormatUnknownException();
            }

            return file;
        }

        public bool TryCreateFile(Uri uri, out string filename, out string extension, out DataFileCompression compression, out DataFileBase file)
        {
            GetFileExtensions(uri, out filename, out extension, out compression);

            var res = TryCreateFileFromExtension(extension, out file);

            file.Uri = uri;

            return res;
        }

        public DataFileBase CreateFileFromExtension(string extension)
        {
            DataFileBase file;
            if (!TryCreateFileFromExtension(extension, out file))
            {
                throw CreateFileFormatUnknownException();
            }

            return file;
        }

        public bool TryCreateFileFromExtension(string extension, out DataFileBase file)
        {
            EnsureFileFormatsLoaded();

            int idx;
            if (filesByExtension.TryGetValue(extension, out idx))
            {
                file = (DataFileBase)prototypes[idx].Clone();
                return true;
            }

            file = null;
            return false;
        }

        public DataFileBase CreateFileFromMimeType(string mediaType)
        {
            DataFileBase file;
            if (!TryCreateFileFromMimeType(mediaType, out file))
            {
                throw CreateFileFormatUnknownException();
            }

            return file;
        }

        public bool TryCreateFileFromMimeType(string mediaType, out DataFileBase file)
        {
            EnsureFileFormatsLoaded();

            // Parse accept header
            var parts = mediaType.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                var accept = new MediaTypeWithQualityHeaderValue(parts[i]);

                int idx;
                if (filesByMimeType.TryGetValue(accept.MediaType, out idx))
                {
                    file = (DataFileBase)prototypes[idx].Clone();
                    return true;
                }
            }

            file = null;
            return false;
        }

        public DataFileBase CreateFileFromAcceptHeader(System.Collections.ObjectModel.Collection<System.Net.Mime.ContentType> accept)
        {
            DataFileBase file;
            if (!TryCreateFileFromAcceptHeader(accept, out file))
            {
                throw CreateFileFormatUnknownException();
            }

            return file;
        }

        public bool TryCreateFileFromAcceptHeader(System.Collections.ObjectModel.Collection<System.Net.Mime.ContentType> accept, out DataFileBase file)
        {
            EnsureFileFormatsLoaded();

            // Because we want to match patterns, look-up by mime type is not a way to go.
            // Loop over each item instead.
            for (int i = 0; i < accept.Count; i++)
            {
                foreach (var fileMime in filesByMimeType.Keys)
                {
                    if (Util.MediaTypeComparer.Compare(accept[i].MediaType, fileMime))
                    {
                        file = (DataFileBase)prototypes[filesByMimeType[fileMime]].Clone();
                        return true;
                    }
                }
            }

            // Fall back to plain text
            return TryCreateFileFromMimeType(Constants.MimeTypePlainText, out file);
        }

        public IEnumerable<FileFormatDescription> EnumerateFileFormatDescriptions()
        {
            EnsureFileFormatsLoaded();
            return prototypes.Select(f => f.Description);
        }

        #endregion

        /// <summary>
        /// Returns the file extension by stripping of the extension of the
        /// compressed file, if any.
        /// </summary>
        public virtual void GetFileExtensions(Uri uri, out string filename, out string extension, out DataFileCompression compressionMethod)
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

            DataFileBase file;
            if (!TryCreateFile(uri, out filename, out extension, out compression, out file))
            {
                throw CreateFileFormatUnknownException();
            }

            file.Uri = uri;
            return file;
        }

        public DataFileBase CreateFile(string filename)
        {
            return CreateFile(Util.UriConverter.FromFilePath(filename));
        }

        private FileFormatException CreateFileFormatUnknownException()
        {
            return new FileFormatException("Unknown file format.");  // *** TODO
        }
    }
}
