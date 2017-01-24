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
            parseUnitString(unitString);
        }

        private void initalizeMembers()
        {
            factor = 1.0;
            parts = new List<UnitPart>();
        }

        private void parseUnitString(string unitString)
        {
            var parts =  unitString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var first = 0;
            if (double.TryParse(parts[0], out factor))
            {
                first = 1;
            }

            for (int i=first; i < parts.Length; i++ )
            {
                this.parts.Add(new UnitPart(parts[i]));
            }

        }

    }
}
