using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Schema
{
    public class QuantityIndex
    {
        private Dictionary<string, List<Variable>> variableIndex;
        private Dictionary<Variable, HashSet<string>> quantityIndex;

        public Dictionary<string, List<Variable>> VariableIndex
        {
            get { return variableIndex; }
        }

        public QuantityIndex(IEnumerable<Variable> variables)
        {
            InitializeMembers();
            Build(variables);
        }

        private void InitializeMembers()
        {
            this.variableIndex = new Dictionary<string, List<Variable>>();
            this.quantityIndex = new Dictionary<Variable, HashSet<string>>();
        }

        public void Build (IEnumerable<Variable> variables)
        {
            foreach (Variable v in variables)
            {
                Add(v);
            }
        }

        private void Add(Variable variable)
        {
            foreach (string q in variable.Metadata.Quantity.Parts)
            {
                if (!variableIndex.ContainsKey(q))
                {
                    variableIndex[q] = new List<Variable>();
                }

                variableIndex[q].Add(variable);
            }

            quantityIndex[variable] = new HashSet<string>(variable.Metadata.Quantity.Parts, Quantity.Comparer);
        }

        public List<Variable> SearchQuantity(IList<string> quantities)
        {
            var res = new List<Variable>();
            var qs = new HashSet<string>(quantities, Quantity.Comparer);
            
            if (quantities.Count > 0 && variableIndex.Keys.Contains(quantities[0]))
            {
                foreach (var v in variableIndex[quantities[0]])
                {
                    if (qs.IsSubsetOf(quantityIndex[v]))
                    {
                        res.Add(v);
                    }
                }
            }

            return res;
        }
    }


}
