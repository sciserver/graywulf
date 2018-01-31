using System;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Data contract interface for delegated tasks.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface IRemoteService : Jhu.Graywulf.Tasks.ICancelableTask
    {
        [OperationContract(AsyncPattern = true)]
        Task ExecuteAsync();
    }
}
