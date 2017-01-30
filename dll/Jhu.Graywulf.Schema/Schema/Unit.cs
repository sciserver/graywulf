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
            initalizeMembers();
        }

        public Unit(string unitString)
        {
            initalizeMembers();
            Parse(unitString);
        }

        private void initalizeMembers()
        {
            factor = 1.0;
            parts = new List<UnitPart>();
        }

        private void Parse(string unitString)
        {
            var parts = unitString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            if (double.TryParse(parts[0], out factor))
            {
                parts.RemoveAt(0);
            }

            parts.ForEach(p => this.parts.Add(new UnitPart(p)));

        }

        override public string ToString()
        {
            var s = string.Format("{0} {1}", factor, string.Join(" ", parts));
            return s;
        }

        public string ToHtml()
        {
            var htmlParts = new List<string>();
            parts.ForEach(p => htmlParts.Add(p.ToHtml()));
            return string.Format("{0} {1}", factor, string.Join(" ", htmlParts));

           // TODO: factor
           // TODO: special letters (greek, M_bol ...)
           
        }

        public string ToLatex()
        {
            var latexParts = new List<string>();
            parts.ForEach(p => latexParts.Add(p.ToLatex()));
            return string.Format(@"${{\rm {0} \times {1}}}$",factor, string.Join("~", latexParts));


            // TODO: factor
            // TODO: special letters (greek, M_bol ...)
        }

    }
}
