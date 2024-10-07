using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;

namespace SpaceExploration.TestClient;

public class Player1 : BackgroundService
{
    public static Guid PlanetId = new Guid("297a732c-1327-43a2-8000-786ed957e68d");
    public static Guid Drone1Id = new Guid("4bd29f7e-5626-4f7f-a82c-152026e1cc55");
    private readonly ILogger<Player1> _logger;
    private readonly IMessageSession _messageSession;

    public Player1(ILogger<Player1> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Hit enter if drone was dropped");
        Console.ReadLine();

        await _messageSession.Send(new ScanEnvironment(Player1.Drone1Id, Player1.PlanetId));

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
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