$ErrorActionPreference = "Continue"
dotnet build Firmeza.web\Firmeza.web.csproj *>&1 | Out-File -FilePath "build_output.txt" -Encoding UTF8
Get-Content "build_output.txt"
