---
title: Automating .NET User Secrets with PowerShell
description: Manage secret configuration for dotnet projects using PowerShell
tags:
  - powershell
commentid: '1304' 
---
For dotnet developers Microsoft created a dev-time convenience to handle secret values.
No need for a shared infrastructure dependency, and no need for storing secrets in the repository.
Add a helper script on top of it, and your dev shop will have a convenient way to get up and running.

UserSecrets are stored in a (unencrypted) json file. Depending on platform they are in either `%APPDATA%\Microsoft\UserSecrets` or `~/.microsoft/usersecrets`.

## Simple Variant

The most simple variant is demonstrated by this PowerShell script.
Please note, in real-world scenario we would parameterize the script to allow entry of the secrets.
For simplicity we use random GUIDs here.

```ps
#Requires -Version 7.4

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path $PSScriptRoot -Parent

$Secret1 = [System.Guid]::NewGuid().ToString()
$Secret2 = [System.Guid]::NewGuid().ToString()

$APP1_FOLDER = Join-Path -Path $RepoRoot -ChildPath "src/App1"
Push-Location $APP1_FOLDER
Write-Host "Setting secrets for $APP1_FOLDER"
dotnet user-secrets clear
dotnet user-secrets set "App1:ConnectionStrings:Secret1" $Secret1
dotnet user-secrets set "App1:ConnectionStrings:Secret2" $Secret2
Pop-Location
```

This produces one of two possible outputs. 

Failure output:
```output
Could not find the global property 'UserSecretsId' in MSBuild project '/Secrets/src/App1/App1.csproj'. Ensure this property is set in the project or use the '--id' command line option.
```

Success output:
```output
Setting secrets for /Users/maxhamulyak/Dev/BlogTopics/_posts/Secrets/src/App1
Successfully saved App1:ConnectionStrings:Secret1 to the secret store.
Successfully saved App1:ConnectionStrings:Secret2 to the secret store.
```

To be able to set secrets on a project level, the property UserSecretsId needs to be set.
For example `<UserSecretsId>[ANY-STRING-VALUE]</UserSecretsId>`.
Doing this for a large solution, project-by-project can be a hassle. So I prefer creating a Directory.Build.Targets file.
We can then ensure each project either has an explicit or implicit secret id.

```xml
<Project>
  <PropertyGroup>
      <UserSecretsId Condition="'$(UserSecretsId)' == ''">$(MSBuildProjectName)-dev-secrets</UserSecretsId>
  </PropertyGroup>
</Project>
```


## Multiple secrets at once

The first version of the script works, but calling a command line for a ton of secrets feels ineffective.
Luckily, we can also bulk import by using a JSON file.
The trick here is, create the object in PowerShell, convert it to JSON and run the `dotnet user-secrets ` command.

```
#Requires -Version 7.4

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path $PSScriptRoot -Parent

$Secret1 = [System.Guid]::NewGuid().ToString()
$Secret2 = [System.Guid]::NewGuid().ToString()

$APP1_FOLDER = Join-Path -Path $RepoRoot -ChildPath "src/App1"
Push-Location $APP1_FOLDER
Write-Host "Setting secrets for $APP1_FOLDER"
dotnet user-secrets clear
$App1Config = @{
    App1 = @{
        ConnectionStrings = @{
            Secret1 = $Secret1
            Secret2 = $Secret2
        }
    }
}
$App1Config | ConvertTo-Json -Depth 5 | dotnet user-secrets set
Pop-Location
```

## Multiple project same secret

In Azure, I usually have a simple KeyVault instance per resource group, and not a KeyVault per executable.
That's why I picked up the habit of prefixing / scoping secrets.
In this case I have been prefixing them with `App1`.

Instead of setting secrets per project, we can also have a shared secret-id for a solution.
This would mimic the behavior when deployed for production.

```ps
#Requires -Version 7.4

$ErrorActionPreference = "Stop"

$Secret1 = [System.Guid]::NewGuid().ToString()
$Secret2 = [System.Guid]::NewGuid().ToString()


$App1Config = @{
    ConnectionStrings = @{
            Secret1 = $Secret1
            Secret2 = $Secret2
    }
}

$Config = @{
    App1 = $App1Config
}

$SecretId = "Project-5ea2d981-14f7-4487-93c0-d4b7e3dbebf1"
dotnet user-secrets clear --id $SecretId
$Config | ConvertTo-Json -Depth 10 | dotnet user-secrets set --id $SecretId
```

## Closing thoughts

This concludes my post.
TODO..

## References

- [UserSecrets Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)