using CliFx;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Planets.Commands;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Cli");

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.ConnectionString(configuration["ConnectionStrings:Transport"]);
transport.SubscriptionRuleNamingConvention(type => type.Name);

var routing = transport.Routing();
routing.RouteToEndpoint(typeof(CreatePlanet).Assembly, "SpaceExploration.Game");
routing.RouteToEndpoint(typeof(Move).Assembly, "SpaceExploration.Game");

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.EnableInstallers();
endpointConfiguration.SendOnly();

var endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration);

return await new CliApplicationBuilder()
    .SetDescription("Demo application showcasing CliFx features.")
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(commandTypes =>
    {
        // We use Microsoft.Extensions.DependencyInjection for injecting dependencies in commands
        var services = new ServiceCollection();
        services.AddSingleton(endpoint);

        // Register all commands as transient services
        foreach (var commandType in commandTypes)
            services.AddTransient(commandType);

        return services.BuildServiceProvider();
    })
    .Build()
    .RunAsync();

