param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

& "${ProjectDir}${OutDir}${TargetName}.exe" "${ProjectDir}..\..\sql\Jhu.Graywulf.Entities\Jhu.Graywulf.Entities.Sql.Create.sql" "${ProjectDir}..\..\dll\Jhu.Graywulf.AccessControl\${OutDir}\Jhu.Graywulf.AccessControl.dll" "${SolutionDir}${OutDir}Jhu.Graywulf.Entities.Sql.Create.sql"
cp "${ProjectDir}..\..\sql\Jhu.Graywulf.Entities\Jhu.Graywulf.Entities.Sql.Drop.sql" "${SolutionDir}${OutDir}"


#$(TargetPath) $(ProjectDir)..\..\sql\Jhu.Graywulf.Entities\Jhu.Graywulf.Entities.Sql.Create.sql $(ProjectDir)..\..\dll\Jhu.Graywulf.AccessControl\$(OutDir)\Jhu.Graywulf.AccessControl.dll $(SolutionDir)$(OutDir)\Jhu.Graywulf.Entities.Sql.Create.sql
#copy $(ProjectDir)..\..\sql\Jhu.Graywulf.Entities\Jhu.Graywulf.Entities.Sql.Drop.sql $(SolutionDir)$(OutDir)\