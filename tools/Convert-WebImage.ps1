#Requires -Version 7.2

$ToolVersion = "libwebp-1.3.2-mac-arm64"
$ToolFile = "libwebp.tar.gz"

if (-Not (Test-Path -Path $ToolFile))
{
    $SourceUrl = "https://storage.googleapis.com/downloads.webmproject.org/releases/webp/$ToolVersion.tar.gz"
    Invoke-WebRequest -Uri $SourceUrl -OutFile $ToolFile
}
else
{
    Write-Host "Already downloaded"
}

$ConversionToolBasePath = "bin/$ToolVersion"
if (-Not (Test-Path -Path $ConversionToolBasePath))
{
    New-Item -ItemType Directory -Path "bin"
    tar xvfz $ToolFile -C "bin"
}
else
{
    Write-Host "Already extracted"
}

$ConversionToolPath = "$ConversionToolBasePath/bin/cwebp"
$Quality = 80

$ScriptRoot = $PSScriptRoot
$RepoRoot = Split-Path $ScriptRoot -Parent

$Images = Get-ChildItem -Recurse -Path "$RepoRoot/_site/assets/images/posts" -Filter *.png
$Images += Get-ChildItem -Recurse -Path "$RepoRoot/_site/assets/images/posts" -Filter *.jpeg
$Images += Get-ChildItem -Recurse -Path "$RepoRoot/_site/assets/images" -Filter social_preview.png
foreach ($image in $Images)
{
    $ConvertedImageName = $image.Name + ".webp"
    $ConvertedImageResultFullPath = [IO.Path]::Combine($image.DirectoryName, $ConvertedImageName)
    &$ConversionToolPath -q $Quality $image.FullName -o $ConvertedImageResultFullPath
}