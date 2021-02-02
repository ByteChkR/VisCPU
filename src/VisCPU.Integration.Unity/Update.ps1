dotnet publish ..\VisCPU.Integration\VisCPU.Integration.csproj -c Release
[string]$sourceDirectory  = "..\VisCPU.Integration\bin\Release\netstandard2.0\publish\*"
[string]$destinationDirectory = ".\Assets\VisCPU\Impl"
Copy-item -Force -Recurse $sourceDirectory -Destination $destinationDirectory