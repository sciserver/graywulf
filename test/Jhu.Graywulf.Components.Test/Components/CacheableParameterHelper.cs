using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components.Test
{
    public class CacheableParameterHelper : GenericParameterHelper, ICacheable
    {
        // Fields
        private bool isCacheable;
        private long cachedVersion;

        // Methods
        public CacheableParameterHelper()
            :base()
        {
            InitializeMembers();
        }

        public CacheableParameterHelper(CacheableParameterHelper old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.isCacheable = false;
            this.cachedVersion = 0;
        }

        private void CopyMembers(CacheableParameterHelper old)
        {
            this.isCacheable = old.isCacheable;
            this.cachedVersion = old.cachedVersion;
        }

 
        #region ICacheable Members

        public void Touch()
        {
            throw new NotImplementedException();
        }

        public bool IsCacheable
        {
            get { return isCacheable; }
            set { isCacheable = value; }
        }

        public long CachedVersion
        {
            get { return cachedVersion; }
        }

        #endregion
    }

 

}
