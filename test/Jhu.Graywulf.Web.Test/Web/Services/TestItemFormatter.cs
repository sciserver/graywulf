using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Jhu.Graywulf.Web.Services.Serialization;

namespace Jhu.Graywulf.Web.Services
{
    public class TestItemFormatter : RawMessageFormatterBase
    {
        public override Type GetFormattedType()
        {
            return typeof(TestItem);
        }

        public override List<RestBodyFormat> GetSupportedFormats()
        {
            return new List<RestBodyFormat>()
            {
                RestBodyFormats.Json,
                RestBodyFormats.Text
            };
        }

        protected override void OnSerializeResponse(Stream stream, string contentType, object value)
        {
            throw new NotImplementedException();
        }

        protected override object OnDeserializeRequest(Stream stream, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}
