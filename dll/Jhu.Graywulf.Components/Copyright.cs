using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf
{
    public static class Copyright
    {
#if DEBUG
        public const string InfoConfiguration = "Debug";
#else
        public const string InfoConfiguration = "Release";
#endif
        public const string InfoProduct = "Graywulf";
        public const string InfoCompany = "ELTE/JHU IDIES";
        public const string InfoCopyright = "Copyright ©  2008-2014 László Dobos, Eötvös University, The Johns Hopkins University";
        public const string InfoTrademark = "";
        public const string InfoCulture = "";
    }
}
