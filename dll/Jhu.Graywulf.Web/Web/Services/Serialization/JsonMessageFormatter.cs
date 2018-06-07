using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public class JsonMessageFormatter : RawMessageFormatterBase
    {
        protected override Type GetFormattedType()
        {
            return null;
        }

        public override List<RestBodyFormat> GetSupportedFormats()
        {
            return new List<RestBodyFormat>()
            {
                RestBodyFormats.Json
            };
        }

        protected override void OnSerializeResponse(Stream stream, string contentType, Type parametersType, object value)
        {
            using (var writer = new StreamWriter(stream))
            {
                var ser = new Newtonsoft.Json.JsonSerializer();
                ser.Serialize(writer, value);
            }
        }

        protected override object OnDeserializeRequest(Stream stream, string contentType, Type parameterType)
        {
            using (var reader = new StreamReader(stream))
            {
                var ser = new Newtonsoft.Json.JsonSerializer();
                return ser.Deserialize(reader, parameterType);
            }
        }
    }
}
