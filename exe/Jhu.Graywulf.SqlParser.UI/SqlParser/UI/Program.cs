using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Jhu.Graywulf.Sql.Jobs.Query;

namespace Jhu.Graywulf.Parser.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Main form = new Main();

            //form.Parser = qf.
            //form.RootNodeType = typeof(Jhu.Graywulf.SqlParser.SelectStatement);

            //form.Parser = new Jhu.SkyQuery.Parser.SkyQueryParser();
            //form.RootNodeType = typeof(Jhu.SkyQuery.Parser.SelectStatement);

            //form.Parser = new Jhu.Graywulf.BnfParser.BnfParser();
            //form.RootNodeType = typeof(Jhu.Graywulf.BnfParser.Syntax);

            
            Application.Run(form);
        }
    }
}
