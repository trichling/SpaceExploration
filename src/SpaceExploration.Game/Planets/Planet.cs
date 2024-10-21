using SpaceExploration.Game.Contracts.Planets.Commands;
using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Contracts.Planets.Messages;

namespace SpaceExploration.Game.Planets;

public class Planet : Saga<PlanetData>
     , IAmStartedByMessages<CreatePlanet>
     , IHandleMessages<CatchUp>
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

    public async Task Handle(CatchUp message, IMessageHandlerContext context)
    {
        var repsone = new CatchUpResponse(
            Data.PlanetId,
            Data.Drones.Select(d => new Contracts.Planets.Messages.Drone(
                    d.DroneSignature, d.DroneType, d.DroneName, d.Position.X, d.Position.Y, d.Heading.Degrees, d.Health
                )
            )
            .ToList());

        await context.Reply(repsone);
    }

    public async Task Handle(DropDrone message, IMessageHandlerContext context)
    {
        if (Data.Drones.Any(d => d.DroneId == message.DroneId))
        {
            return;
        }

        var drone = new Drone(message.DroneId, message.DroneSignature, message.DroneType, message.DroneName);
        Data.Drones.Add(drone);

        await context.Publish(new Contracts.Planets.Events.DroneDropped(Data.PlanetId, drone.DroneSignature, drone.DroneType, drone.DroneName, drone.Position.X, drone.Position.Y, drone.Heading.Degrees, Data.Drones.Count));
    }

    public async Task Handle(ScanEnvironment message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (drone is null)
        {
            return;
        }

        var visibleDrones = EnvironmentalScan.ScanEnvironment(drone, Data.Drones);
        var sensorReadings = visibleDrones.Select(d => new SensorReading(Guid.NewGuid(), d.Drone.DroneId, d.Drone.DroneSignature, d.Distance, d.Angle)).ToList();

        if (Data.SensorReadings.ContainsKey(message.DroneId))
        {
            Data.SensorReadings[message.DroneId] = sensorReadings;
        }
        else
        {
            Data.SensorReadings.Add(message.DroneId, sensorReadings);
        }

        await context.Reply(new ScanEnvironmentResult(message.DroneId, sensorReadings.Select(sr => new DroneReading(sr.ReadingId, sr.DroneSignature, sr.Distance, sr.Angle)).ToList()));
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
        var shootingDrone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (shootingDrone is null)
        {
            return;
        }

        if (!Data.SensorReadings.ContainsKey(message.DroneId))
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            return;
        }

        var reading = Data.SensorReadings[message.DroneId].Find(sr => sr.ReadingId == message.TargetId);

        if (reading is null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            return;
        }

        _logger.LogInformation("Drone {0} shot at drone {1}", message.DroneId, reading.DroneId);

        var targetDrone = Data.Drones.Find(d => d.DroneId == reading.DroneId);

        if (targetDrone == null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            return;
        }

        targetDrone.Health--;

        if (targetDrone.Health <= 0)
        {
            _logger.LogInformation("Drone {0} destroyed", targetDrone.DroneId);
            Data.Drones.Remove(targetDrone);

            await context.Publish(new DroneDestroyed(Data.PlanetId, targetDrone.DroneSignature, shootingDrone.DroneSignature));
            return;
        }

        Data.SensorReadings.Remove(message.DroneId);
        await context.Publish(new DroneHit(Data.PlanetId, targetDrone.DroneSignature, shootingDrone.DroneSignature, targetDrone.Health));
    }

    public async Task Handle(Turn message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);
        drone.Heading = drone.Heading with { Degrees = (drone.Heading.Degrees + message.Angle) % 360 };

        await context.Publish(new DroneTurned(Data.PlanetId, drone.DroneSignature, drone.Heading.Degrees, drone.Position.X, drone.Position.Y));
        await context.Reply(new TurnResult(Data.PlanetId, drone.DroneId));
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

        await context.Publish(new DroneMoved(Data.PlanetId, drone.DroneSignature, drone.Position.X, drone.Position.Y, drone.Heading.Degrees));
        await context.Reply(new MoveResult(Data.PlanetId, drone.DroneId));
    }



    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<CreatePlanet>(msg => msg.PlanetId)
            .ToMessage<CatchUp>(msg => msg.PlanetId)
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

public record SensorReading(Guid ReadingId, Guid DroneId, Guid DroneSignature, double Distance, double Angle);