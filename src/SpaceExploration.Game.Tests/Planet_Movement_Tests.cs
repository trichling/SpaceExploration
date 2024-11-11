using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NServiceBus.Testing;

using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Tests;

public class Planet_Movement_Tests
{
    private ILogger<Planet> _logger;

    public Planet_Movement_Tests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<Planet>();
    }

    [Fact]
    public async Task Turn_ChangesHeading()
    {
        var planetId = Guid.NewGuid();
        var droneId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(droneId, new Coordinate(0.5, 0.5), new Angle(180)),

                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Turn(planetId, droneId, 90);

        await saga.Handle(message, context);

        var drone = saga.Data.Drones.Single();
        Assert.Equal(270, drone.Heading.Degrees);
    }

    [Fact]
    public async Task Turn_PublishesDroneTurned()
    {
        var planetId = Guid.NewGuid();
        var droneId = Guid.NewGuid();
        var droneSignature = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(droneId, droneSignature, new Coordinate(0.5, 0.5), new Angle(180)),

                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Turn(planetId, droneId, 90);

        await saga.Handle(message, context);

        var publisedMessages = context.PublishedMessages.Single().Message as DroneTurned;

        Assert.Equal(270, publisedMessages.Heading);
        Assert.Equal(droneSignature, publisedMessages.DroneSignature);
    }

    [Fact]
    public async Task Move_ChangesPosition()
    {
        var planetId = Guid.NewGuid();
        var droneId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(droneId, new Coordinate(0.5, 0.5), new Angle(180)),

                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Move(planetId, droneId);

        await saga.Handle(message, context);

        var drone = saga.Data.Drones.Single();
        Assert.Equal(new Coordinate(0.49, 0.5), drone.Position);
    }

    [Fact]
    public async Task Move_PublishesDroneMoved()
    {
        var planetId = Guid.NewGuid();
        var droneId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId,
                Drones = new List<Drone>
                {
                    new Drone(droneId, new Coordinate(0.5, 0.5), new Angle(180)),

                }
            }
        };

        var context = new TestableMessageHandlerContext();
        var message = new Move(planetId, droneId);

        await saga.Handle(message, context);

        var publisedMessages = context.PublishedMessages.Single().Message as DroneMoved;

        Assert.Equal(0.49, publisedMessages.X, 2);
        Assert.Equal(0.5, publisedMessages.Y, 2);
    }
}