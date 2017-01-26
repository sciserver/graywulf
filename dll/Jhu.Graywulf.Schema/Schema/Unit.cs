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
            ParseUnitString(unitString);
        }

        private void initalizeMembers()
        {
            factor = 1.0;
            parts = new List<UnitPart>();
        }

        private void ParseUnitString(string unitString)
        {
            var parts =  unitString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            
            if (double.TryParse(parts[0], out factor))
            {
                parts.RemoveAt(0);
            }

            parts.ForEach(p => this.parts.Add(new UnitPart(p)));

        }

        override public string ToString()
        {
            var s = string.Format("{0} {1}",factor,string.Join(" ",parts));
            return s;
        }
       

    }
}
