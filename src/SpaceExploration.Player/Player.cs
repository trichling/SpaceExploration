using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Contracts.Drones.Messages;

namespace SpaceExploration.Player;

public class Player : BackgroundService
{
    public static Guid PlanetId = new Guid("822cad3c-fb93-4b31-a915-cc88000f5ab0");
    public static Guid Drone1Id = new Guid("a73e7075-31ff-4967-b625-1499ac9404aa");
    public static Guid Drone1Signature = new Guid("15a43000-db6f-42cd-b448-572328917161");

    private readonly ILogger<Player> _logger;
    private readonly IMessageSession _messageSession;

    public Player(ILogger<Player> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageSession.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
            _logger.LogInformation("Player running at: {time}", DateTimeOffset.Now);
            //     await _messageSession.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
        }
    }
}

public class ScanEnvrionmentResultHander : IHandleMessages<ScanEnvironmentResult>
{
    private readonly ILogger<ScanEnvrionmentResultHander> _logger;

    public ScanEnvrionmentResultHander(ILogger<ScanEnvrionmentResultHander> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ScanEnvironmentResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("ScanEnvironmentResult received {result}", message);
        if (message.SensorReadings.Any())
        {
            await context.Send(new Shot(Player.PlanetId, Player.Drone1Id, message.SensorReadings.First().ReadingId));
        }
        else
        {
            await context.Send(new Turn(Player.PlanetId, Player.Drone1Id, 15));
        }
    }
}

public class ShotResultHandler : IHandleMessages<ShotResult>
{

    private readonly ILogger<ShotResultHandler> _logger;

    public ShotResultHandler(ILogger<ShotResultHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ShotResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("ShotResult received {result}", message);
        await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
    }
}


public class TurnResultHander : IHandleMessages<TurnResult>
{
    private readonly ILogger<TurnResultHander> _logger;

    public TurnResultHander(ILogger<TurnResultHander> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TurnResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("TurnResult received {result}", message);
        Random random = new Random();
        if (random.NextDouble() <= 0.7)
        {
            await context.Send(new Move(Player.PlanetId, Player.Drone1Id)); // Example turn command
        }
        else
        {
            await context.Send(new ScanEnvironment(Player.Drone1Id, Player.PlanetId));
        }

    }
}

public class MoveResultHandler : IHandleMessages<MoveResult>
{
    private readonly ILogger<MoveResultHandler> _logger;

    public MoveResultHandler(ILogger<MoveResultHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(MoveResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("MoveResult received {result}", message);
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
}