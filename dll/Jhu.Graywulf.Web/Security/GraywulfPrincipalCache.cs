using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Security
{
    class GraywulfPrincipalCache : ConcurrentDictionary<string, GraywulfPrincipal>
    {
        public GraywulfPrincipalCache()
            :base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public GraywulfPrincipal GetOrAdd(GraywulfPrincipal item)
        {
            return GetOrAdd(GetKey(item), item);
        }

        public bool TryGetValue(GraywulfPrincipal item, out GraywulfPrincipal value)
        {
            return TryGetValue(GetKey(item), out value); 
        }

        public bool TryAdd(GraywulfPrincipal item)
        {
            return TryAdd(GetKey(item), item);
        }

        public bool TryRemove(GraywulfPrincipal item, out GraywulfPrincipal value)
        {
            return TryRemove(GetKey(item), out value);
        }

        private string GetKey(GraywulfPrincipal item)
        {
            var identity = (GraywulfIdentity)item.Identity;
            return String.Format("{0}|{1}|{2}", identity.Protocol, identity.Authority, identity.Identifier);
        }
    }
}
