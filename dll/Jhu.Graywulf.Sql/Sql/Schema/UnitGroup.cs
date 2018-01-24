using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Sql.Schema
{
    // class for units with function
    public class UnitGroup: UnitPart
    {
        private Regex regex = new Regex(@"(?<func>\w+)\((?<units>.*?)\)(?<exp>[\+|\-]\d+)?");

        private List<UnitEntity> parts;
        private string function;
        private string exponent;

        public List<UnitEntity> Parts
        {
            get { return parts; }
        }

        public string Function
        {
            get { return function; }
            set { function = value; }
        }

        public string Exponent
        {
            get { return exponent; }
            set { exponent = value; }
        }

        public UnitGroup()
        {
            InitializeMembers();
        }

        public UnitGroup(string groupString)
        {
            InitializeMembers();
            Parse(groupString);
        }

        private void InitializeMembers()
        {
            parts = new List<UnitEntity>();
            function = "";
            exponent = "";
        }

        private void Parse(string groupString)
        {
            var mParts = regex.Match(groupString);

            function = mParts.Groups["func"].ToString();
            exponent = mParts.Groups["exp"].ToString();

            var units = mParts.Groups["units"].ToString().Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();
            units.ForEach(u => parts.Add(new UnitEntity(u)));
        }

        public override string ToString()
        {
            var units = new List<string>();
            parts.ForEach(p => units.Add(p.ToString()));

            if (!exponent.StartsWith("-") & exponent.Length != 0)
            {
                exponent = string.Concat("+", exponent);
            }

            return string.Format("{0}({1}){2}",function,string.Join(" ",units),exponent);
            
        }

        public override string ToHtml()
        {
            var units = new List<string>();
            parts.ForEach(p => units.Add(p.ToHtml()));
            var s = string.Format("{0}({1})", function, string.Join(" ",units));

            if (exponent != "" & exponent != null)
            {
                s = string.Format("{0}<sup>{1}</sup>", s, exponent);
            }

            return s;
        }

        public override string ToLatex()
        {
            var units = new List<string>();
            parts.ForEach(p => units.Add(p.ToLatex()));

            var s = string.Format(@"{0}({1})", function, string.Join("~", units));
            
            if (exponent != "" & exponent != null)
            {
                s = string.Format(@"{0}^{{{1}}}</sup>", s, exponent);
            }

            return s;
        }
    }
}
