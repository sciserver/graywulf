using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    [RemoteServiceClass(typeof(DataFileImporter))]
    public interface IDataFileImporter : ITableImporter
    {
        DataFileBase Source 
        {
            [OperationContract]
            get;

            [OperationContract]
            set; 
        }

        [OperationContract]
        void CreateDestinationTable();
    }
}
