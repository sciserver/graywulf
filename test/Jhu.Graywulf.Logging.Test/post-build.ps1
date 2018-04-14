# Copy plugins
$source = "$SolutionDir\plugins\$OutDir\*"
$target = "$ProjectDir$OutDir"
cp $source $target