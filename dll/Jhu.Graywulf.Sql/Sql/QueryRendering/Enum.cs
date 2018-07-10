using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.QueryRendering
{
    public enum IdentifierQuoting
    {
        AlwaysQuote,
        SelectivelyQuote
    }

    public enum NameRendering
    {
        Original,
        IdentifierOnly,
        FullyQualified,

        Default = Original
    }

    public enum AliasRendering
    {
        Default,
        Always,
        Never,
    }

    public enum VariableRendering
    {
        Original,
        Substitute,
        
        Default = Original
    }

}
