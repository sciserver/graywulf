rm $ProjectDir$OutDir\Microsoft.SqlServer.*.dll
rm $ProjectDir$OutDir\Microsoft.SqlServer.*.xml

& "${SolutionDir}${OutDir}gwpgen.exe" generate -a ..\..\build\Jhu.Graywulf.Sql.Parser.Grammar\$OutDir\Jhu.Graywulf.Sql.Parser.Grammar.dll -t Jhu.Graywulf.Sql.Parser.Grammar.SqlGrammar -o Sql\Parsing\SqlParser.g.cs