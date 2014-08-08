using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    [ServiceContract]
    [RemoteService]
    public interface IRemoteServiceControl
    {
        /// <summary>
        /// Returns version information from the server.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [LimitedAccessOperation]
        string Hello();

        /// <summary>
        /// Returns info about the user under which the server operations run.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAuthenticated"></param>
        /// <param name="authenticationType"></param>
        [OperationContract]
        void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType);

        [OperationContract]
        void WhoAreYou(out string name, out bool isAuthenticated, out string authenticationType);

        /// <summary>
        /// Returns the URI to a service of type contractType
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [OperationContract]
        Uri GetServiceEndpointUri(string contractType);

        /// <summary>
        /// Returns a list of the registered service types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string[] QueryRegisteredServices();
    }
}
