using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DataType : IDataTypeReference
    {
        private DataTypeReference dataTypeReference;

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return dataTypeReference; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return dataTypeReference; }
            set { dataTypeReference = value; }
        }

        public string TypeName
        {
            get { return FindDescendant<TypeName>().Value; }
        }

        public int Length
        {
            get
            {
                var s = FindDescendant<DataTypeSize>();

                if (s == null)
                {
                    return -1;
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

        public bool IsNullable
        {
            get
            {
                var k = FindDescendantRecursive<Jhu.Graywulf.Parsing.Literal>();

                if (k != null && SqlParser.ComparerInstance.Compare("not", k.Value) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.dataTypeReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (DataType)other;
            this.dataTypeReference = old.dataTypeReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.dataTypeReference = new DataTypeReference(this);
        }
    }
}
