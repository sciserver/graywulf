param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$ConfigurationName,
	[string]$TargetName
)

. ../lib-build.ps1

& "${SolutionDir}${OutDir}${ConfigurationName}\gwconfig.exe" merge $SolutionDir$SolutionName.sln $ProjectName

Add-Master Basic
Add-Theme Basic
Add-App Common
Add-App Docs
