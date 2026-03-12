# Generate TypeScript interfaces from C# ValueObjects
Write-Host "Generating TypeScript interfaces..." -ForegroundColor Green

# Build the project first
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Generate OpenAPI specification and TypeScript client
Write-Host "Generating TypeScript from OpenAPI..." -ForegroundColor Yellow

# Install NSwag CLI if not installed
$nswagVersion = "14.2.0"
$toolInstalled = dotnet tool list -g | Select-String "NSwag.ConsoleCore"

if (-not $toolInstalled) {
    Write-Host "Installing NSwag CLI tool..." -ForegroundColor Yellow
    dotnet tool install -g NSwag.ConsoleCore --version $nswagVersion
}

# Run NSwag to generate TypeScript
nswag run nswag.json

if ($LASTEXITCODE -eq 0) {
    Write-Host "TypeScript interfaces generated successfully!" -ForegroundColor Green
    Write-Host "Output: ../Frontend/src/app/api/api-client.ts" -ForegroundColor Cyan
} else {
    Write-Host "TypeScript generation failed!" -ForegroundColor Red
    exit 1
}
