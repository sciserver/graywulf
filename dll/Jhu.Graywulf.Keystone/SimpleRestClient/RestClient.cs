using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Jhu.Graywulf.SimpleRestClient
{
    public abstract class RestClient
    {
        private Uri baseUri;

        /// <summary>
        /// Gets or sets the base URI of the REST service.
        /// </summary>
        public Uri BaseUri
        {
            get { return baseUri; }
            set { baseUri = value; }
        }

        protected RestClient(Uri baseUri)
        {
            this.baseUri = baseUri;
        }

        protected virtual RestHeaderCollection SendRequest(HttpMethod method, string path, RestHeaderCollection headers)
        {
            var response = SendRequestInternal(method, path, null, headers);
            var resheaders = ReadResponseHeaders(response);

            return resheaders;
        }

        protected virtual RestMessage<R> SendRequest<R>(HttpMethod method, string path, RestHeaderCollection headers)
        {
            var response = SendRequestInternal(method, path, null, headers);
            var resheaders = ReadResponseHeaders(response);
            var resbody = ReadResponseBody(response);

            return new RestMessage<R>(
                DeserializeJson<R>(resbody),
                resheaders
            );
        }

        /// <summary>
        /// Sends a REST request that does not return a body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual RestHeaderCollection SendRequest<T>(HttpMethod method, string path, RestMessage<T> message)
        {
            var response = SendRequestInternal(method, path, SerializeJson(message.Body), message.Headers);
            var resheaders = ReadResponseHeaders(response);

            return resheaders;
        }

        /// <summary>
        /// Sends a REST requests, parses and returns its body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual RestMessage<R> SendRequest<T, R>(HttpMethod method, string path, RestMessage<T> message)
        {
            var reqbody = SerializeJson(message.Body);
            var response = SendRequestInternal(method, path, reqbody, message.Headers);
            var resheaders = ReadResponseHeaders(response);
            var resbody = ReadResponseBody(response);

            return new RestMessage<R>(
                DeserializeJson<R>(resbody),
                resheaders
            );
        }

        /// <summary>
        /// Composes and sends a REST request, processes response.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private HttpWebResponse SendRequestInternal(HttpMethod method, string path, string body, RestHeaderCollection headers)
        {
            var req = (HttpWebRequest)WebRequest.Create(CreateAbsoluteUri(path));

            req.Method = method.ToString().ToUpper();
            req.SendChunked = false;
            req.ContentType = Constants.JsonMimeType;

            WriteRequestHeaders(req, headers);

            switch (method)
            {
                case HttpMethod.Get:
                case HttpMethod.Delete:
                case HttpMethod.Head:
                    break;
                case HttpMethod.Post:
                case HttpMethod.Put:
                case HttpMethod.Patch:
                    WriteRequestBody(req, body);
                    break;
                default:
                    throw new NotImplementedException();
            }

            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw;
                }
                else
                {
                    // TODO
                    var rex = new RestException(ExceptionMessages.InternalRestError, ex)
                    {
                        Body = ReadResponseBody(ex.Response)
                    };

                    throw rex;
                }
            }
        }

        /// <summary>
        /// Writes a string into a REST request body.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        private void WriteRequestBody(WebRequest request, string body)
        {
            var stream = request.GetRequestStream();

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(body);
            }
        }

        /// <summary>
        /// Reads the response of a REST request as string.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string ReadResponseBody(WebResponse response)
        {
            var stream = response.GetResponseStream();

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Writes the headers into an HTTP request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        private void WriteRequestHeaders(HttpWebRequest request, RestHeaderCollection headers)
        {
            if (headers != null)
            {
                foreach (var header in headers.Values)
                {
                    request.Headers.Add(header.Name, header.Value);
                }
            }
        }

        /// <summary>
        /// Extracts and returns the headers from a HTTP response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private RestHeaderCollection ReadResponseHeaders(WebResponse response)
        {
            var headers = new RestHeaderCollection();

            for (int i = 0; i < response.Headers.Count; i++)
            {
                headers.Add(new RestHeader(response.Headers.Keys[i], response.Headers[i]));
            }

            return headers;
        }

        /// <summary>
        /// Serializes an object into a JSON document.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected string SerializeJson(object body)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(body, settings);
        }

        /// <summary>
        /// Deserializes a JSON document into an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <returns></returns>
        protected T DeserializeJson<T>(string body)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(body, settings);
        }

        /// <summary>
        /// Creates and absolute URI from a base URI and a relative path.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private Uri CreateAbsoluteUri(string path)
        {
            return new Uri(baseUri, path);
        }

        protected string UrlEncode(string value)
        {
            return System.Web.HttpUtility.UrlEncode(value);
        }

    }
}
