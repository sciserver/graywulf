using System;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICopyDataStream : IRemoteService
    {
        [OperationContract]
        Task OpenAsync();

        [OperationContract]
        void Open();

        [OperationContract]
        void Close();
    }
}
