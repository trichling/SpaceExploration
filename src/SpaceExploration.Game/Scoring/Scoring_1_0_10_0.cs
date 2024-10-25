

using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Planets.Commands;

namespace SpaceExploration.Game.Scoring;

public class Scoring_1_0_10_0 : Saga<SocringData>,
    IAmStartedByMessages<Create_1_0_10_0_Scoring>,
    IHandleMessages<DroneDropped>,
    IHandleMessages<DroneHit>,
    IHandleMessages<DroneDestroyed>
{

    public Task Handle(Create_1_0_10_0_Scoring message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;
        Data.Drones = new List<DroneScore>();
        return Task.CompletedTask;
    }

    public Task Handle(DroneDropped message, IMessageHandlerContext context)
    {
        Data.Drones.Add(new DroneScore
        {
            DroneSignature = message.DroneSignature,
            Score = 0
        });

        return Task.CompletedTask;
    }

    public async Task Handle(DroneHit message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.FirstOrDefault(d => d.DroneSignature == message.ShootingDroneSignature);
        if (drone == null)
        {
            drone = new DroneScore
            {
                DroneSignature = message.ShootingDroneSignature,
                Score = 0
            };
            Data.Drones.Add(drone);
        }
        drone.Score += 1;

        await context.Publish(new DroneScoreUpdated(Data.PlanetId, message.ShootingDroneSignature, drone.Score));
    }

    public async Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.FirstOrDefault(d => d.DroneSignature == message.DestroyedByDroneSignature);
        if (drone == null)
        {
            drone = new DroneScore
            {
                DroneSignature = message.DestroyedByDroneSignature,
                Score = 0
            };
            Data.Drones.Add(drone);
        }
        drone.Score += 10;

        await context.Publish(new DroneScoreUpdated(Data.PlanetId, message.DestroyedByDroneSignature, drone.Score));
    }



    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SocringData> mapper)
    {
        mapper.MapSaga(m => m.PlanetId)
            .ToMessage<Create_1_0_10_0_Scoring>(m => m.PlanetId)
            .ToMessage<DroneDropped>(m => m.PlanetId)
            .ToMessage<DroneHit>(m => m.PlanetId)
            .ToMessage<DroneDestroyed>(m => m.PlanetId);
    }
}

public class SocringData : ContainSagaData
{
    public Guid PlanetId { get; set; }

    public List<DroneScore> Drones { get; set; } = new();
}

public class DroneScore
{

    public Guid DroneSignature { get; set; }

    public int Score { get; set; }

}