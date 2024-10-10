using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Planets.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("planet create", Description = "Creates a new planet.")]
public class CreatePlanetCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandOption("planetId", 'p', Description = "The unique identifier of the planet.")]
    public Guid PlanetId { get; set; }

    [CommandOption("planetName", 'n', Description = "The name of the planet.")]
    public string PlanetName { get; set; }

    public CreatePlanetCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (PlanetId == Guid.Empty)
        {
            PlanetId = Guid.NewGuid();
        }

        if (string.IsNullOrWhiteSpace(PlanetName))
        {
            PlanetName = PlanetId.ToString();
        }

        console.Output.WriteLine($"Creating planet '{PlanetName}' with ID '{PlanetId}'...");

        var createPlanet = new CreatePlanet(PlanetId, PlanetName);
        await _endpointInstance.Send(createPlanet);
    }
}