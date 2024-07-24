#!/usr/bin/env pwsh

param(
	[Parameter()][Alias('u')][String] $user = 'postgres',
	[Parameter()][Alias('d')][String] $database = 'otp',
	[Parameter(ValueFromRemainingArguments=$true)][String[]]$Args = @()
)

Push-Location "$PSScriptRoot/../.."

Add-Type -Path "artifacts/bin/Server/Debug/net8.0/DarkPatterns.OneTimePassword.Server.dll"

$appId = [System.Guid]::NewGuid()
$confgId = [System.Guid]::NewGuid()
[DarkPatterns.OneTimePassword.Auth.ApiKeyTools]::ToApiKey($appId, $confgId)

& ./eng/psql/psql.ps1 -U $user -d $database @Args -c "INSERT INTO `"Applications`" (`"ApplicationId`") VALUES ('$appId')" > $null
& ./eng/psql/psql.ps1 -U $user -d $database @Args -c "
INSERT INTO `"Configurations`" (`"ConfigurationId`", `"MaxAttemptCount`", `"ExpirationWindow`", `"IsSliding`")
VALUES ('$confgId', 3, interval'20 minutes', true)" > $null
& ./eng/psql/psql.ps1 -U $user -d $database @Args -c "
INSERT INTO `"ConfiguredApplications`" (`"ApplicationId`", `"ConfigurationId`")
VALUES ('$appId', '$confgId')" > $null
