param ($projDir, $outPath)
echo "Project Path: $projDir"
echo "Output Path: $outPath"

echo "Moving files to Output Path"
cp "$projDir/Program.vbin" "$outPath"
cp "$projDir/Program.vbin.linkertext" "$outPath.linkertext"

