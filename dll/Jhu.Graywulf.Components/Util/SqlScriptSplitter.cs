using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Util
{
    public static class SqlScriptSplitter
    {
        /// <summary>
        /// Splits SQL script into chunks base on the GO statements.
        /// </summary>
        /// <param name="script">The script to chunk up.</param>
        /// <returns>The script chunks.</returns>
        public static string[] SplitByGo(string script)
        {
            var parts = script.Split(new string[] { "\r\nGO", "\nGO", "\r\ngo", "\ngo" }, StringSplitOptions.RemoveEmptyEntries);
            return parts;
        }
    }
}
