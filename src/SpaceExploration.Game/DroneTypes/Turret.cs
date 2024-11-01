
using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;

namespace SpaceExploration.Game.DroneTypes;

public class Turret : Saga<TurretData>,
    IAmStartedByMessages<BecomeATurret>,
    IHandleMessages<ScanEnvironmentResult>,
    IHandleMessages<TurnResult>,
    IHandleMessages<DroneHit>,
    IHandleMessages<DroneMissed>,
    IHandleMessages<DroneDestroyed>
{
    public async Task Handle(BecomeATurret message, IMessageHandlerContext context)
    {
        Data.DroneId = message.DroneId;
        Data.DroneSiganture = message.DroneSiganture;
        Data.PlanetId = message.PlanetId;
        await context.SendLocal(new ScanEnvironment(Data.DroneId, Data.PlanetId));
    }

    public async Task Handle(ScanEnvironmentResult message, IMessageHandlerContext context)
    {
        if (message.SensorReadings.Any())
        {
            await context.SendLocal(new Shot(Data.PlanetId, Data.DroneId, message.SensorReadings.First().ReadingId));
        }
        else
        {
            await context.SendLocal(new Turn(Data.PlanetId, Data.DroneId, 20));
        }
    }

    public async Task Handle(TurnResult message, IMessageHandlerContext context)
    {
        await context.SendLocal(new ScanEnvironment(Data.DroneId, Data.PlanetId));
    }

    public async Task Handle(DroneHit message, IMessageHandlerContext context)
    {
        await context.SendLocal(new ScanEnvironment(Data.DroneId, Data.PlanetId));
    }

    public async Task Handle(DroneMissed message, IMessageHandlerContext context)
    {
        await context.SendLocal(new ScanEnvironment(Data.DroneId, Data.PlanetId));
    }

    public async Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        await context.SendLocal(new ScanEnvironment(Data.DroneId, Data.PlanetId));
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TurretData> mapper)
    {
        mapper.MapSaga(d => d.DroneSiganture)
            .ToMessage<BecomeATurret>(m => m.DroneSiganture)
            .ToMessage<ScanEnvironmentResult>(m => m.DroneSignature)
            .ToMessage<TurnResult>(m => m.DroneSignature)
            .ToMessage<DroneHit>(m => m.ShootingDroneSignature)
            .ToMessage<DroneMissed>(m => m.DroneSignature)
            .ToMessage<DroneDestroyed>(m => m.DestroyedByDroneSignature);
    }
}

public record BecomeATurret(Guid PlanetId, Guid DroneId, Guid DroneSiganture);

public class TurretData : ContainSagaData
{
    public Guid PlanetId { get; set; }

    public Guid DroneId { get; set; }

    public Guid DroneSiganture { get; set; }

}