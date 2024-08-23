using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Events;
using SpaceExploration.Game.Contracts.Messages;

namespace SpaceExploration.TestClient;

public class Player1 : BackgroundService
{
    public static Guid PlanetId = Guid.NewGuid(); // new Guid("699BBD37-1719-4942-AC03-5917FF851BA4");
    public static Guid Player1Id = Guid.NewGuid();
    public static Guid Drone1Id = Guid.NewGuid();
    private readonly ILogger<Player1> _logger;
    private readonly IMessageSession _messageSession;

    public Player1(ILogger<Player1> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageSession.Send(new CreatePlanet(PlanetId, "Earth"));

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public class PlanetCreatedHandler : IHandleMessages<PlanetCreated>
{
    public async Task Handle(PlanetCreated message, IMessageHandlerContext context)
    {
        if (message.PlanetId != Player1.PlanetId)
        {
            return;
        }

        await context.Send(new DropDrone(Player1.Drone1Id, Player1.PlanetId));

        for (var i = 0; i < 90; i++)
        {
            await context.Send(new DropDrone(Guid.NewGuid(), Player1.PlanetId));
        }

    }
}	

public class DroneDroppedHandler : IHandleMessages<DroneDropped>
{

    public async Task Handle(DroneDropped message, IMessageHandlerContext context)
    {
        Console.WriteLine("Drones dropped: {0}", message.OverallDroneCount);

        if (message.DroneId == Player1.Drone1Id)
        {
            Console.WriteLine("Player 1 Drone dropped: {0}", message.DroneId);
            await context.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));
        }
    }
}

public class ScanResultHandler : IHandleMessages<ScanEnvironmentResult>
{
    private readonly ILogger<ScanResultHandler> _logger;

    public ScanResultHandler(ILogger<ScanResultHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ScanEnvironmentResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Scan result received: {0}", message.SensorReadings.Count);

        if (message.SensorReadings.Count > 0)
        {
            var firstReading = message.SensorReadings.First();
            await context.Send(new Shot(Player1.PlanetId, Player1.Drone1Id, firstReading.ReadingId));
        }
        else
        {
            await context.Send(new Turn(Player1.PlanetId, Player1.Drone1Id, new Random().Next(0, 360)));
            await context.Send(new Move(Player1.PlanetId, Player1.Drone1Id));
            await context.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));
        }
    }
}

public class LocatePositionHandler : IHandleMessages<LocatePositionResult>
{
    public Task Handle(LocatePositionResult message, IMessageHandlerContext context)
    {
        Console.WriteLine("Position located: {0}, {1}", message.PositionX, message.PositionY);

        return Task.CompletedTask;
    }
}

public class ShotResultHandler : IHandleMessages<DroneHit>,
    IHandleMessages<DroneMissed>,
    IHandleMessages<DroneDestroyed>
{
    private readonly ILogger<ShotResultHandler> _logger;

    public ShotResultHandler(ILogger<ShotResultHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(DroneHit message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone hit: {0}", message.TargetDroneId);

        await context.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));
    }

    public async Task Handle(DroneMissed message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone missed: {0}", message.DroneId);

        await context.Send(new Move(Player1.PlanetId, Player1.Drone1Id));
        await context.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));
    }

    public async Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone destroyed: {0}", message.DroneId);

        await context.Send(new Move(Player1.PlanetId, Player1.Drone1Id));
        await context.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));
    }
}