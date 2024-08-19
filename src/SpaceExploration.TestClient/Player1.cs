using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Messages;

namespace SpaceExploration.TestClient;

public class Player1 : BackgroundService
{
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
        await _messageSession.Send(new CreatePlanet(new Guid("699BBD37-1719-4942-AC03-5917FF851BA4"), "Earth"));
        await _messageSession.Send(new DropDrone(Drone1Id, new Guid("699BBD37-1719-4942-AC03-5917FF851BA4")));

        for (var i = 0; i < 100; i++)
        {
            await _messageSession.Send(new DropDrone(Player1Id, new Guid("699BBD37-1719-4942-AC03-5917FF851BA4")));
        }

        await _messageSession.Send(new ScanEnvironment(Drone1Id, new Guid("699BBD37-1719-4942-AC03-5917FF851BA4")));

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
            await context.Send(new Shot(new Guid("699BBD37-1719-4942-AC03-5917FF851BA4"), Player1.Drone1Id, firstReading.ReadingId));
        }

        await context.Send(new Turn(new Guid("699BBD37-1719-4942-AC03-5917FF851BA4"), Player1.Drone1Id, 10));
        await context.Send(new ScanEnvironment(Player1.Drone1Id, new Guid("699BBD37-1719-4942-AC03-5917FF851BA4")));

    }
}