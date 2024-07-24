#!/usr/bin/env pwsh

Push-Location "$PSScriptRoot/../.."

Add-Type -Path "artifacts/bin/Server/Debug/net8.0/DarkPatterns.OneTimePassword.Server.dll"

[DarkPatterns.OneTimePassword.Auth.ApiKeyTools]::ToApiKey([System.Guid]::NewGuid(), [System.Guid]::NewGuid())
