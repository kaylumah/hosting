#Requires -Version 7.2

param (
    [string] $Message,
    [string] $Caption = $Message
)

$Yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Yes";
$No = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "No";
$Choices = [System.Management.Automation.Host.ChoiceDescription[]]($Yes, $No);
$Answer = $Host.UI.PromptForChoice($Caption, $Message, $Choices, 0)

switch ($Answer) {
    0 { return $true; break }
    1 { return $false; break }
}