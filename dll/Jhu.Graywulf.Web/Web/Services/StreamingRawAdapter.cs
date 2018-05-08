using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawAdapter<T>
    {
        private WebHeaderCollection headers;

        public WebHeaderCollection Headers
        {
            get { return headers; }
            set { headers = value; }
        }

        protected StreamingRawAdapter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.headers = null;
        }

        public abstract List<RestBodyFormat> GetSupportedFormats();

        internal string GetPostedContentType()
        {
            var contentType = headers[HttpRequestHeader.ContentType];
            return contentType;
        }

        internal RestBodyFormat GetPreferredFormat()
        {
            var formats = GetSupportedFormats();
            return formats[0];
        }

        internal string GetRequestedContentType()
        {
            var acceptHeader = headers[HttpRequestHeader.Accept] ??
                               headers[HttpRequestHeader.ContentType];

            // Parse accept header
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();
            var formats = GetSupportedFormats();

            for (int i = 0; i < accept.Count; i++)
            {
                foreach (var format in formats)
                {
                    if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, format.MimeType))
                    {
                        return format.MimeType;
                    }
                }
            }

            return Constants.MimeTypeText;
        }

        public T ReadFromStream(Stream stream)
        {
            var contentType = GetPostedContentType();
            return ReadFromStream(stream, contentType);
        }

        public T ReadFromStream(Stream stream, string contentType)
        {
            return OnDeserializeRequest(stream, contentType);
        }

        protected abstract T OnDeserializeRequest(Stream stream, string contentType);

        public void WriteToStream(Stream stream, T value)
        {
            var contentType = GetRequestedContentType();
            WriteToStream(stream, contentType, value);
        }

        public void WriteToStream(Stream stream, string contentType, T value)
        {
            OnSerializeResponse(stream, contentType, value);
        }

        protected abstract void OnSerializeResponse(Stream stream, string contentType, T value);

    }
}
