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
        #region Constructors and initializers

        protected FileFormatFactory()
        {
        }

        #endregion

        /// <summary>
        /// Returns the file extension by stripping of the extension of the
        /// compressed file, if any.
        /// </summary>
        protected void GetExtensionWithoutCompression(Uri uri, out string filename, out string extension, out DataFileCompression compressionMethod)
        {
            var path = uri.PathAndQuery;    // This isn't always a file path, so it's the safest to do now
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
        /// Initializes a file object descriptios based on file type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private FileFormatDescription GetFileFormatDescription(Type type)
        {
            var f = (DataFileBase)Activator.CreateInstance(type, true);
            
            var fd = f.Description;
            fd.Type = type;

            return fd;
        }

        /// <summary>
        /// Returns an initialized file format descriptor.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public FileFormatDescription GetFileFormatDescription(string typeName)
        {
            return GetFileFormatDescriptions()[typeName];
        }

        public FileFormatDescription GetFileFormatDescription(Uri uri, out string filename, out string extension, out DataFileCompression compression)
        {
            GetExtensionWithoutCompression(uri, out filename, out extension, out compression);

            // FInd file format with the appropriate extensions
            FileFormatDescription format = null;
            foreach (var f in GetFileFormatDescriptions())
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(extension, f.Value.DefaultExtension) == 0)
                {
                    format = f.Value;
                    break;
                }
            }

            return format;
        }

        /// <summary>
        /// When overriden in derived classes, collects supported file types in
        /// a collection.
        /// </summary>
        /// <param name="fileTypes"></param>
        protected virtual void OnCreateFileFormatDescriptions(HashSet<Type> fileTypes)
        {
            fileTypes.Add(typeof(DelimitedTextDataFile));
            fileTypes.Add(typeof(SqlServerNativeDataFile));
        }

        /// <summary>
        /// Returns a list of supported file formats.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FileFormatDescription> GetFileFormatDescriptions()
        {
            // Gather file types
            var fileTypes = new HashSet<Type>();
            OnCreateFileFormatDescriptions(fileTypes);

            // Return them as a dictionary keys by typenames.
            var res = new Dictionary<string, FileFormatDescription>();
            foreach (var t in fileTypes)
            {
                res.Add(t.FullName, GetFileFormatDescription(t));
            }

            return res;
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

            var ffd = GetFileFormatDescription(uri, out filename, out extension, out compression);
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
