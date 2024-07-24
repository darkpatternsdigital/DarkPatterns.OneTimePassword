#!/usr/bin/env pwsh

param(
	[Parameter()][Alias('u')][String] $user = 'postgres',
	[Parameter()][Alias('d')][String] $database = 'otp',
	[Parameter(ValueFromRemainingArguments=$true)][String[]]$Args
)

# Launches the "psql" command in the terminal with a connection to the
# docker-compose postgres db instance

docker exec -ti db-db-1 psql -U $user -d $database @Args
