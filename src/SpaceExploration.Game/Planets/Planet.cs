using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Messages;

namespace SpaceExploration.Game.Planets;

public class Planet : Saga<PlanetData>
     , IAmStartedByMessages<CreatePlanet>
     , IHandleMessages<DropDrone>
     , IHandleMessages<ScanEnvironment>
     , IHandleMessages<Shot>
     , IHandleMessages<Turn>
     , IHandleMessages<Move>
{
    private readonly ILogger<Planet> _logger;

    public Planet(ILogger<Planet> logger)
    {
        _logger = logger;
    }


    public Task Handle(CreatePlanet message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;

        return Task.CompletedTask;
    }

    public Task Handle(DropDrone message, IMessageHandlerContext context)
    {
        // validate that id is unique
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

    public Task Handle(Shot message, IMessageHandlerContext context)
    {
        if (!Data.SensorReadings.ContainsKey(message.DroneId))
        {
            return Task.CompletedTask;
        }

        var reading = Data.SensorReadings[message.DroneId].Find(sr => sr.ReadingId == message.TargetId);
        Data.SensorReadings.Remove(message.DroneId);

        if (reading is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation("Drone {0} shot at drone {1}", message.DroneId, reading.DroneId);

        var drone = Data.Drones.Single(d => d.DroneId == reading.DroneId);
        drone.Health--;

        if (drone.Health <= 0)
        {
            _logger.LogInformation("Drone {0} destroyed", drone.DroneId);
            Data.Drones.Remove(drone);
        }

        return Task.CompletedTask;
    }

    public Task Handle(Turn message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);
        drone.Heading = drone.Heading with { Degrees = (drone.Heading.Degrees + message.Angle) % 360 };

        return Task.CompletedTask;
    }

    public Task Handle(Move message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);
        
        var x = drone.Position.X + Math.Cos(drone.Heading.Degrees) * drone.MoveDistance;
        var y = drone.Position.Y + Math.Sin(drone.Heading.Degrees) * drone.MoveDistance;

        drone.Position = drone.Position with { X = x, Y = y };

        return Task.CompletedTask;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<CreatePlanet>(msg => msg.PlanetId)
            .ToMessage<DropDrone>(msg => msg.PlanetId)
            .ToMessage<ScanEnvironment>(msg => msg.PlanetId)
            .ToMessage<Shot>(msg => msg.PlanetId)
            .ToMessage<Turn>(msg => msg.PlanetId)
            .ToMessage<Move>(msg => msg.PlanetId);
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
    public readonly double MoveDistance = 0.1;

    public int Health { get; set; } = 10;

    public Coordinate Position { get; set; } = new Coordinate(new Random().NextDouble(), new Random().NextDouble());

    public Angle Heading { get; set; } = new Angle(new Random().Next(0, 360));
}

public record SensorReading(Guid ReadingId, Guid DroneId, double Distance, double Angle);