using Microsoft.Data.SqlClient;

using SpaceExploration.Game.Contracts.Drones.Commands;

namespace SpaceExploration.Player;

public static class EndpointConfigurationExtensions
{

    public static TransportExtensions<AzureServiceBusTransport> ConfigureTransport(this EndpointConfiguration endpointConfiguration, string transportConnectionString)
    {
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(transportConnectionString);
        transport.SubscriptionRuleNamingConvention(type => type.Name);

        return transport;
    }

    public static void ConfigureRoutingOfTransport(this TransportExtensions<AzureServiceBusTransport> transport)
    {
        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(Move).Assembly, "SpaceExploration.Game");
    }

    public static void ConfigureConventions(this EndpointConfiguration endpointConfiguration)
    {
        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
        conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));
    }

    public static void ConfigurePersistence(this EndpointConfiguration endpointConfiguration, string persistenceConnectionString)
    {
        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () =>
            {
                return new SqlConnection(persistenceConnectionString);
            });
    }

}