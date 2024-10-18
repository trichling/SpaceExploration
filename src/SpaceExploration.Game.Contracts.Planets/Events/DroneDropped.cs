using SpaceExploration.Game.Contracts.Planets;

namespace SpaceExploration.Game.Contracts.Planets.Events;

public record DroneDropped(Guid PlanetId, Guid DroneSignature, string DroneType, string DroneName, double X, double Y, int Heading, int OverallDroneCount);