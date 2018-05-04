using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatterBase : RestMessageFormatter
    {
        protected const string PriTemplateMatchResultsPropertyName = "UriTemplateMatchResults";

        private OperationDescription operation;
        private string httpMethod;
        private UriTemplate uriTemplate;
        private ServiceEndpoint endpoint;
        private StreamingRawFormatterDirection direction;
        private int inParameterIndex;

        private Dictionary<int, string> pathMapping;
        private Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private QueryStringConverter queryStringConverter;
        private int totalNumUTVars;

        internal OperationDescription Operation
        {
            get { return operation; }
        }

        internal UriTemplate UriTemplate
        {
            get { return uriTemplate; }
        }

        internal ServiceEndpoint Endpoint
        {
            get { return endpoint; }
        }

        internal StreamingRawFormatterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        internal int InParameterIndex
        {
            get { return inParameterIndex; }
            set { inParameterIndex = value; }
        }

        internal abstract Type FormattedType
        {
            get;
        }

        protected StreamingRawFormatterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.operation = null;
            this.endpoint = null;
            this.direction = StreamingRawFormatterDirection.None;
            this.inParameterIndex = 0;
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.operation = operationDescription;
            this.endpoint = endpoint;

            CreateOperationUri();
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.operation = operationDescription;
            this.endpoint = endpoint;

            CreateOperationUri();
        }

        private void GetUriTemplate(out string method, out string uriTemplate)
        {
            // From referencesource.ms.com

            var wga = operation.Behaviors.Find<WebGetAttribute>();
            var wia = operation.Behaviors.Find<WebInvokeAttribute>();

            if (wga != null)
            {
                method = "GET";
                uriTemplate = wga.UriTemplate;
            }
            else if (wia != null)
            {
                method = wia.Method;
                uriTemplate = wia.UriTemplate;
            }
            else
            {
                method = null;
                uriTemplate = null;
            }
        }

        private void CreateOperationUri()
        {
            GetUriTemplate(out string method, out string uri);
            this.httpMethod = method;
            this.uriTemplate = new UriTemplate(uri);

            // Rest if from referencesource.ms.com

            this.pathMapping = new Dictionary<int, string>();
            this.queryMapping = new Dictionary<int, KeyValuePair<string, Type>>();
            this.queryStringConverter = new RestQueryStringConverter();

            // TODO: this is the default query string converter only, what if
            // a service overrides it? How to get it from operation

            var neededPathVars = new List<string>(uriTemplate.PathSegmentVariableNames);
            var neededQueryVars = new List<string>(uriTemplate.QueryValueVariableNames);
            var alreadyGotVars = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

            this.totalNumUTVars = neededPathVars.Count + neededQueryVars.Count;
            
            for (int i = 0; i < operation.Messages[0].Body.Parts.Count; ++i)
            {
                MessagePartDescription mpd = operation.Messages[0].Body.Parts[i];
                string parameterName = mpd.Name;

                if (alreadyGotVars.ContainsKey(parameterName))
                {
                    throw new InvalidOperationException();
                }

                List<string> neededPathCopy = new List<string>(neededPathVars);
                foreach (string pathVar in neededPathCopy)
                {
                    if (string.Compare(parameterName, pathVar, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (mpd.Type != typeof(string))
                        {
                            throw new InvalidOperationException();
                        }

                        pathMapping.Add(i, parameterName);
                        alreadyGotVars.Add(parameterName, 0);
                        neededPathVars.Remove(pathVar);
                    }
                }
                List<string> neededQueryCopy = new List<string>(neededQueryVars);
                foreach (string queryVar in neededQueryCopy)
                {
                    if (string.Compare(parameterName, queryVar, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!queryStringConverter.CanConvert(mpd.Type))
                        {
                            throw new InvalidOperationException();
                        }

                        queryMapping.Add(i, new KeyValuePair<string, Type>(parameterName, mpd.Type));
                        alreadyGotVars.Add(parameterName, 0);
                        neededQueryVars.Remove(queryVar);
                    }
                }
            }

            if (neededPathVars.Count != 0)
            {
                throw new InvalidOperationException();
            }

            if (neededQueryVars.Count != 0)
            {
                throw new InvalidOperationException();
            }
        }

        protected void DeserializeUrlParameters(Message message, object[] parameters)
        {
            // From referencesource.ms.com

            UriTemplateMatch utmr = null;

            if (message.Properties.ContainsKey(PriTemplateMatchResultsPropertyName))
            {
                utmr = message.Properties[PriTemplateMatchResultsPropertyName] as UriTemplateMatch;
            }
            else
            {
                if (message.Headers.To != null && message.Headers.To.IsAbsoluteUri)
                {
                    utmr = UriTemplate.Match(endpoint.Address.Uri, message.Headers.To);
                }
            }

            var innerParameters = new object[parameters.Length - this.totalNumUTVars];
            var nvc = (utmr == null) ? new NameValueCollection() : utmr.BoundVariables;

            int j = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (this.pathMapping.ContainsKey(i) && utmr != null)
                {
                    parameters[i] = nvc[this.pathMapping[i]];
                }
                else if (this.queryMapping.ContainsKey(i) && utmr != null)
                {
                    string queryVal = nvc[this.queryMapping[i].Key];
                    parameters[i] = queryStringConverter.ConvertStringToValue(queryVal, this.queryMapping[i].Value);
                }
                else
                {
                    parameters[i] = innerParameters[j];
                    ++j;
                }
            }
        }

        protected void SerializeUrlParameters(Message message, object[] parameters)
        {
            var innerParameters = new object[parameters.Length - this.totalNumUTVars];
            var nvc = new NameValueCollection();

            int j = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (this.pathMapping.ContainsKey(i))
                {
                    nvc[this.pathMapping[i]] = parameters[i] as string;
                }
                else if (this.queryMapping.ContainsKey(i))
                {
                    if (parameters[i] != null)
                    {
                        nvc[this.queryMapping[i].Key] = queryStringConverter.ConvertValueToString(parameters[i], this.queryMapping[i].Value);
                    }
                }
                else
                {
                    innerParameters[j] = parameters[i];
                    ++j;
                }
            }

            var uri = uriTemplate.BindByName(endpoint.Address.Uri, nvc);
            message.Headers.To = uriTemplate.BindByName(endpoint.Address.Uri, nvc);

            if (WebOperationContext.Current != null)
            {
                if (httpMethod == "GET")
                {
                    WebOperationContext.Current.OutgoingRequest.SuppressEntityBody = true;
                }

                if (this.httpMethod != "*" && WebOperationContext.Current.OutgoingRequest.Method != null)
                {
                    WebOperationContext.Current.OutgoingRequest.Method = this.httpMethod;
                }
            }
            else
            {
                HttpRequestMessageProperty hrmp;

                if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    hrmp = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                }
                else
                {
                    hrmp = new HttpRequestMessageProperty();
                    message.Properties.Add(HttpRequestMessageProperty.Name, hrmp);
                }

                if (httpMethod == "GET")
                {
                    hrmp.SuppressEntityBody = true;
                }

                if (this.httpMethod != "*")
                {
                    hrmp.Method = this.httpMethod;
                }
            }
        }
    }
}
