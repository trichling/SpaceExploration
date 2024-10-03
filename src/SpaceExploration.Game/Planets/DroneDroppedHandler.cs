using SpaceExploration.Game.Events;

namespace SpaceExploration.Game;

public class DroneDroppeHandler : IHandleMessages<DroneDropped>
{
    public async Task Handle(DroneDropped message, IMessageHandlerContext context)
    {
        await context.Publish(new Contracts.Events.DroneDropped(message.PlanetId, message.DroneId, message.DroneType, message.DroneName, message.OverallDroneCount));
    }
}
