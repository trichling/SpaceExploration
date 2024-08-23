using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Events;

public record DroneDropped(Guid PlanetId, Guid DroneId, double X, double Y, int Heading, int OverallDroneCount);