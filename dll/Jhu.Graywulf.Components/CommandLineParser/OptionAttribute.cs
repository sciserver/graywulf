using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.CommandLineParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : ParameterAttribute
    {
        // TODO: this cannot be required, remove required property
        // by separating from the parameter class
    }
}
