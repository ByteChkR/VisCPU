param ($projDir, $outPath, $defSymbols)
echo "Project Path: $projDir"
echo "Output Path: $outPath"
echo "Cleaning.."
vis project clean $projDir
echo "Building.."
vis project make $projDir Publish
vis project make $projDir Release -imp:args $defSymbols

./Scripts/MoveProject.ps1 -projDir $projDir -outPath $outPath