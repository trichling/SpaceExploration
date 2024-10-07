using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Planets.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("create-planet", Description = "Creates a new planet.")]
public class CreatePlanetCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandParameter(0, Name = "planetId", Description = "The unique identifier of the planet.")]
    public required Guid PlanetId { get; set; }

    [CommandParameter(1, Name = "planetName", Description = "The name of the planet.")]
    public required string PlanetName { get; set; }

    public CreatePlanetCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Creating planet '{PlanetName}' with ID '{PlanetId}'...");

        var createPlanet = new CreatePlanet(PlanetId, PlanetName);
        await _endpointInstance.Send(createPlanet);
    }
}