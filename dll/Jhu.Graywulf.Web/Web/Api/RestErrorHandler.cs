using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Api
{
    public class RestErrorHandler : IErrorHandler
    {
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
            // wrap exception so the framework doesn't choke on it
            var wfe = new WebFaultException<Exception>(error, System.Net.HttpStatusCode.InternalServerError);

            fault = System.ServiceModel.Channels.Message.CreateMessage(
                version,
                wfe.CreateMessageFault(),
                "http://"
            );

            var response = WebOperationContext.Current.OutgoingResponse;

            response.SuppressEntityBody = true;
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            response.StatusDescription = error.Message;
            response.ContentType = "text/plain";
            HttpContext.Current.Response.Write(error.Message);
        }
    }
}
