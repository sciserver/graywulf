using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mime;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services
{
    public class RestErrorHandler : IErrorHandler
    {
        private RestServiceBase service;

        public RestErrorHandler()
        {
        }

        public RestErrorHandler(RestServiceBase service)
        {
            this.service = service;
        }

        public bool HandleError(Exception error)
        {
            return true; // do not abort session
        }

        /// <summary>
        /// Replaces defaule error handling behavior by returning a status code
        /// and the plain text error message in the body.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="fault"></param>
        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            var restError = GetRestError(error);
            fault = SerializeRestError(restError);
            //SetHttpResponseStatus(restError.InnerException);
        }

        private RestError GetRestError(Exception ex)
        {
            if (ex is RestOperationException)
            {
                return ((RestOperationException)ex).RestError;
            }
            else
            {
                return new RestError(ex);
            }
        }

        private Message SerializeRestError(RestError ex)
        {
            var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();

            for (int i = 0; i < accept.Count; i++)
            {
                if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, "text/json") ||
                    Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, "application/json"))
                {
                    return WebOperationContext.Current.CreateJsonResponse<RestError>(ex);
                }
                else if (Jhu.Graywulf.Util.MediaTypeComparer.Compare(accept[i].MediaType, MediaTypeNames.Text.Plain))
                {
                    var sb = new StringBuilder();

                    sb.AppendFormat("An exception of type {0} occurred:", ex.Type);
                    sb.AppendLine();
                    sb.AppendLine(ex.Message);
                    sb.AppendLine();
                    sb.AppendFormat("Log event ID: {0}", ex.LogEventID);
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine(ex.StackTrace);

                    return WebOperationContext.Current.CreateTextResponse(sb.ToString());
                }
            }

            return WebOperationContext.Current.CreateXmlResponse<RestError>(ex);
        }

        private void SetHttpResponseStatus(Exception ex)
        {
            var response = WebOperationContext.Current.OutgoingResponse;

            HttpStatusCode statusCode;
            
            if (ex is System.Security.SecurityException)
            {
                statusCode = HttpStatusCode.Forbidden;
            }
            else if (ex is KeyNotFoundException ||
                ex is ResourceNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            response.SuppressEntityBody = false;
            response.StatusCode = statusCode;
            response.StatusDescription = ex.Message;
        }
    }
}
