using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Jhu.Graywulf.Components
{
    public class DefaultDateTimeAttribute : DefaultValueAttribute
    {
        public DefaultDateTimeAttribute()
            : base(default(DateTime)) { }

        public DefaultDateTimeAttribute(string dateTime)
            : base(DateTime.Parse(dateTime, System.Globalization.CultureInfo.InvariantCulture)) { }
    }
}
