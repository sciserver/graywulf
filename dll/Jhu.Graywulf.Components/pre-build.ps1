param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

Get-Date
echo "SolutionDir: ||$SolutionDir||"
& "${SolutionDir}${OutDir}gwconfig.exe" merge $SolutionDir$SolutionName.sln $ProjectName | Out-String
Get-Date