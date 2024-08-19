using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Events;

public record DroneTurned(Guid PlanetId, Guid DroneId, Angle Heading);