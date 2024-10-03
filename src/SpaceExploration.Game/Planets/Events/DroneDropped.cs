using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Events;

public record DroneDropped(Guid PlanetId, Guid DroneId, string DroneType, string DroneName, double X, double Y, int Heading, int OverallDroneCount);