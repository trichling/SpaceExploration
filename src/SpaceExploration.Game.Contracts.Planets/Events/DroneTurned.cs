using SpaceExploration.Game.Contracts.Planets;

namespace SpaceExploration.Game.Contracts.Planets.Events;

public record DroneTurned(Guid PlanetId, Guid DroneSignature, int Heading, double X, double Y);