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
        public abstract string[] GetSupportedMimeTypes();

        private WebHeaderCollection GetRequestHeaders()
        {
            var request = OperationContext.Current.RequestContext.RequestMessage;
            var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            return prop.Headers;
        }

        internal string GetPostedContentType()
        {
            var headers = GetRequestHeaders();
            var contentType = headers[HttpRequestHeader.ContentType];
            return contentType;
        }

        internal string GetRequestedContentType()
        {
            var headers = GetRequestHeaders();
            var acceptHeader = headers[HttpRequestHeader.Accept] ?? 
                               headers[HttpRequestHeader.ContentType];

            // Parse accept header
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();

            for (int i = 0; i < accept.Count; i++)
            {
                foreach (var mime in GetSupportedMimeTypes())
                {
                    if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, mime))
                    {
                        return mime;
                    }
                }
            }

            return null;
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
