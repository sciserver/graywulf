using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    public class UnitPart
    {
        private static readonly Regex unitRegex = new Regex(@"((?<func>\w+)\()?(?<unit>\w+)\)?(?<exp>[\+-]\d*[/]?\d*)?", RegexOptions.Compiled);

        private string prefix;
        private string unitBase;
        private string exponent;
        private string function;

        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        public string UnitBase
        {
            get { return unitBase; }
            set { unitBase = value; }
        }

        public string Exponent
        {
            get { return exponent; }
            set { exponent = value; }
        }

        public string Function
        {
            get { return function; }
            set { function = value; }
        }

        public UnitPart()
        {
            InitializeMembers();
        }

        public UnitPart(string unit)
        {
            InitializeMembers();
            Parse(unit);
        }
        private void InitializeMembers()
        {
            prefix = "";
            unitBase = "";
            exponent = "";
            function = "";
        }


        private void Parse(string part)
        {
            Match m = unitRegex.Match(part);
            var unit = m.Groups["unit"].Value;

            if (Constants.UnitNames.Contains(unit))
            {
                unitBase = unit;
            }
            else
            {
                foreach (string p in Constants.UnitPrefixes.Keys.ToList())
                {
                    if (unit.StartsWith(p) & Constants.UnitNames.Contains(unit.Substring(p.Length)))
                    {
                        prefix = p;
                        unitBase = unit.Substring(p.Length);
                        break;
                    }
                    else
                    {
                        unitBase = unit;
                    }
                }
            }

            function = m.Groups["func"].Value;
            exponent = m.Groups["exp"].Value;
        }
        override public string ToString()
        {
            if (function == "" | function == null)
            {
                return string.Format("{0}{1}{2}", prefix, unitBase, exponent);
            }
            else
            {
                return string.Format("{0}({1}{2}){3}", function, prefix, unitBase, exponent);
            }
        }
        
    }
}
