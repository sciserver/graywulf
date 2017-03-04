cp $ProjectDir$OutDir$TargetName.exe $SolutionDir$OutDir
cp $ProjectDir$OutDir$TargetName.pdb $SolutionDir$OutDir
cp $ProjectDir$OutDir$TargetName.exe.config $SolutionDir$OutDir

if ( -not (test-path "$SolutionDir$OutDir\eseutil.exe") ) {
  cp "$ProjectDir..\..\util\eseutil.exe" "$SolutionDir$OutDir"
  cp "$ProjectDir..\..\util\ese.dll" "$SolutionDir$OutDir"
}
