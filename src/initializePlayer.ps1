param (
    [Parameter(Mandatory)] [string] $EndpointName,
    [Parameter(Mandatory)] [string] $PersistenceConnectionString,
    [Parameter(Mandatory)] [string] $TransportConnectionString,
    [Parameter(Mandatory)] [string] $PlanetId,
    [Parameter(Mandatory)] [string] $PlayerId
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

Set-Location -Path .\SpaceExploration.Player

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Persistence" "$PersistenceConnectionString"
dotnet user-secrets set "ConnectionStrings:Transport" "$TransportConnectionString"


(Get-Content "Program.cs") |
        Foreach-Object { $_ -replace "<EndointName>", $EndpointName } |
        Set-Content "Program.cs"

(Get-Content "Player.cs") |
        Foreach-Object { $_ -replace "<PlayerId>", $PlayerId } |
        Foreach-Object { $_ -replace "<PlanetId>", $PlanetId } |
        Set-Content "Player.cs"