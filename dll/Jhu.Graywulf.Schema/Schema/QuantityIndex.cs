using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Schema
{
    public class QuantityIndex : ConcurrentDictionary<string, List<Variable>>
    {

        public static QuantityIndex Create(IEnumerable<Variable> variables)
        {
            var res = new QuantityIndex();

            foreach (Variable v in variables)
            {
                foreach (string q in v.Metadata.Quantity.Parts)
                {
                    if (res.ContainsKey(q))
                    {
                        res[q].Add(v);
                    }
                    else
                    {
                        res[q] = new List<Variable>();
                        res[q].Add(v);
                    }
                }
            }

            return res;
        }
    }


}
