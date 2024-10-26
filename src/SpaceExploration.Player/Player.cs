using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Messages;

namespace SpaceExploration.Player;

public class Player : BackgroundService
{
    public static Guid PlanetId = new Guid("40bcb1ba-b8a4-47f7-b3e5-00a1c724b47e");
    public static Guid Drone1Id = new Guid("66fa1699-e895-4a74-ace1-35ef643a9397");
    public static Guid Drone1Signature = new Guid("f1814763-ff92-49bf-b120-79a2ff603ff4");

    private readonly ILogger<Player> _logger;
    private readonly IMessageSession _messageSession;

    public Player(ILogger<Player> logger, IMessageSession messageSession)
    {
        _logger = logger;
        _messageSession = messageSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Hit enter to start");
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

    public Task Handle(ScanEnvironmentResult message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Scan result received: {0}", message);
        return Task.CompletedTask;
    }
}