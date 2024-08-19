using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Messages;

namespace SpaceExploration.Game.Planets;

public class Planet : Saga<PlanetData>
     , IAmStartedByMessages<CreatePlanet>
     , IHandleMessages<DropDrone>
     , IHandleMessages<ScanEnvironment>
{
    public Task Handle(CreatePlanet message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        return Task.CompletedTask;
    }

    public Task Handle(DropDrone message, IMessageHandlerContext context)
    {
        Data.Drones.Add(new Drone(message.DroneId));   

        return Task.CompletedTask;
    }

    public async Task Handle(ScanEnvironment message, IMessageHandlerContext context)
    {
        var visibleDrones = EnvironmentalScan.ScanEnvironment(Data.Drones.Single(d => d.DroneId == message.DroneId), Data.Drones);
        var sensorReadings = visibleDrones.Select(d => new SensorReading(Guid.NewGuid(), d.Drone.DroneId, d.Distance, d.Angle)).ToList();

        if (Data.SensorReadings.ContainsKey(message.DroneId))
        {
            Data.SensorReadings[message.DroneId] = sensorReadings;
        }
        else
        {
            Data.SensorReadings.Add(message.DroneId, sensorReadings);
        }

        await context.Reply(new ScanEnvironmentResult(message.DroneId, sensorReadings.Select(sr => new DroneReading(sr.ReadingId, sr.Distance, sr.Angle)).ToList()));
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<CreatePlanet>(msg => msg.PlanetId)
            .ToMessage<DropDrone>(msg => msg.PlanetId)
            .ToMessage<ScanEnvironment>(msg => msg.PlanetId);
    }
}

public class PlanetData : ContainSagaData
{
    public Guid PlanetId { get; set; }
    public List<Drone> Drones { get; set; } = new List<Drone>();

    public Dictionary<Guid, List<SensorReading>> SensorReadings { get; set; } = new Dictionary<Guid, List<SensorReading>>();


}


public record Coordinate(double X, double Y);

public record Angle(int Degrees);

public class Drone(Guid droneId)
{

    public Guid DroneId { get; set; } = droneId;

    public readonly int FieldOvView = 45;
    public readonly double ScanDistance = 0.1;

    public Coordinate Position { get; set; } = new Coordinate(new Random().NextDouble(), new Random().NextDouble());

    public Angle Heading { get; set; } = new Angle(new Random().Next(0, 360));
}

public record SensorReading(Guid ReadingId, Guid DroneId, double Distance, double Angle);