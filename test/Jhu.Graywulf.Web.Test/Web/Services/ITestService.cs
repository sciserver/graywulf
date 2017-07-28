using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Jhu.Graywulf.Web.Services
{
    [ServiceContract]
    [ServiceName(Name = "Test", Version = "v1")]
    [Description("This is a service.")]
    public interface ITestService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

        [OperationContract]
        [WebInvoke(UriTemplate = "/proxy", Method = HttpMethod.Get)]
        Stream GenerateProxy();

        [OperationContract]
        [WebGet(UriTemplate="/SimpleFunction")]
        void SimpleFunction();

        [OperationContract]
        [WebGet(UriTemplate = "/SinglePathPartFunction/{parameter}")]
        void SinglePathPartFunction(string parameter);

        [OperationContract]
        [WebGet(UriTemplate = "/SingleQueryPartFunction?parameter={parameter}")]
        void SingleQueryPartFunction(string parameter);

        [OperationContract]
        [WebGet(UriTemplate = "/MultipleQueryPartFunction?parameter={parameter}&parameter2={parameter2}")]
        void MultipleQueryPartFunction(string parameter);

        [OperationContract]
        [WebGet(UriTemplate = "/PathAndQueryPartFunction/{pathParameter}?parameter={parameter}&parameter2={parameter2}")]
        void PathAndQueryParameterFunction(string parameter);

        [OperationContract]
        [WebGet(UriTemplate = "/SingleParameterFunction")]
        void SingleParameterFunction(string parameter);
    }
}
