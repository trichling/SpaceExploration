using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus.Testing;
using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Contracts.Events;
using SpaceExploration.Game.Contracts.Messages;
using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Tests;

public class Planet_CreatePlanet_Tests
{
    private ILogger<Planet> _logger;

    public Planet_CreatePlanet_Tests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        _logger = factory.CreateLogger<Planet>();
    }

    [Fact]
    public async Task CreatePlanet_RecordsPlanetId()
    {
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
        };
        var context = new TestableMessageHandlerContext();

        var planetId = Guid.NewGuid();
        var message = new CreatePlanet(planetId, "Earth");
        await saga.Handle(message, context);


        Assert.Equal(message.PlanetId, saga.Data.PlanetId);
        Assert.False(saga.Completed);
    }

    [Fact]
    public async Task CreatePlanet_PublishesPlanetCreatedEvent()
    {
        var saga = new Planet(_logger)
        {
            Data = new PlanetData()
        };
        var context = new TestableMessageHandlerContext();

        var planetId = Guid.NewGuid();
        var message = new CreatePlanet(planetId, "Earth");

        await saga.Handle(message, context);


        var publishedMessage = context.PublishedMessages.Single().Message as PlanetCreated;

        Assert.NotNull(publishedMessage);
        Assert.Equal(message.PlanetId, publishedMessage.PlanetId);
    }
    
}