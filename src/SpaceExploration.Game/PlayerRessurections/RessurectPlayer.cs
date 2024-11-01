
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Commands;

namespace SpaceExploration.Game.PlayerRessurections;


public class RessurectPlayer : Saga<RessurectPlayerData>,
    IAmStartedByMessages<EnablePlayerRessurection>,
    IHandleMessages<DroneDestroyed>
{
    public Task Handle(EnablePlayerRessurection message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;
        Data.DroneId = message.DroneId;
        Data.DroneNmae = message.DroneName;
        Data.DroneSignature = message.DroneSignature;

        return Task.CompletedTask;

    }

    public async Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        await context.Send(new DropDrone(Data.DroneId, Data.DroneSignature, Data.PlanetId, "Player", Data.DroneNmae));
        MarkAsComplete();
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RessurectPlayerData> mapper)
    {
        mapper.MapSaga(d => d.DroneSignature)
            .ToMessage<EnablePlayerRessurection>(m => m.DroneSignature)
            .ToMessage<DroneDestroyed>(m => m.DroneSignature);

    }
}

public record EnablePlayerRessurection(Guid PlanetId, Guid DroneId, Guid DroneSignature, string DroneName);

public class RessurectPlayerData : ContainSagaData
{

    public Guid PlanetId { get; set; }
    public Guid DroneId { get; set; }
    public Guid DroneSignature { get; set; }
    public string DroneNmae { get; set; }

}