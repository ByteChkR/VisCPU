#Delete Build Output
del Build -Recurse -Force


#Make sure the Project is built
dotnet clean D:\Users\Tim\Documents\VisCPU\src
dotnet build D:\Users\Tim\Documents\VisCPU\src

#Build Virtual Machine BIOS
. ./BuildVM.ps1

#Delete Builds to get rid of non-standard config files
del D:\Users\Tim\Documents\VisCPU\src\VisCPU.Console\bin\Debug\net5.0 -Recurse -Force
del D:\Users\Tim\Documents\VisCPU\src\VisCPU.Networking\bin\Debug\net5.0 -Recurse -Force

#Rebuild Vis Solution
dotnet clean D:\Users\Tim\Documents\VisCPU\src
dotnet build D:\Users\Tim\Documents\VisCPU\src

#Create Folders
mkdir Build\vis-core
mkdir Build\vis-net

#Copy to Output
cp D:\Users\Tim\Documents\VisCPU\src\VisCPU.Console\bin\Debug\net5.0\* Build\vis-core\ -Force -Recurse
cp D:\Users\Tim\Documents\VisCPU\src\VisCPU.Networking\bin\Debug\net5.0\* Build\vis-net\ -Force -Recurse

#Include ZIP Utils
. ./Scripts/Zip.ps1

#Create Zip from folders
Zip-Pack $pwd\Build\vis-core\ $pwd\Build\vis-core.zip
Zip-Pack $pwd\Build\bios\ $pwd\Build\vis-bios.zip
Zip-Pack $pwd\Build\vis-net\ $pwd\Build\vis-net.zip

#Delete Build Output
del .\Build\vis-core -Recurse -Force
del .\Build\bios -Recurse -Force
del .\Build\vis-net -Recurse -Force

