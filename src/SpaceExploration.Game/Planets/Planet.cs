using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Events;
using SpaceExploration.Game.Contracts.Messages;
using SpaceExploration.Game.Events;

namespace SpaceExploration.Game.Planets;

public class Planet : Saga<PlanetData>
     , IAmStartedByMessages<CreatePlanet>
     , IHandleMessages<DropDrone>
     , IHandleMessages<ScanEnvironment>
     , IHandleMessages<LocatePosition>
     , IHandleMessages<Shot>
     , IHandleMessages<Turn>
     , IHandleMessages<Move>

{
    private readonly ILogger<Planet> _logger;

    public Planet(ILogger<Planet> logger)
    {
        _logger = logger;
    }


    public async Task Handle(CreatePlanet message, IMessageHandlerContext context)
    {
        Data.PlanetId = message.PlanetId;
        await context.Publish(new PlanetCreated(message.PlanetId, message.Name));
    }

    public async Task Handle(DropDrone message, IMessageHandlerContext context)
    {
        if (Data.Drones.Any(d => d.DroneId == message.DroneId))
        {
            return;
        }

        var drone = new Drone(message.DroneId, message.DroneType, message.DroneName);
        Data.Drones.Add(drone);

        await context.Publish(new Events.DroneDropped(Data.PlanetId, drone.DroneId, drone.DroneType, drone.DroneName, drone.Position.X, drone.Position.Y, drone.Heading.Degrees, Data.Drones.Count));
    }

    public async Task Handle(ScanEnvironment message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (drone is null)
        {
            return;
        }

        var visibleDrones = EnvironmentalScan.ScanEnvironment(drone, Data.Drones);
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

    public async Task Handle(LocatePosition message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (drone is null)
        {
            return;
        }

        await context.Reply(new LocatePositionResult(message.DroneId, drone.Heading.Degrees, drone.Position.X, drone.Position.Y));
    }


    public async Task Handle(Shot message, IMessageHandlerContext context)
    {
        if (!Data.SensorReadings.ContainsKey(message.DroneId))
        {
            await context.Publish(new DroneMissed(Data.PlanetId, message.DroneId));
            return;
        }

        var reading = Data.SensorReadings[message.DroneId].Find(sr => sr.ReadingId == message.TargetId);

        if (reading is null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, message.DroneId));
            return;
        }

        _logger.LogInformation("Drone {0} shot at drone {1}", message.DroneId, reading.DroneId);

        var targetDrone = Data.Drones.Find(d => d.DroneId == reading.DroneId);

        if (targetDrone == null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, message.DroneId));
            return;
        }

        targetDrone.Health--;

        if (targetDrone.Health <= 0)
        {
            _logger.LogInformation("Drone {0} destroyed", targetDrone.DroneId);
            Data.Drones.Remove(targetDrone);

            await context.Publish(new DroneDestroyed(Data.PlanetId, targetDrone.DroneId));
            return;
        }

        Data.SensorReadings.Remove(message.DroneId);
        await context.Publish(new DroneHit(Data.PlanetId, targetDrone.DroneId, message.DroneId, targetDrone.Health));
    }

    public async Task Handle(Turn message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);
        drone.Heading = drone.Heading with { Degrees = (drone.Heading.Degrees + message.Angle) % 360 };

        await context.Publish(new DroneTurned(Data.PlanetId, message.DroneId, drone.Heading.Degrees));
    }

    public async Task Handle(Move message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);

        var x = drone.Position.X + Math.Cos(drone.Heading.Radians) * drone.MoveDistance;
        var y = drone.Position.Y + Math.Sin(drone.Heading.Radians) * drone.MoveDistance;

        if (x > 1)
            x = x - 1;

        if (y > 1)
            y = y - 1;

        if (x < 0)
            x = 1 - Math.Abs(x);

        if (y < 0)
            y = 1 - Math.Abs(y);

        drone.Position = drone.Position with { X = x, Y = y };

        await context.Publish(new DroneMoved(Data.PlanetId, message.DroneId, drone.Position.X, drone.Position.Y));
    }


    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<CreatePlanet>(msg => msg.PlanetId)
            .ToMessage<DropDrone>(msg => msg.PlanetId)
            .ToMessage<ScanEnvironment>(msg => msg.PlanetId)
            .ToMessage<LocatePosition>(msg => msg.PlanetId)
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

public record SensorReading(Guid ReadingId, Guid DroneId, double Distance, double Angle);