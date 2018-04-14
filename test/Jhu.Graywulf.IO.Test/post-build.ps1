if ( -not (Test-Path "$ProjectDir$OutDir\eseutil.exe")) {
  cp "$ProjectDir..\..\util\eseutil.exe" "$ProjectDir$OutDir"
  cp "$ProjectDir..\..\util\ese.dll" "$ProjectDir$OutDir"
}

# Copy plugins
$source = "$SolutionDir\plugins\$OutDir\*"
$target = "$ProjectDir$OutDir"
cp $source $target