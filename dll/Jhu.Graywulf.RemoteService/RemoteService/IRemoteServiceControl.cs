using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    [ServiceContract]
    [RemoteService]
    [ServiceLoggingBehavior]
    public interface IRemoteServiceControl
    {
        /// <summary>
        /// Returns version information from the server.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        Task<string> HelloAsync();

        /// <summary>
        /// Returns info about the user under which the server operations run.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAuthenticated"></param>
        /// <param name="authenticationType"></param>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        Task<AuthenticationDetails> WhoAmIAsync();

        [OperationContract]
        Task<AuthenticationDetails> WhoAreYouAsync();

        /// <summary>
        /// Returns the URI to a service of type contractType
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        Task<Uri> GetServiceEndpointUriAsync(string contractType);

        /// <summary>
        /// Returns a list of the registered service types.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        Task<string[]> QueryRegisteredServicesAsync();
    }
}
