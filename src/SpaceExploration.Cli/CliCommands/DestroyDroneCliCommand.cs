using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Planets.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("drone destroy", Description = "Destroys one or more drones on a planet.")]
public class DestroyDroneCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandParameter(0, Name = "planetId", Description = "The unique identifier of the planet.")]
    public required Guid PlanetId { get; set; }

    [CommandOption("droneId", 'd', Description = "The unique identifier of the drone. A random guid will be generated if not provided.")]
    public Guid? DroneId { get; set; }

    [CommandOption("droneType", 't', Description = "The type of the drone of the drone. The default value is an 'Default'.")]
    public string DroneType { get; set; }


    public DestroyDroneCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (DroneId.HasValue)
        {
            await console.Output.WriteLineAsync($"Destroying drone with ID '{DroneId}' on planet '{PlanetId}'...");
            await _endpointInstance.Send(new DestroyDrone(PlanetId, DroneId.Value));
        }

        if (!string.IsNullOrEmpty(DroneType))
        {
            await console.Output.WriteLineAsync($"Destroying drones of type '{DroneType}' on planet '{PlanetId}'...");
            await _endpointInstance.Send(new DestroyDronesOfType(PlanetId, DroneType));
        }
    }

}