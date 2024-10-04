using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("drone drop", Description = "Drops one or more drones on a planet.")]
public class DropDroneCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandParameter(0, Name = "planetId", Description = "The unique identifier of the planet.")]
    public required Guid PlanetId { get; set; }

    [CommandOption("numberOfDrones", 'i', Description = "The number of drones to drop on the planet. The default value is 1. If a value greater than 1 is provided, the droneId, droneType, and droneName options are ignored.")]
    public int NumberOfDrones { get; set; } = 1;

    [CommandOption("droneId", 'd', Description = "The unique identifier of the drone. A random guid will be generated if not provided.")]
    public Guid? DroneId { get; set; }

    [CommandOption("droneType", 't', Description = "The type of the drone of the drone. The default value is an 'Default'.")]
    public string DroneType { get; set; }

    [CommandOption("droneName", 'n', Description = "The name of the drone. If no name is passed, the droneId is used.")]
    public string DroneName { get; set; }

    public DropDroneCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (NumberOfDrones == 1)
        {
            await DropSingleDrone(console);
        }
        else
        {
            await DropMultipleDrones(console);
        }


    }



    private async Task DropSingleDrone(IConsole console)
    {
        if (!DroneId.HasValue)
        {
            DroneId = Guid.NewGuid();
        }

        if (string.IsNullOrWhiteSpace(DroneName))
        {
            DroneName = DroneId.ToString();
        }

        if (string.IsNullOrWhiteSpace(DroneType))
        {
            DroneType = "Default";
        }

        await console.Output.WriteLineAsync($"Dropping drone '{DroneName}' with ID '{DroneId}' on planet '{PlanetId}'...");

        var dropDrone = new DropDrone(DroneId.Value, PlanetId, DroneType, DroneName);
        await _endpointInstance.Send(dropDrone);
    }

    private async Task DropMultipleDrones(IConsole console)
    {
        for (var i = 0; i < NumberOfDrones; i++)
        {
            var droneId = Guid.NewGuid();

            await console.Output.WriteLineAsync($"Dropping drone with ID '{droneId}' on planet '{PlanetId}'...");

            var dropDrone = new DropDrone(droneId, PlanetId);
            await _endpointInstance.Send(dropDrone);
        }
    }
}