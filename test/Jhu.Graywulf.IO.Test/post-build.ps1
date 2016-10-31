param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

cp "$ProjectDir..\..\util\eseutil.exe" "$ProjectDir$OutDir"
cp "$ProjectDir..\..\util\ese.dll" "$ProjectDir$OutDir"