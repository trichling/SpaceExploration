using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;

namespace SpaceExploration.Player;

public class Player : BackgroundService
{
    public static Guid PlanetId = new Guid("4e6ab55f-31f3-4921-bc36-40894a2a908e");
    public static Guid Drone1Id = new Guid("9f3a9498-3f94-4b0b-bd12-10a29dc92089");
    public static Guid Drone1Signature = new Guid("f6336b3b-8b3f-4450-85e7-53beeb2b2988");
    private readonly ILogger<Player> _logger;
    private readonly IMessageSession _messageSession;

    public Player(ILogger<Player> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Hit enter if drone was dropped");
        Console.ReadLine();

        await _messageSession.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
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
            await context.Send(new Shot(Player.PlanetId, Player.Drone1Id, firstReading.ReadingId));
        }
        else
        {
            await context.Send(new Move(Player.PlanetId, Player.Drone1Id));
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
    IHandleMessages<DroneDestroyed>,
    IHandleMessages<MoveResult>,
    IHandleMessages<TurnResult>
{
    private readonly ILogger<ShotResultHandler> _logger;

    public ShotResultHandler(ILogger<ShotResultHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(DroneHit message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone hit: {0}", message.TargetDroneSignature);
        await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
    }

    public async Task Handle(DroneMissed message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone missed: {0}", message.DroneSignature);
        if (message.DroneSignature.Equals(Player.Drone1Id))
            await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
    }

    public async Task Handle(DroneDestroyed message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone destroyed: {0}", message.DroneSignature);

        await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
    }

    public async Task Handle(MoveResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone moved: {0}", message.DroneId);
        // 30 percent chance to issue a turn command
        Random random = new Random();
        if (random.NextDouble() <= 0.3)
        {
            await context.Send(new Turn(Player.PlanetId, Player.Drone1Id, 5)); // Example turn command
        }
        else
        {
            await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
        }
    }

    public async Task Handle(TurnResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Drone turned: {0}", message.DroneId);
        // 30 percent chance to issue a move command
        Random random = new Random();
        if (random.NextDouble() <= 0.3)
        {
            await context.Send(new Move(Player.PlanetId, Player.Drone1Id)); // Example turn command
        }
        else
        {
            await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
        }
    }
}
