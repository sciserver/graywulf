using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Controls
{
    public class Int32ArrayConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                throw base.GetConvertFromException(value);
            }

            if (((string)value).Length == 0)
            {
                return new Int32[0];
            }

            var parts = ((string)value).Split(new char[] { ',' });
            var res = new Int32[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                res[i] = Int32.Parse(parts[i], culture);
            }

            return res;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(destinationType == typeof(string)))
            {
                throw base.GetConvertToException(value, destinationType);
            }

            if (value == null)
            {
                return String.Empty;
            }

            return String.Join(",", (Int32[])value);
        }
    }
}
