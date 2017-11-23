using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Util
{
    public static class EnumParser
    {
        public static void SetListItemsFlags<T>(System.Web.UI.WebControls.ListControl list, T flags)
            where T : struct, IFormattable, IComparable, IConvertible
        {
            var ff = flags.ToUInt64(System.Globalization.CultureInfo.InvariantCulture);

            foreach (System.Web.UI.WebControls.ListItem item in list.Items)
            {
                T v;

                if (Enum.TryParse(item.Value, true, out v))
                {
                    var vv = v.ToUInt64(System.Globalization.CultureInfo.InvariantCulture);
                    item.Selected = (ff & vv) != 0;
                }
            }
        }

        public static T GetListItemFlags<T>(System.Web.UI.WebControls.ListControl list)
            where T : struct, IFormattable, IComparable, IConvertible
        {
            UInt64 ff = 0;

            foreach (System.Web.UI.WebControls.ListItem item in list.Items)
            {
                T v;

                if (item.Selected && Enum.TryParse(item.Value, true, out v))
                {
                    var vv = v.ToUInt64(System.Globalization.CultureInfo.InvariantCulture);
                    ff |= vv;
                }
            }

            return (T)Enum.ToObject(typeof(T), ff);
        }
    }
}
