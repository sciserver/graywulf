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

New-Link "${ProjectDir}App_Masters\Basic" "${SolutionDir}graywulf\web\Jhu.Graywulf.Web.UI\App_Masters\Basic"
New-Link "${ProjectDir}App_Themes\Basic" "${SolutionDir}graywulf\web\Jhu.Graywulf.Web.UI\App_Themes\Basic"

New-Link "${ProjectDir}Apps\Common" "${SolutionDir}graywulf\web\Jhu.Graywulf.Web.UI\Apps\Common"
New-Link "${ProjectDir}Apps\Auth" "${SolutionDir}graywulf\web\Jhu.Graywulf.Web.UI\Apps\Auth"


#if not exist $(ProjectDir)App_Masters\Basic mklink /D $(ProjectDir)App_Masters\Basic $(SolutionDir)graywulf\web\Jhu.Graywulf.Web.UI\App_Masters\Basic
#if not exist $(ProjectDir)App_Themes\Basic mklink /D $(ProjectDir)App_Themes\Basic $(SolutionDir)graywulf\web\Jhu.Graywulf.Web.UI\App_Themes\Basic

#if not exist $(ProjectDir)Apps\Common mklink /D $(ProjectDir)Apps\Common $(SolutionDir)graywulf\web\Jhu.Graywulf.Web.UI\Apps\Common
#if not exist $(ProjectDir)Apps\Auth mklink /D $(ProjectDir)Apps\Auth $(SolutionDir)graywulf\web\Jhu.Graywulf.Web.UI\Apps\Auth
