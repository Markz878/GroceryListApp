$packages = npm outdated --json | ConvertFrom-Json

foreach ($key in $packages.PSObject.Properties.Name) {
    Write-Host "Updating $key to latest version..."
    npm install "$key@latest"
}