param (
    [Parameter(Mandatory)] [string] $PersistenceConnectionString,
    [Parameter(Mandatory)] [string] $TransportConnectionString
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

Set-Location -Path .\SpaceExploration.AppHost

dotnet user-secrets init
dotnet user-secrets set "Parameters:PersistenceConnectionString" "$PersistenceConnectionString"
dotnet user-secrets set "Parameters:TransportConnectionString" "$TransportConnectionString"

Set-Location -Path $prevPwd
