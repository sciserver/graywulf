using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    public class UnitEntity : UnitPart
    {
        private static readonly Regex unitRegex = new Regex(@"((?<func>\w+)\()?(?<unit>\w+|%)\)?(?<exp>[\+-]\d*[/]?\d*)?", RegexOptions.Compiled);

        private string prefix;
        private string unitBase;
        private string exponent;

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


        public UnitEntity()
        {
            InitializeMembers();
        }

        public UnitEntity(string unit)
        {
            InitializeMembers();
            Parse(unit);
        }
        private void InitializeMembers()
        {
            prefix = "";
            unitBase = "";
            exponent = "";
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
                    if (unit.Length < p.Length)
                    {
                        continue;
                    }

                    if (unit.StartsWith(p) & Constants.UnitNames.Contains(unit.Substring(p.Length)))
                    {
                        prefix = p;
                        unitBase = unit.Substring(p.Length);
                        break;
                    }
                }
                if (prefix.Length == 0)
                {
                    unitBase = unit;
                }
            }

            exponent = m.Groups["exp"].Value;
            if (exponent.StartsWith("+"))
            {
                exponent = exponent.Substring(1);
            }

        }
        override public string ToString()
        {
            if (!exponent.StartsWith("-") & exponent.Length != 0)
            {
                exponent = string.Concat("+", exponent);
            }
            return string.Format("{0}{1}{2}", prefix, unitBase, exponent);

        }

        public override string ToHtml()
        {
            if (Constants.SpecialUnitCharsHtml.ContainsKey(prefix))
            {
                prefix = Constants.SpecialUnitCharsHtml[prefix];
            }

            if (Constants.SpecialUnitCharsHtml.ContainsKey(unitBase))
            {
                unitBase = Constants.SpecialUnitCharsHtml[unitBase];
            }

            var s = string.Format("{0}{1}", prefix, unitBase);

            if (exponent != "" & exponent != null)
            {
                s = string.Format("{0}<sup>{1}</sup>", s, exponent);
            }

            return s;
        }

        public override string ToLatex()
        {
            if (Constants.SpecialUnitCharsHtml.ContainsKey(prefix))
            {
                prefix = Constants.SpecialUnitCharsLatex[prefix];
            }

            if (Constants.SpecialUnitCharsHtml.ContainsKey(unitBase))
            {
                unitBase = Constants.SpecialUnitCharsLatex[unitBase];
            }

            var s = string.Format(@"{0}{1}", prefix, unitBase);

            if (exponent != "" & exponent != null)
            {
                s = string.Format(@"{0}^{{{1}}}", s, exponent);
            }

            return s;
        }


    }
}
