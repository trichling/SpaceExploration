param (
    [Parameter(Mandatory)] [string] $EndpointName,
    [Parameter(Mandatory)] [string] $PersistenceConnectionString,
    [Parameter(Mandatory)] [string] $TransportConnectionString,
    [Parameter(Mandatory)] [string] $PlanetId,
    [Parameter(Mandatory)] [string] $PlayerId,
    [Parameter(Mandatory)] [string] $PlayerSignature
)

$prevPwd = $PWD; Set-Location -ErrorAction Stop -LiteralPath $PSScriptRoot

Set-Location -Path .\SpaceExploration.Player

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Persistence" "$PersistenceConnectionString"
dotnet user-secrets set "ConnectionStrings:Transport" "$TransportConnectionString"


(Get-Content "Program.cs.template") |
        Foreach-Object { $_ -replace "<EndpointName>", $EndpointName } |
        Set-Content "Program.cs"

(Get-Content "Player.cs.template") |
        Foreach-Object { $_ -replace "<PlanetId>", $PlanetId } |
        Foreach-Object { $_ -replace "<PlayerId>", $PlayerId } |
        Foreach-Object { $_ -replace "<PlanetSignature>", $PlayerSignature } |
        Set-Content "Player.cs"

Set-Location -Path $prevPwd