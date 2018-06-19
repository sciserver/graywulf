using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SystemDataTypeIdentifier
    {
        public string TypeName
        {
            get { return FindDescendant<DataTypeName>().Value; }
        }

        public int Length
        {
            get
            {
                var s = FindDescendant<DataTypeSize>();

                if (s == null)
                {
                    return 0;
                }
                else
                {
                    var max = s.FindDescendant<Jhu.Graywulf.Parsing.Literal>();
                    if (max != null && SqlParser.ComparerInstance.Compare(max.Value, "max") == 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return int.Parse(s.FindDescendant<Number>(0).Value);
                    }
                }
            }
        }

        public byte Precision
        {
            get
            {
                var sp = FindDescendant<DataTypeScaleAndPrecision>();

                if (sp != null)
                {
                    return byte.Parse(sp.FindDescendant<Number>(0).Value);
                }
                else
                {
                    return 0;
                }
            }
        }

        public byte Scale
        {
            get
            {
                var sp = FindDescendant<DataTypeScaleAndPrecision>();

                if (sp != null)
                {
                    return byte.Parse(sp.FindDescendant<Number>(1).Value);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
