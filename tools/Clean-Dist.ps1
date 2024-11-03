Write-Host "Cleaning Up DevDependencies"
Remove-Item dist/styles.css
Remove-Item dist/package.json
Remove-Item dist/package-lock.json
Remove-Item dist/tailwind.config.js
Remove-Item dist/node_modules -Recurse -Force