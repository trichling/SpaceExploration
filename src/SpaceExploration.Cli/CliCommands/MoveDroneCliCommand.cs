using CliFx.Attributes;
using CliFx.Infrastructure;

using SpaceExploration.Game.Contracts.Commands;

using ICommand = CliFx.ICommand;

namespace SpaceExploration.Cli.CliCommands;

[Command("drone move", Description = "Moves a drone")]
public class MoveDroneCliCommand : ICommand
{
    private readonly IEndpointInstance _endpointInstance;

    [CommandParameter(0, Name = "planetId", Description = "The unique identifier of the planet.")]
    public required Guid PlanetId { get; set; }

    [CommandParameter(1, Name = "droneId", Description = "The unique identifier of the drone.")]
    public required Guid DroneId { get; set; }

    [CommandOption("turn", 't', Description = "Turns the drone by the specified angle. If no value is passed the drone will not turn.")]
    public int? Heading { get; set; } = null;

    [CommandOption("steps", 's', Description = "The number of steps to move the drone. The default value is 0.")]
    public int Steps { get; set; } = 0;



    public MoveDroneCliCommand(IEndpointInstance endpointInstance)
    {
        _endpointInstance = endpointInstance;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (Heading is not null)
        {
            console.Output.WriteLine($"Turning drone {DroneId} on planet {PlanetId} to heading {Heading.Value}");
            await _endpointInstance.Send(new Turn(PlanetId, DroneId, Heading.Value));
        }

        for (int i = 0; i < Steps; i++)
        {
            console.Output.WriteLine($"Moving drone {DroneId} on planet {PlanetId} step {i + 1}");
            await _endpointInstance.Send(new Move(PlanetId, DroneId));
        }
    }



}