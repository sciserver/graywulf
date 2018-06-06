using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public static class RestBodyFormats
    {
        public static RestBodyFormat Text
        {
            get { return new RestBodyFormat("text", "txt", Constants.MimeTypeText); }
        }

        public static RestBodyFormat Xml
        {
            get { return new RestBodyFormat("xml", "xml", Constants.MimeTypeXml); }
        }

        public static RestBodyFormat Json
        {
            get { return new RestBodyFormat("json", "json", Constants.MimeTypeJson); }
        }

        public static RestBodyFormat Jpeg
        {
            get { return new RestBodyFormat("jpeg", "jpg", Constants.MimeTypeJpeg); }
        }

        public static RestBodyFormat Png
        {
            get { return new RestBodyFormat("png", "png", Constants.MimeTypePng); }
        }

        public static RestBodyFormat Gif
        {
            get { return new RestBodyFormat("gif", "gif", Constants.MimeTypeGif); }
        }

        public static RestBodyFormat Bmp
        {
            get { return new RestBodyFormat("bitmap", "bmp", Constants.MimeTypeBmp); }
        }

        public static RestBodyFormat Pdf
        {
            get { return new RestBodyFormat("pdf", "pdf", Constants.MimeTypePdf); }
        }

        public static RestBodyFormat Eps
        {
            get { return new RestBodyFormat("eps", "eps", Constants.MimeTypeEps); }
        }

        public static RestBodyFormat Emf
        {
            get { return new RestBodyFormat("emf", "emf", Constants.MimeTypeEmf); }
        }
    }
}
