dotnet publish ..\VisCPU.Integration\VisCPU.Integration.csproj -c Release
dotnet publish ..\VisCPU.Peripherals\VisCPU.Peripherals.csproj -c Release
dotnet publish ..\VisCPU.Instructions\VisCPU.Instructions.csproj -c Release
[string]$sourceDirectory  = "..\VisCPU.Integration\bin\Release\netstandard2.0\publish\*"
[string]$destinationDirectory = ".\Assets\VisCPU\Impl"
Copy-item -Force -Recurse $sourceDirectory -Destination $destinationDirectory
[string]$sourceDirectory  = "..\VisCPU.Peripherals\bin\Release\netstandard2.0\publish\*"
Copy-item -Force -Recurse $sourceDirectory -Destination $destinationDirectory
[string]$sourceDirectory  = "..\VisCPU.Instructions\bin\Release\netstandard2.0\publish\*"
Copy-item -Force -Recurse $sourceDirectory -Destination $destinationDirectory