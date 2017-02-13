using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Web.Admin
{
    public class PageBase : Jhu.Graywulf.Web.UI.PageBase
    {
        private string GetFormDataCacheKey(Guid guid)
        {
            var key = Web.UI.Constants.ApplicationFormDataCache + "_" + guid.ToString();
            return key;
        }

        public Guid FormCacheSave(object data)
        {
            var guid = new Guid();
            var key = GetFormDataCacheKey(guid);

            Page.Cache[key] = data;
            
            return guid;
        }

        public object FormCacheLoad(Guid guid)
        {
            var key = GetFormDataCacheKey(guid);

            return Page.Cache[key];
        }
    }
}