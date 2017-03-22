using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Unit
    {
        [NonSerialized]
        private static Regex regex = new Regex(@"(?<factor>\d+[e|E][\+|\-]\d+)|(?<unit>[\w\-\+\%]+)(\s)|(?<unit>[\w\-\+\%]+)$|(?<func>\w+\(.*?\)([\+|\-]\d+)?)", RegexOptions.ExplicitCapture);

        [NonSerialized]
        private double factor;

        [NonSerialized]
        private List<UnitPart> parts;

        [IgnoreDataMember]
        public double Factor
        {
            get { return factor; }
            set { factor = value; }
        }

        [IgnoreDataMember]
        public List<UnitPart> Parts
        {
            get { return parts; }
            set { parts = value; }
        }

        public Unit()
        {
            InitalizeMembers();
        }

        private void InitalizeMembers()
        {
            factor = 1.0;
            parts = new List<UnitPart>();
        }

        public static Unit Parse(string unitString)
        {
            var unit = new Unit();

            var mParts = regex.Match(unitString);

            if (!mParts.Success)
            {
                return unit;
            }

            double.TryParse(mParts.Groups["factor"].ToString(), out unit.factor);

            while (mParts.Success)
            {
                if (mParts.Groups["unit"].Length != 0)
                {
                    unit.parts.Add(new UnitEntity(mParts.Groups["unit"].ToString()));
                }

                else if (mParts.Groups["func"].Length != 0)
                {
                    unit.parts.Add(new UnitGroup(mParts.Groups["func"].ToString()));
                }
                mParts = mParts.NextMatch();
            }

            /*
            var parts = unitString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList<string>();

            if (parts.Count == 0)
            {
                return unit;
            }

            if (double.TryParse(parts[0], out unit.factor))
            {
                parts.RemoveAt(0);
            }

            parts.ForEach(p => unit.parts.Add(new UnitEntity(p)));
            */
            return unit;
        }

        override public string ToString()
        {
            if (factor == 1 | factor == 0)
            {
                return string.Join(" ", parts);
            }

            else
            {
                return string.Format("{0:0.###E+0} {1}", factor, string.Join(" ", parts));
            }
        }

        public string ToHtml()
        {
            var htmlParts = new List<string>();
            parts.ForEach(p => htmlParts.Add(p.ToHtml()));
            
            var s = string.Join(" ", htmlParts);
            if (factor != 1 & factor != 0)
            {
                s= string.Format("{0:0.###E+0} {1}", factor, s);
            }

            return s;
            // TODO: factor
            // TODO: special letters (greek, M_bol ...)

        }

        public string ToLatex()
        {
            var latexParts = new List<string>();
            parts.ForEach(p => latexParts.Add(p.ToLatex()));

            var s = string.Join("~", latexParts);
            if (factor != 1 & factor != 0)
            {
                s = string.Format(@"${{\rm {0} \times {1}}}$", factor, s);
            }

            return s;

            // Math.Floor
            // Math.

            // TODO: factor
            // TODO: special letters (greek, M_bol ...)
        }

    }
}
