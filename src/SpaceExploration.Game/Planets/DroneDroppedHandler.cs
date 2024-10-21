
using SpaceExploration.Game.Contracts.Planets.Events;

namespace SpaceExploration.Game;

public class DroneDroppeHandler : IHandleMessages<DroneDropped>
{
    public async Task Handle(DroneDropped message, IMessageHandlerContext context)
    {
        await context.Publish(new Contracts.Drones.Events.DroneDropped(message.PlanetId, message.DroneSignature, message.DroneType, message.DroneName, message.OverallDroneCount));
    }
}
