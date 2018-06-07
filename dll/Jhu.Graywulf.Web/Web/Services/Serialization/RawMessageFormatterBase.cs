using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public abstract class RawMessageFormatterBase : RestMessageFormatterBase
    {
        #region Constants

        protected const string UriTemplateMatchResultsPropertyName = "UriTemplateMatchResults";

        #endregion
        #region Private member variables

        private OperationDescription operation;
        private string httpMethod;
        private UriTemplate uriTemplate;
        private ServiceEndpoint endpoint;
        private RawMessageFormatterDirection direction;
        private Type returnValueType;
        private Type inParameterType;
        private int inParameterIndex;
        private Type formattedType;
        private string mimeType;

        private Dictionary<int, string> pathMapping;
        private Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private QueryStringConverter queryStringConverter;
        private int totalNumUTVars;

        #endregion
        #region Properties

        protected OperationDescription Operation
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

        internal RawMessageFormatterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        protected Type ReturnValueType
        {
            get { return returnValueType; }
        }

        protected Type InParameterType
        {
            get { return inParameterType; }
        }

        internal int InParameterIndex
        {
            get { return inParameterIndex; }
        }

        public Type FormattedType
        {
            get { return formattedType; }
        }

        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        #endregion
        #region Constructors and initializers

        protected RawMessageFormatterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.operation = null;
            this.httpMethod = null;
            this.uriTemplate = null;
            this.endpoint = null;
            this.direction = RawMessageFormatterDirection.None;
            this.returnValueType = null;
            this.inParameterType = null;
            this.inParameterIndex = 0;
            this.formattedType = null;
            this.mimeType = null;
        }

        public void Initialize()
        {
            this.formattedType = GetFormattedType();
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            this.operation = operationDescription;
            this.endpoint = endpoint;

            ConfigureFormatter();
            CreateOperationUri();
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.Initialize(operationDescription, endpoint);
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.Initialize(operationDescription, endpoint);
        }

        #endregion
        #region Parameter handling

        protected abstract Type GetFormattedType();

        private Type[] GetParameterTypes(OperationDescription operationDescription)
        {
            var inmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Input);
            var res = new Type[inmsg.Body.Parts.Count];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = inmsg.Body.Parts[i].Type;
            }

            return res;
        }

        private Type GetRetvalType(OperationDescription operationDescription)
        {
            var outmsg = operationDescription.Messages.First(m => m.Direction == MessageDirection.Output);
            return outmsg.Body.ReturnValue.Type;
        }

        private bool CanHandleType(Type parameterType)
        {
            if (formattedType == parameterType)
            {
                return true;
            }
            else if (formattedType == null)
            {
                if (parameterType == typeof(Stream) ||
                    parameterType == typeof(String) ||
                    parameterType.IsPrimitive)
                {
                    return false;
                }
                else if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        internal void ConfigureFormatter()
        {
            this.formattedType = GetFormattedType();

            var parameterTypes = GetParameterTypes(operation);
            var retvalType = GetRetvalType(operation);

            if (CanHandleType(retvalType))
            {
                returnValueType = retvalType;
                direction |= RawMessageFormatterDirection.ReturnValue;
            }

            for (int i = 0; i < parameterTypes.Length; i++)
            {
                if (CanHandleType(parameterTypes[i]))
                {
                    inParameterIndex = i;
                    inParameterType = parameterTypes[i];
                    direction |= RawMessageFormatterDirection.ParameterIn;
                }
            }
        }

        #endregion
        #region URI template handling

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

            if (message.Properties.ContainsKey(UriTemplateMatchResultsPropertyName))
            {
                utmr = message.Properties[UriTemplateMatchResultsPropertyName] as UriTemplateMatch;
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

        #endregion
        #region Read and write functions and hooks

        public object ReadFromStream(Stream stream, string contentType, Type parameterType)
        {
            return OnDeserialize(stream, contentType, parameterType);
        }

        protected abstract object OnDeserialize(Stream stream, string contentType, Type parameterType);

        public void WriteToStream(Stream stream, string contentType, Type parameterType, object value)
        {
            OnSerialize(stream, contentType, parameterType, value);
        }

        protected abstract void OnSerialize(Stream stream, string contentType, Type parameterType, object value);

        protected override void OnSetMessageHeaders(HttpRequestMessageProperty http)
        {
            base.OnSetMessageHeaders(http);
            http.Headers[HttpRequestHeader.ContentType] = mimeType;
        }

        #endregion
        #region Server

        public override void DeserializeRequest(Message message, object[] parameters)
        {
            if (!message.IsEmpty &&
                (direction & RawMessageFormatterDirection.ParameterIn) != 0)
            {
                DeserializeUrlParameters(message, parameters);

                var headers = GetRequestHeaders(message);
                var contentType = GetPostedContentType(headers);
                var buffer = new byte[0x4000];
                var body = message.GetReaderAtBodyContents();

                using (var ms = new MemoryStream())
                {
                    while (true)
                    {
                        // TODO: this needs to be made async somehow
                        var cnt = body.ReadContentAsBase64(buffer, 0, buffer.Length);

                        if (cnt == 0)
                        {
                            break;
                        }
                        else
                        {
                            ms.Write(buffer, 0, cnt);
                        }
                    }

                    ms.Seek(0, SeekOrigin.Begin);

                    parameters[InParameterIndex] = ReadFromStream(ms, contentType, inParameterType);
                }
            }
            else
            {
                base.DeserializeRequest(message, parameters);
            }
        }

        public override Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            if ((direction & RawMessageFormatterDirection.ReturnValue) != 0 && returnValueType != typeof(void))
            {
                var headers = GetRequestHeaders();
                var contentType = GetRequestedContentType(headers);

                var message = WebOperationContext.Current.CreateStreamResponse(
                    stream =>
                    {
                        WriteToStream(stream, contentType, returnValueType, result);
                    },
                    contentType);

                SetMessageHeaders(message);

                return message;
            }
            else
            {
                return base.SerializeReply(messageVersion, parameters, result);
            }
        }

        #endregion
        #region Client

        public override Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if ((Direction & RawMessageFormatterDirection.ParameterIn) != 0 &&
                parameters != null)
            {
                var data = parameters[InParameterIndex];
                var body = new RawBodyWriter(this, mimeType, inParameterType, data);
                var action = Operation.Messages[0].Action;
                var message = Message.CreateMessage(messageVersion, action, body);

                SetMessageHeaders(message);
                SerializeUrlParameters(message, parameters);

                return message;
            }
            else
            {
                return base.SerializeRequest(messageVersion, parameters);
            }
        }

        public override object DeserializeReply(Message message, object[] parameters)
        {
            if ((Direction & RawMessageFormatterDirection.ReturnValue) != 0 && returnValueType != typeof(void))
            {
                object bodyFormatProperty;
                if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) ||
                    (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
                {
                    // **** TODO
                    throw new InvalidOperationException("Incoming messages must have a body format of Raw. Is a ContentTypeMapper set on the WebHttpBinding?");
                }

                var body = message.GetReaderAtBodyContents();
                byte[] raw = body.ReadContentAsBase64();

                using (var ms = new MemoryStream(raw))
                {
                    var headers = GetResponseHeaders(message);
                    var contentType = GetPostedContentType(headers);
                    return ReadFromStream(ms, contentType, returnValueType);
                }
            }
            else
            {
                return base.DeserializeReply(message, parameters);
            }
        }

        #endregion
    }
}
