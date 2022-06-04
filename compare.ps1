# & "./build.ps1" -CleanDevDependencies
$SourceDirectory = "/workspaces/kaylumah.github.io/dist"
$DistDirectory = "/workspaces/kaylumah.github.io/compare"

Get-ChildItem -Path  $DistDirectory -Recurse 
    | Select-Object -ExpandProperty FullName 
    | Sort-Object length -Descending 
    | Remove-Item -force 

Get-ChildItem –Path $SourceDirectory -Recurse 
    | Move-Item -Destination $DistDirectory
