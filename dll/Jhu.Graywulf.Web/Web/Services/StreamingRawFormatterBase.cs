using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatterBase : RestMessageFormatter, IDispatchMessageFormatter, IClientMessageFormatter
    {
        private StreamingRawFormatterDirection direction;

        public StreamingRawFormatterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        internal abstract Type FormattedType
        { get; }

        protected StreamingRawFormatterBase(IDispatchMessageFormatter dispatchMessageFormatter)
            : base(dispatchMessageFormatter)
        {
            InitializeMembers();
        }

        protected StreamingRawFormatterBase(IClientMessageFormatter clientMessageFormatter)
            : base(clientMessageFormatter)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.direction = StreamingRawFormatterDirection.None;
        }
    }
}
