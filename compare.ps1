& "./build.ps1" -CleanDevDependencies
$SourceDirectory = "C:\projects\kaylumah.github.io\dist"
$DistDirectory = "C:\projects\dist"

Get-ChildItem -Path  $DistDirectory -Recurse 
    | Select-Object -ExpandProperty FullName 
    | Sort-Object length -Descending 
    | Remove-Item -force 

Get-ChildItem –Path $SourceDirectory -Recurse 
    | Move-Item -Destination $DistDirectory
