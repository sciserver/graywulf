param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$ConfigurationName,
	[string]$TargetName
)

& "${SolutionDir}${OutDir}${ConfigurationName}\gwconfig.exe" merge $SolutionDir$SolutionName.sln $ProjectName
