using SpaceExploration.Game.Contracts.Planets.Commands;
using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Contracts.Planets.Messages;
using SpaceExploration.Game.DroneTypes;
using SpaceExploration.Game.PlayerRessurections;

namespace SpaceExploration.Game.Planets;

public class Planet : Saga<PlanetData>
     , IAmStartedByMessages<CreatePlanet>
     , IHandleMessages<DestroyPlanet>
     , IHandleMessages<ScanPlanet>
     , IHandleMessages<DropDrone>
     , IHandleMessages<DestroyDrone>
     , IHandleMessages<DestroyDronesOfType>
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
        await context.SendLocal(new Create_1_0_10_0_Scoring(message.PlanetId));
        await context.Publish(new PlanetCreated(message.PlanetId, message.Name));
    }

    public async Task Handle(DestroyPlanet message, IMessageHandlerContext context)
    {
        foreach (var drone in Data.Drones)
        {
            await context.Publish(new DroneDestroyed(Data.PlanetId, drone.DroneSignature, Data.PlanetId));
        }

        Data.Drones.Clear();

        MarkAsComplete();
    }

    public async Task Handle(ScanPlanet message, IMessageHandlerContext context)
    {
        var repsone = new ScanPlanetResponse(
            Data.PlanetId,
            Data.Drones.Select(d => new Contracts.Planets.Messages.Drone(
                    d.DroneSignature, d.DroneType, d.DroneName, d.Position.X, d.Position.Y, d.Heading.Degrees, d.Health
                )
            )
            .ToList());

        await context.Reply(repsone);
    }

    public async Task Handle(DestroyDrone message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (drone is null)
        {
            return;
        }

        Data.Drones.Remove(drone);
        await context.Publish(new DroneDestroyed(Data.PlanetId, drone.DroneSignature, Data.PlanetId));
    }

    public async Task Handle(DestroyDronesOfType message, IMessageHandlerContext context)
    {
        var drones = Data.Drones.Where(d => d.DroneType == message.DroneType).ToList();

        foreach (var drone in drones)
        {
            Data.Drones.Remove(drone);
            await context.Publish(new DroneDestroyed(Data.PlanetId, drone.DroneSignature, Data.PlanetId));
        }
    }

    public async Task Handle(DropDrone message, IMessageHandlerContext context)
    {
        if (Data.Drones.Any(d => d.DroneId == message.DroneId))
        {
            return;
        }

        var drone = new Drone(message.DroneId, message.DroneSignature, message.DroneType, message.DroneName);
        Data.Drones.Add(drone);

        await (drone.DroneType switch
        {
            "Player" => context.SendLocal(new EnablePlayerRessurection(Data.PlanetId, drone.DroneId, drone.DroneSignature, drone.DroneName)),
            "Turret" => context.SendLocal(new BecomeATurret(Data.PlanetId, drone.DroneId, drone.DroneSignature)),
            //"Scout" => context.Send(new BecomeAScout(Data.PlanetId, drone.DroneSignature)),
            _ => Task.CompletedTask
        });

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
        var sensorReadings = visibleDrones.Select(d => new SensorReading(Guid.NewGuid(), d.Drone.DroneId, d.Drone.DroneSignature, d.Distance, d.RelativeHeading, d.Drone.Heading.Degrees)).ToList();

        if (Data.SensorReadings.ContainsKey(message.DroneId))
        {
            Data.SensorReadings[message.DroneId] = sensorReadings;
        }
        else
        {
            Data.SensorReadings.Add(message.DroneId, sensorReadings);
        }

        await context.Reply(new ScanEnvironmentResult(drone.DroneId, drone.DroneSignature, sensorReadings.Select(sr => new DroneReading(sr.ReadingId, sr.DroneSignature, sr.Distance, sr.RelativeHeading, sr.Heading)).ToList()));
    }

    public async Task Handle(LocatePosition message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (drone is null)
        {
            return;
        }

        await context.Reply(new LocatePositionResult(message.DroneId, drone.DroneSignature, drone.Heading.Degrees, drone.Position.X, drone.Position.Y));
    }


    public async Task Handle(Shot message, IMessageHandlerContext context)
    {
        var shootingDrone = Data.Drones.Find(d => d.DroneId == message.DroneId);

        if (shootingDrone is null)
        {
            await context.Reply(new ShotResult(message.DroneId, ShotOutcome.ShootingDroneNotFound));
            return;
        }

        if (!Data.SensorReadings.ContainsKey(message.DroneId))
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            await context.Reply(new ShotResult(message.DroneId, ShotOutcome.NoSensorReadings));
            return;
        }

        var reading = Data.SensorReadings[message.DroneId].Find(sr => sr.ReadingId == message.TargetId);

        if (reading is null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            await context.Reply(new ShotResult(message.DroneId, ShotOutcome.NoSensorReadingForTargetId, message.TargetId));
            return;
        }

        _logger.LogInformation("Drone {0} shot at drone {1}", message.DroneId, reading.DroneId);

        var targetDrone = Data.Drones.Find(d => d.DroneId == reading.DroneId);

        if (targetDrone == null)
        {
            await context.Publish(new DroneMissed(Data.PlanetId, shootingDrone.DroneSignature));
            await context.Reply(new ShotResult(message.DroneId, ShotOutcome.TargetDroneNotFound, message.TargetId, reading.DroneSignature));
            return;
        }

        targetDrone.Health--;

        if (targetDrone.Health <= 0)
        {
            _logger.LogInformation("Drone {0} destroyed", targetDrone.DroneId);
            Data.Drones.Remove(targetDrone);

            await context.Publish(new DroneDestroyed(Data.PlanetId, targetDrone.DroneSignature, shootingDrone.DroneSignature));
            await context.Reply(new ShotResult(message.DroneId, ShotOutcome.Destroyed, message.TargetId, reading.DroneSignature));

            return;
        }

        Data.SensorReadings.Remove(message.DroneId);
        await context.Publish(new DroneHit(Data.PlanetId, targetDrone.DroneSignature, shootingDrone.DroneSignature, targetDrone.Health));
        await context.Reply(new ShotResult(message.DroneId, ShotOutcome.Hit, message.TargetId, reading.DroneSignature));
    }

    public async Task Handle(Turn message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);
        drone.Heading = drone.Heading with { Degrees = (drone.Heading.Degrees + message.Angle) % 360 };

        await context.Publish(new DroneTurned(Data.PlanetId, drone.DroneSignature, drone.Heading.Degrees, drone.Position.X, drone.Position.Y));
        await context.Reply(new TurnResult(drone.DroneId, drone.DroneSignature, drone.Heading.Degrees));
    }

    public async Task Handle(Move message, IMessageHandlerContext context)
    {
        var drone = Data.Drones.Single(d => d.DroneId == message.DroneId);

        var deltaX = Math.Cos(drone.Heading.Radians) * drone.MoveDistance;
        var deltaY = Math.Sin(drone.Heading.Radians) * drone.MoveDistance;

        var x = drone.Position.X + deltaX;
        var y = drone.Position.Y + deltaY;

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
        await context.Reply(new MoveResult(Data.PlanetId, drone.DroneId, deltaX, deltaY));
    }



    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlanetData> mapper)
    {
        mapper.MapSaga(saga => saga.PlanetId)
            .ToMessage<CreatePlanet>(msg => msg.PlanetId)
            .ToMessage<DestroyPlanet>(msg => msg.PlanetId)
            .ToMessage<ScanPlanet>(msg => msg.PlanetId)
            .ToMessage<DropDrone>(msg => msg.PlanetId)
            .ToMessage<DestroyDrone>(msg => msg.PlanetId)
            .ToMessage<DestroyDronesOfType>(msg => msg.PlanetId)
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

public record SensorReading(Guid ReadingId, Guid DroneId, Guid DroneSignature, double Distance, double RelativeHeading, double Heading);