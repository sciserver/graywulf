using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Api
{
    public class RestErrorHandler : IErrorHandler
    {
        private ServiceBase service;

        public RestErrorHandler()
        {
        }

        public RestErrorHandler(ServiceBase service)
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
            HttpStatusCode statusCode;
            if (error is System.Security.SecurityException)
            {
                statusCode = System.Net.HttpStatusCode.Forbidden;
            }
            else
            {
                statusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            fault = WebOperationContext.Current.CreateTextResponse(error.Message);

            var response = WebOperationContext.Current.OutgoingResponse;

            response.SuppressEntityBody = false;
            response.StatusCode = statusCode;
            response.StatusDescription = error.Message;
        }
    }
}
