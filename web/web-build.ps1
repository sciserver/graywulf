function New-Link($path, $target) {
	# if dir doesn't not exists, create it to create ascendants
	if (!(Test-Path "$path")) {
		mkdir "$path" 
	}

	# now delete the directory and make it a link, this fixes the case when
	# directiories are created by VS automatically and mklink cannot overwrite it
	# need to use cmd here, powershell doesn't support symbolic links
	if (Test-Path "$path") { 
		cmd /c rmdir /s /q "$path"
	}

	# create link
	cmd /c mklink /D "$path" "$target"
}

function Add-Master($name) {
	New-Link "${ProjectDir}App_Masters\$name" "${SolutionDir}modules\graywulf\web\Jhu.Graywulf.Web.UI\App_Masters\$name"
}

function Add-Theme($name) {
	New-Link "${ProjectDir}App_Themes\$name" "${SolutionDir}modules\graywulf\web\Jhu.Graywulf.Web.UI\App_Themes\$name"
}

function Add-App($name) {
	New-Link "${ProjectDir}Apps\$name" "${SolutionDir}modules\graywulf\web\Jhu.Graywulf.Web.UI\Apps\$name"
}

function Add-Script($package, $name) {
	New-Link "${ProjectDir}Scripts\$name" "${SolutionDir}packages\$package\content\Scripts\$name"
}