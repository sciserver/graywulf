using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Parsing.Generator
{
    /// <summary>
    /// Represents a source code generator that can be
    /// used to tabulate generated code.
    /// </summary>
    class CodeStringBuilder
    {
        StringBuilder code;

        public CodeStringBuilder()
        {
            code = new StringBuilder();
        }

        public void Append(string value)
        {
            code.Append(value);
        }

        public void AppendLine()
        {
            code.AppendLine();
        }

        public void AppendLine(int tabs, string value)
        {
            AppendTabs(tabs);
            code.AppendLine(value);
        }

        public void AppendLineFormat(int tabs, string format, params object[] args)
        {
            AppendTabs(tabs);
            code.AppendFormat(format, args);
            code.AppendLine();
        }

        private void AppendTabs(int tabs)
        {
            for (int i = 0; i < tabs; i ++)
            {
                code.Append("    ");
            }
        }

        public override string ToString()
        {
            return code.ToString();
        }
    }
}
