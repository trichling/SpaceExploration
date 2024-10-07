using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus.Testing;

using SpaceExploration.Game.Contracts.Planets.Commands;
using SpaceExploration.Game.Contracts.Planets.Events;
using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Tests;

public class Planet_DropDrone_Tests
{
    private ILogger<Planet> _logger;

    public Planet_DropDrone_Tests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<Planet>();
    }


    [Fact]
    public async Task DropDrone_AddsDroneToThePlanet()
    {
        var planetId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId
            }
        };
        var context = new TestableMessageHandlerContext();

        var droneId = Guid.NewGuid();   
        var message = new DropDrone(droneId, planetId);
        await saga.Handle(message, context);

        Assert.Equal(1, saga.Data.Drones.Count);
        var addedDrone = saga.Data.Drones.Single();
        Assert.Equal(droneId, addedDrone.DroneId);
    }

    [Fact]
    public async Task DropDrone_DroneIdMustBeUnique()
    {
        var planetId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId
            }
        };
        var context = new TestableMessageHandlerContext();

        var droneId = Guid.NewGuid();   
        var message = new DropDrone(droneId, planetId);
        // Add drone with same id twice
        await saga.Handle(message, context);
        await saga.Handle(message, context);

        // there should be only one drone
        Assert.Single(saga.Data.Drones);
        var addedDrone = saga.Data.Drones.Single();
        Assert.Equal(droneId, addedDrone.DroneId);
    }

    [Fact]
    public async Task DropDrone_PublishesDroneDropped()
    {
        var planetId = Guid.NewGuid();
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
            {
                PlanetId = planetId
            }
        };
        var context = new TestableMessageHandlerContext();

        var droneId = Guid.NewGuid();   
        var message = new DropDrone(droneId, planetId);
        await saga.Handle(message, context);

        var droneDroppedGame = context.PublishedMessages.Single().Message as DroneDropped;

        Assert.NotNull(droneDroppedGame);
        Assert.Equal(message.PlanetId, droneDroppedGame.PlanetId);
        Assert.Equal(droneId, droneDroppedGame.DroneId);

    }
}