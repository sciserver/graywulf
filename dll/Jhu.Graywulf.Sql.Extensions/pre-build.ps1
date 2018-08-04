rm $ProjectDir$OutDir\Microsoft.SqlServer.*.dll
rm $ProjectDir$OutDir\Microsoft.SqlServer.*.xml

& "${SolutionDir}${OutDir}gwpgen.exe" generate -a ..\..\build\Jhu.Graywulf.Sql.Extensions.Grammar\$OutDir\Jhu.Graywulf.Sql.Extensions.Grammar.dll -t Jhu.Graywulf.Sql.Extensions.Grammar.GraywulfSqlGrammar -o Sql\Extensions\Parsing\GraywulfSqlParser.g.cs