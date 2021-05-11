. ./vis-make.ps1


$buildOutputPath = ".\Build\"

if (Test-Path $buildOutputPath){
    echo "Deleting Previous Builds"
    rm -Path .\Build\ -Recurse -Force
}

mkdir $buildOutputPath
mkdir $buildOutputPath/bios
mkdir $buildOutputPath/bios/apps
mkdir $buildOutputPath/bios/libs



./Scripts/MakeProject.ps1 -projDir ./Kernel -outPath $buildOutputPath/bios/kernel.vbin -defSymbols "INSTALLTARGET=VM"

./Scripts/MakeProject.ps1 -projDir ./KernelLibs/cfgloader -outPath $buildOutputPath/bios/libs/cfgloader.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelLibs/consoletools -outPath $buildOutputPath/bios/libs/consoletools.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelLibs/time -outPath $buildOutputPath/bios/libs/time.vbin

./Scripts/MakeProject.ps1 -projDir ./KernelApps/alias -outPath $buildOutputPath/bios/apps/alias.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/clear -outPath $buildOutputPath/bios/apps/clear.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/console -outPath $buildOutputPath/bios/apps/console.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/copy -outPath $buildOutputPath/bios/apps/copy.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/delete -outPath $buildOutputPath/bios/apps/delete.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/echo -outPath $buildOutputPath/bios/apps/echo.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/kernelloader -outPath $buildOutputPath/bios/apps/kernelloader.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/kerneltools -outPath $buildOutputPath/bios/apps/kerneltools.vbin
./Scripts/MakeProject.ps1 -projDir ./KernelApps/move -outPath $buildOutputPath/bios/apps/move.vbin

./Clean.ps1