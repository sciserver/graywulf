using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services
{
    public class TestItemAdapter : StreamingRawAdapter<TestItem>
    {
        public override string[] GetSupportedMimeTypes()
        {
            return new string[]
                {
                    "application/json",
                    "text/xml",
                };
        }

        protected override void OnSerializeResponse(Stream stream, string contentType, TestItem value)
        {
            throw new NotImplementedException();
        }

        protected override TestItem OnDeserializeRequest(Stream stream, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}
