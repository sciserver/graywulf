using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Schema
{
    public class Unit
    {
        private double factor;
        private List<UnitPart> parts;

        public double Factor
        {
            get { return factor; }
            set { factor = value; }
        }
        public List<UnitPart> Parts
        {
            get { return parts; }
        }

        public Unit()
        {
            InitalizeMembers();
        }

        public Unit(string unitString)
        {
            InitalizeMembers();
            Parse(unitString);
        }
        private void InitalizeMembers()
        {
            factor = 1.0;
            parts = new List<UnitPart>();
        }

        public Unit Parse(string unitString)
        {
            var parts = unitString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            
            if (double.TryParse(parts[0], out factor))
            {
                parts.RemoveAt(0);
            }

            parts.ForEach(p => this.parts.Add(new UnitPart(p)));

            return this;
        }
        
        

        override public string ToString()
        {
            if (factor == 1 | factor ==0)
            {
                return string.Join(" ", parts);
            }

            else {
                return string.Format("{0} {1}", factor, string.Join(" ", parts));
            } 
        }

        public string ToHtml()
        {
            var htmlParts = new List<string>();

            foreach (var p in parts)
            {
                var s = string.Format("{0}{1}", p.Prefix, p.UnitBase);

                if (p.Function != "" & p.Function != null)
                {
                    s = string.Format("{0}({1})", p.Function, s);
                }

                if (p.Exponent != "" & p.Exponent != null)
                {
                    s = string.Format("{0}<sup>{1}</sup>", s, p.Exponent);
                }

                htmlParts.Add(s);
            }

            return string.Format("{0} {1}", factor, string.Join(" ", htmlParts));

            // TODO: factor
            // TODO: special letters (greek, M_bol ...)

        }

        public string ToLatex()
        {
            var latexParts = new List<string>();

            foreach (var p in parts)
            {
                var s = string.Format(@"{0}{1}", p.Prefix, p.UnitBase);

            if (p.Function != "" & p.Function != null)
            {
                s = string.Format(@"{0}({1})", p.Function, s);
            }

            if (p.Exponent != "" & p.Exponent != null)
            {
                s = string.Format(@"{0}^{{{1}}}", s, p.Exponent);
            }
            latexParts.Add(s);

            }
            
            return string.Format(@"${{\rm {0} \times {1}}}$", factor, string.Join("~", latexParts));

            // Math.Floor
            // Math.

            // TODO: factor
            // TODO: special letters (greek, M_bol ...)
        }

    }
}
