dotnet publish ..\VisCPU.HL.Integration\VisCPU.HL.Integration.csproj -c Release
[string]$sourceDirectory  = "..\VisCPU.HL.Integration\bin\Release\netstandard2.0\publish\*"
[string]$destinationDirectory = ".\Assets\VisCPU\Impl"
Copy-item -Force -Recurse $sourceDirectory -Destination $destinationDirectory