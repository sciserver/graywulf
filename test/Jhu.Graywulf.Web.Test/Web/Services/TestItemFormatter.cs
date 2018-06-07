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
        protected override Type GetFormattedType()
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

        protected override void OnSerialize(Stream stream, string contentType, Type parameterType, object value)
        {
            throw new NotImplementedException();
        }

        protected override object OnDeserialize(Stream stream, string contentType, Type parameterType)
        {
            throw new NotImplementedException();
        }
    }
}
