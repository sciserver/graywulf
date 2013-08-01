using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf
{
    public class ServiceTesterToken : MarshalByRefObject, IDisposable
    {
        private ServiceTesterBase wrapper;
        private bool isExclusive;

        public bool IsExclusive
        {
            get { return isExclusive; }
        }

        public ServiceTesterToken(ServiceTesterBase wrapper, bool isExclusive)
        {
            InitializeLifetimeService();

            this.wrapper = wrapper;
            this.isExclusive = isExclusive;
        }

        private void InitializeMembers()
        {
            this.wrapper = null;
            this.isExclusive = false;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Dispose()
        {
            wrapper.ReleaseToken(this);
        }
    }
}
