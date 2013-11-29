using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality to return a list of supported file formats
    /// and figure out file format from extensions.
    /// </summary>
    public class FileFormatFactory
    {
        #region Static utility functions

        public static string GetPathFromUri(Uri uri)
        {
            return uri.IsAbsoluteUri ? uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped) : uri.ToString();
        }

        /// <summary>
        /// Returns the file extension by stripping of the extension of the
        /// compressed file, if any.
        /// </summary>
        public static void GetExtensionWithoutCompression(Uri uri, out string path, out string extension, out DataFileCompression compressionMethod)
        {
            path = GetPathFromUri(uri);
            extension = Path.GetExtension(path);

            if (Constants.CompressionExtensions.ContainsKey(extension))
            {
                compressionMethod = Constants.CompressionExtensions[extension];
                path = Path.GetFileNameWithoutExtension(path);
                extension = Path.GetExtension(path);
            }
            else
            {
                compressionMethod = DataFileCompression.None;
            }
        }

        /// <summary>
        /// Returns a file object based on the format description.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DataFileBase CreateFile(FileFormatDescription format)
        {
            var c = format.Type.GetConstructor(Type.EmptyTypes);
            var f = (DataFileBase)c.Invoke(null);

            return f;
        }

        #endregion
        #region Constructors and initializers

        public FileFormatFactory()
        {
        }

        #endregion

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

        public FileFormatDescription GetFileFormatDescription(Uri uri, out string path, out string extension, out DataFileCompression compression)
        {
            GetExtensionWithoutCompression(uri, out path, out extension, out compression);

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
            fileTypes.Add(typeof(CsvFile));
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
    }
}
