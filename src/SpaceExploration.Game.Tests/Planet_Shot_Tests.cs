using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NServiceBus.Testing;

using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Drones.Events;
using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Tests;

public class Planet_Shot_Tests
{
    private ILogger<Planet> _logger;

    public Planet_Shot_Tests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<Planet>();
    }


    [Fact]
    public async Task Shot_IssuesDamageToARedingAnInvalidatesPastSensorReadings()
    {
        var planetId = Guid.NewGuid();
        var scanningDrone = Guid.NewGuid();
        var visibleDroneId = Guid.NewGuid();
        var visibleDroneSignature = Guid.NewGuid();
        var readingId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(scanningDrone, new Coordinate(0.5, 0.5), new Angle(180)),
                    new Drone(visibleDroneId, new Coordinate(0.45, 0.5), new Angle(0)),
                },
                SensorReadings = new Dictionary<Guid, List<SensorReading>>()
                {
                    { scanningDrone, new List<SensorReading>()
                        {
                            new SensorReading(readingId, visibleDroneId, visibleDroneSignature, 0.05, 0.0)
                        }
                    }
                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Shot(planetId, scanningDrone, readingId);

        await saga.Handle(message, context);

        Assert.Equal(9, saga.Data.Drones.Single(d => d.DroneId == visibleDroneId).Health);
        Assert.False(saga.Data.SensorReadings.ContainsKey(scanningDrone));
    }

    [Fact]
    public async Task Shot_WhenDroneIsHit_DroneHitIsPublished()
    {
        var planetId = Guid.NewGuid();
        var scanningDrone = Guid.NewGuid();
        var visibleDroneId = Guid.NewGuid();
        var visibleDroneSignature = Guid.NewGuid();
        var readingId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(scanningDrone, new Coordinate(0.5, 0.5), new Angle(180)),
                    new Drone(visibleDroneId, visibleDroneSignature, new Coordinate(0.45, 0.5), new Angle(0)),
                },
                SensorReadings = new Dictionary<Guid, List<SensorReading>>()
                {
                    { scanningDrone, new List<SensorReading>()
                        {
                            new SensorReading(readingId, visibleDroneId, visibleDroneSignature, 0.05, 0)
                        }
                    }
                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Shot(planetId, scanningDrone, readingId);

        await saga.Handle(message, context);

        var droneHitEvent = context.PublishedMessages.Single().Message as DroneHit;
        Assert.Equal(planetId, droneHitEvent.PlanetId);
        Assert.Equal(visibleDroneSignature, droneHitEvent.TargetDroneSignature);
        Assert.Equal(9, droneHitEvent.RemainingHealth);
    }

    [Fact]
    public async Task Shot_ADroneWithHeahlth0IsDestroyed()
    {
        var planetId = Guid.NewGuid();
        var scanningDrone = Guid.NewGuid();
        var visibleDroneId = Guid.NewGuid();
        var visibleDroneSignature = Guid.NewGuid();
        var readingId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(scanningDrone, new Coordinate(0.5, 0.5), new Angle(180)),
                    new Drone(visibleDroneId, new Coordinate(0.45, 0.5), new Angle(0), 1),
                },
                SensorReadings = new Dictionary<Guid, List<SensorReading>>()
                {
                    { scanningDrone, new List<SensorReading>()
                        {
                            new SensorReading(readingId, visibleDroneId,visibleDroneSignature, 0.05, 0)
                        }
                    }
                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Shot(planetId, scanningDrone, readingId);

        await saga.Handle(message, context);

        Assert.Single(saga.Data.Drones);
        var leftoverDrone = saga.Data.Drones.Single();
        Assert.Equal(scanningDrone, leftoverDrone.DroneId);
    }

    [Fact]
    public async Task Shot_WhenADroneIsDestroyed_DroneDestroyedIsPublished()
    {
        var planetId = Guid.NewGuid();
        var scanningDrone = Guid.NewGuid();
        var visibleDroneId = Guid.NewGuid();
        var visibleDroneSignature = Guid.NewGuid();
        var readingId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(scanningDrone, new Coordinate(0.5, 0.5), new Angle(180)),
                    new Drone(visibleDroneId, visibleDroneSignature, new Coordinate(0.45, 0.5), new Angle(0), 1),
                },
                SensorReadings = new Dictionary<Guid, List<SensorReading>>()
                {
                    { scanningDrone, new List<SensorReading>()
                        {
                            new SensorReading(readingId, visibleDroneId, visibleDroneSignature, 0.05, 0)
                        }
                    }
                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Shot(planetId, scanningDrone, readingId);

        await saga.Handle(message, context);

        var droneDestroyedEvent = context.PublishedMessages.Single().Message as DroneDestroyed;
        Assert.Equal(planetId, droneDestroyedEvent.PlanetId);
        Assert.Equal(visibleDroneSignature, droneDestroyedEvent.DroneSignature);
    }

}