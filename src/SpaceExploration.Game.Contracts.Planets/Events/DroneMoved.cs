using SpaceExploration.Game.Contracts.Planets;

namespace SpaceExploration.Game.Contracts.Planets.Events;

public record DroneMoved(Guid PlanetId, Guid DroneId, double X, double Y, int Heading);