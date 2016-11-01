param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

& "${SolutionDir}${OutDir}gwconfig.exe" merge $SolutionDir$SolutionName.sln $ProjectName

cd "${ProjectDir}..\..\build\Jhu.Graywulf.SqlParser.Generator\${OutDir}"
.\gwsqlpgen.exe generate -o ..\..\..\..\dll\Jhu.Graywulf.Sql\SqlParser\SqlParser.cs