﻿cp $ProjectDir$OutDir$TargetName.exe $SolutionDir$OutDir
cp $ProjectDir$OutDir$TargetName.pdb $SolutionDir$OutDir
cp $ProjectDir$OutDir$TargetName.exe.config $SolutionDir$OutDir

# Copy plugins
$source = "$SolutionDir\plugins\$ConfigurationName\*"
$target = "$ProjectDir$OutDir"
cp $source $target