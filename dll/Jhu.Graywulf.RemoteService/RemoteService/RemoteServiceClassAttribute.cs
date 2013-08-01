using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// This attribut is used to define the default service class
    /// to service contracts
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RemoteServiceClassAttribute : Attribute
    {
        public Type Type { get; set; }

        public RemoteServiceClassAttribute()
        {
        }

        public RemoteServiceClassAttribute(Type type)
        {
            Type = type;
        }
    }
}
