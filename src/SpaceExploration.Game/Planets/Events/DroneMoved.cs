using SpaceExploration.Game.Planets;

namespace SpaceExploration.Game.Events;

public record DroneMoved(Guid PlanetId, Guid DroneId, Coordinate Position);