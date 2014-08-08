using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// This attribute is used to define the default service class
    /// to service contracts
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RemoteServiceAttribute : Attribute
    {
        public Type Type { get; set; }

        public RemoteServiceAttribute()
        {
        }

        public RemoteServiceAttribute(Type type)
        {
            Type = type;
        }
    }
}
