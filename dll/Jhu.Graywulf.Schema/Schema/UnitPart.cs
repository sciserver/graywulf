﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    public class UnitPart
    {
        private string prefix;
        private string unitBase;
        private string exponent;
        private string function;

        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        public string Unit
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
            initializeMembers();
        }

        public UnitPart(string unit)
        {
            parseUnitPartString(unit);
        }
        private void initializeMembers()
        {
            prefix = "";
            unitBase = "";
            exponent = "";
            function = "";
        }


        private void parseUnitPartString(string part)
        {
            Regex sPattern = new Regex(@"(?<unit>\w+)(?<exp>[\+-]\d*[/]?\d*)?"); //

            Match m = sPattern.Match(part);
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

            exponent = m.Groups["exp"].Value;
        }
    }
}