using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Events;

namespace SpaceExploration.Game.Ui;

public class PlanetEventHandler : Saga<PlanetEventHandlerData>,
    IAmStartedByMessages<PlanetCreated>,
    IAmStartedByMessages<Contracts.Planets.Events.DroneDropped>,
    IAmStartedByMessages<DroneTurned>,
    IAmStartedByMessages<DroneMoved>,
    IAmStartedByMessages<DroneHit>,
    IAmStartedByMessages<DroneDestroyed>

{
    private readonly GameStateService gameStateService;

    public PlanetEventHandler(GameStateService gameStateService)
    {
        this.gameStateService = gameStateService;
    }

    public Task Handle(PlanetCreated message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;
        return Task.CompletedTask;
    }

    public Task Handle(Contracts.Planets.Events.DroneDropped message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        gameStateService.HandleDroneDropped(message);

        return Task.CompletedTask;
    }

    public Task Handle(DroneTurned message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        gameStateService.HandleDroneTurned(message);
        return Task.CompletedTask;
    }

    public Task Handle(DroneMoved message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        gameStateService.HandleDroneMoved(message);
        return Task.CompletedTask;
    }

    public Task Handle(DroneHit message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        gameStateService.HandleDroneHit(message);
        return Task.CompletedTask;
    }

    public Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        gameStateService.HandleDroneDestroyed(message);
        return Task.CompletedTask;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetEventHandlerData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<PlanetCreated>(msg => msg.PlanetId)
            .ToMessage<Contracts.Planets.Events.DroneDropped>(msg => msg.PlanetId)
            .ToMessage<DroneTurned>(msg => msg.PlanetId)
            .ToMessage<DroneMoved>(msg => msg.PlanetId)
            .ToMessage<DroneHit>(msg => msg.PlanetId)
            .ToMessage<DroneDestroyed>(msg => msg.PlanetId)
            ;
    }
}

public class PlanetEventHandlerData : ContainSagaData
{
    public Guid PlanetId { get; set; }

}