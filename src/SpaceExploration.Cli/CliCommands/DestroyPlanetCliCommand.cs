using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Planets.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("planet destroy", Description = "Destroys a planet.")]
public class DestroyPlaneCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandParameter(0, Name = "planetId", Description = "The unique identifier of the planet.")]
    public Guid PlanetId { get; set; }

    public DestroyPlaneCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Destroying planet with ID '{PlanetId}'...");

        var destroyPlanet = new DestroyPlanet(PlanetId);
        await _endpointInstance.Send(destroyPlanet);
    }
}