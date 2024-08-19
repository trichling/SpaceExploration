using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus.Testing;
using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Messages;
using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Tests;

public class Planet_ScanEnvironment_Tests
{
    private ILogger<Planet> _logger;

    public Planet_ScanEnvironment_Tests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<Planet>();
    }


  [Fact]
    public async Task ScanEnvironment_CreatesADroneReadingInTheSagaAndReturnsItWithoutTheDroneId()
    {
        var planetId = Guid.NewGuid();
        var scanningDrone = Guid.NewGuid();
        var visibleDroneId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(scanningDrone, new Coordinate(0.5, 0.5), new Angle(180)),
                    new Drone(visibleDroneId, new Coordinate(0.45, 0.5), new Angle(0)),
                    // Out of view
                    new Drone(Guid.NewGuid(), new Coordinate(0.6, 0.6), new Angle(0)),

                }   
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new ScanEnvironment(scanningDrone, planetId);

        await saga.Handle(message, context);

        var scanResult = (ScanEnvironmentResult) context.RepliedMessages.Single().Message;
        Assert.Single(scanResult.SensorReadings);
        var droneReading = scanResult.SensorReadings.Single();
        Assert.Equal(0, droneReading.Heading);

        Assert.True(saga.Data.SensorReadings.ContainsKey(scanningDrone));
        var sensorReading = saga.Data.SensorReadings[scanningDrone].Single();
        Assert.Equal(visibleDroneId, sensorReading.DroneId);    
        Assert.Equal(sensorReading.ReadingId, droneReading.ReadingId);
    }

}