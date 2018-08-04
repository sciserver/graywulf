rm $ProjectDir$OutDir\Microsoft.SqlServer.*.dll
rm $ProjectDir$OutDir\Microsoft.SqlServer.*.xml

& "${SolutionDir}${OutDir}gwpgen.exe" generate -a ..\..\build\Jhu.Graywulf.Sql.Grammar\$OutDir\Jhu.Graywulf.Sql.Grammar.dll -t Jhu.Graywulf.Sql.Grammar.SqlGrammar -o Sql\Parsing\SqlParser.g.cs