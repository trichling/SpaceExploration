namespace SpaceExploration.Game.Contracts.Planets.Messages;
public record CatchUpResponse(Guid PlanetId, List<Drone> Drones);

public record Drone(Guid DroneId, string DroneType, string DroneName, double X, double Y, int Heading, int Health);