using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    public class TextMessageFormatter : RawMessageFormatterBase
    {
        protected override Type GetFormattedType()
        {
            return null;
        }

        public override List<RestBodyFormat> GetSupportedFormats()
        {
            return new List<RestBodyFormat>()
            {
                RestBodyFormats.Text
            };
        }

        protected override void OnSerialize(Stream stream, string contentType, Type parameterType, object value)
        {
            var writer = new TextResponseMessageBodyWriter(value);
            writer.WriteBodyContents(stream);
        }

        protected override object OnDeserialize(Stream stream, string contentType, Type parameterType)
        {
            throw new NotImplementedException();
        }
    }
}