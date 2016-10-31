function New-Link($path, $target) {
	Write-Host "Creating link to $target"

	# if dir doesn't not exists, create it to create ascendants
	if (!(Test-Path "$path")) {
		mkdir "$path" 
	}

	# now delete the directory and make it a link, this fixes the case when
	# directiories are created by VS automatically and mklink cannot overwrite it
	# need to use cmd here, powershell doesn't support symbolic links
	if (Test-Path "$path") { 
		cmd /c rmdir "$path"
	}

	# create link
	cmd /c mklink /D "$path" "$target"
}