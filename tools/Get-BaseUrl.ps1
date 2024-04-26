[string] $PrBuildId = $env:PR_BUILD_ID
[string] $BaseUrl = ![string]::IsNullOrEmpty($PrBuildId) ? "https://green-field-0353fee03-$PrBuildId.westeurope.1.azurestaticapps.net" : "https://kaylumah.nl"
return $BaseUrl